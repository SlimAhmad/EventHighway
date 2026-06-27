# EventHighway Client V2 — The NFlix Sample

This sample tells a small story to show how **EventHighway V2** routes events from a
publisher to interested subscribers. Everything in the story is wired up in
[`Program.cs`](Program.cs); this document explains *why* it is wired the way it is, and
then *how* you would do the same in your own application.

---

## 1. The Story

### The cast

**NFlix** is a streaming platform. Every time a new title becomes available it wants to
announce it once, to a single place, and let anyone who cares react however they like.
NFlix does not want to know who its audience is or what each member does with the
announcement — it just publishes "a new release is out".

To publish, NFlix is registered as a **participant** and is given a **secret**. Think of
the secret as NFlix's API key: every announcement NFlix sends is signed with its
participant id and secret so the system can prove the message genuinely came from NFlix.

NFlix announces its releases on a single channel — an **event address** called
**`NFlix-NewReleases`**. Everything flows through this one address.

### The subscribers

Two parties are interested in NFlix's releases, and their interests are different:

| Subscriber | Who they are | What they care about |
|---|---|---|
| **BingeBox** | A NFlix affiliate that re-lists everything | **Every** new release, no exceptions |
| **Joe** | An individual movie buff | **Only good movies** — a `Movie` (not a series) rated **8.0 or higher** |

Both are registered as **participants** and each owns an **event listener** on the
`NFlix-NewReleases` address. A listener is a standing subscription: "when something lands
on this address, run my code."

- **BingeBox's listener has no filter.** It receives every release and reacts to all of them.
- **Joe's listener has a filter.** Before Joe's code is ever run, EventHighway checks the
  release against Joe's rule (`Type == "Movie"` **and** `Rating >= 8.0`). If the release
  does not match, Joe is quietly skipped — his code is never invoked for things he does
  not care about.

### What NFlix announces

NFlix publishes five titles. Each is a `MediaItem` (`Title`, `Type`, `Rating`, …):

| # | Title | Type | Rating | How it is sent |
|---|---|---|---|---|
| 1 | Yellowstone | Series | 8.6 | scheduled |
| 2 | Spider-Man: Across the Spider-Verse | Movie | 8.5 | immediate |
| 3 | Guardians of the Galaxy Vol. 3 | Movie | 7.9 | immediate |
| 4 | Top Gun: Maverick | Movie | 8.2 | scheduled, **sent 4 times** |
| 5 | John Wick: Chapter 4 | Movie | 7.6 | immediate, **sent with no participant id** |

A couple of these are deliberately mischievous so we can see the platform's safety nets:

- **Title 4 is announced four times.** A healthy publisher should not shout the same
  release over and over. EventHighway's **loop detection** is configured to allow only
  **one identical announcement per minute**, so the first Top Gun gets through and the
  next three are **quarantined** (blocked, never delivered).
- **Title 5 is sent without a participant id** (but with a random secret). This simulates
  an unauthenticated sender. EventHighway **rejects it** before it is ever stored.

### What each subscriber ends up seeing

| Release | BingeBox (everything) | Joe (good movies only) |
|---|---|---|
| Yellowstone (Series, 8.6) | ✅ delivered | ⏭️ skipped (not a Movie) |
| Spider-Verse (Movie, 8.5) | ✅ delivered | ✅ delivered |
| Guardians (Movie, 7.9) | ✅ delivered | ⏭️ skipped (rating < 8.0) |
| Top Gun #1 (Movie, 8.2) | ✅ delivered | ✅ delivered |
| Top Gun #2–4 | 🚫 loop-detected | 🚫 loop-detected |
| John Wick (no participant) | 🚫 unauthorised | 🚫 unauthorised |

So **BingeBox handles 4 releases** and **Joe handles 2** — exactly the two good movies.
Running the sample prints this, ending with a per-subscriber summary.

```
[Joe] New Release - Spider-Man: Across the Spider-Verse (Movie with rating of 8.5)
[BingeBox] New Release - Spider-Man: Across the Spider-Verse (Movie with rating of 8.5)
[BingeBox] New Release - Guardians of the Galaxy Vol. 3 (Movie with rating of 7.9)
...
  BingeBox: handled 4 event(s)
  Joe: handled 2 event(s)
```

---

## 2. How it is set up

The story above maps almost one-to-one onto the EventHighway V2 API. Here is each step.

### Step 1 — Create the client and configure loop detection

The client is created with a connection string and an optional configuration. Here we
tune loop detection so the story's "shout it four times" scenario is blocked after the
first announcement.

```csharp
var configuration = new EventHighwayConfiguration();
configuration.LoopDetection.Enabled = true;
configuration.LoopDetection.Threshold = 0;             // allow no repeats…
configuration.LoopDetection.Window = TimeSpan.FromMinutes(1);  // …within a one-minute window

var client = new EventHighwayClient(connectionString, configuration);
```

Loop detection fingerprints each event by address + name + a hash of its content. With
`Threshold = 0`, the first occurrence is allowed and any identical event seen again
within the window is **quarantined**.

### Step 2 — Write the subscribers' handlers

A handler is the code that runs when an event is delivered. The simplest kind is a
`DelegateEventHandler`: you give it a stable `Id`, a function, and (optionally) a **name**.

```csharp
var bingeBoxHandler = new DelegateEventHandler(
    Guid.NewGuid(),
    (content, cancellationToken) =>
    {
        MediaItem item = Deserialize(content);
        Console.WriteLine($"[BingeBox] New Release - {item.Title} ({item.Type} with rating of {item.Rating})");

        return ValueTask.FromResult(new EventHandlerResult
        {
            IsSuccess = true,
            Response = item.Title,
            ResponseCode = "200",
            ResponseMessage = "OK"
        });
    },
    name: "BingeBox");
```

> **The handler is the integration point.** In this sample the function just **writes to
> the console**, but that body is the place where your real business logic goes. It could
> just as easily **call a REST API** to forward the release to BingeBox's catalogue
> service, drop a message on a queue, write to a database, send an email, or run any other
> in-process logic. Whatever you return in the `EventHandlerResult` (`Response`,
> `ResponseCode`, `ResponseMessage`, `IsSuccess`) is stored so you can audit exactly what
> each subscriber did with each event.

Joe's handler is the same shape (it prints `[Joe] …`). Both are then registered with the
client:

```csharp
client.V2
    .RegisterEventHandler(bingeBoxHandler)
    .RegisterEventHandler(joeHandler);
```

> **Give each handler a distinct identity.** Handlers are resolved by their `Id`, which
> must be unique per registration. The optional `name` ("BingeBox", "Joe") makes logs and
> health output readable.

### Step 3 — Register the publisher (NFlix) and its secret

NFlix is a participant. Adding it returns the participant with its generated `Id`, which we
use to mint a secret. (The service assigns participant and secret ids, so you leave `Id`
unset.)

```csharp
EventParticipantV2 nflix =
    await client.V2.EventParticipantV2Client.AddEventParticipantV2Async(new EventParticipantV2
    {
        Name = "NFlix",
        Description = "NFlix streaming platform.",
        IsActive = true,
        CreatedDate = now,
        UpdatedDate = now
    });

await client.V2.EventParticipantSecretV2Client.AddEventParticipantSecretV2Async(new EventParticipantSecretV2
{
    Secret = "NFlix",
    ParticipantId = nflix.Id,
    IsActive = true,
    CreatedDate = now,
    UpdatedDate = now
});
```

### Step 4 — Register (or add) the channel

The address is the single channel everything flows through. `RetrieveOrRegister` makes the
call idempotent.

```csharp
EventAddressV2 newReleases =
    await client.V2.EventAddressV2Client.RetrieveOrRegisterEventAddressV2Async(new EventAddressV2
    {
        Id = Guid.NewGuid(),
        Name = "NFlix-NewReleases",
        Description = "NFlix New Releases",
        CreatedDate = now,
        UpdatedDate = now
    });
```

### Step 5 — Subscribe BingeBox (everything)

BingeBox is a participant that owns a listener on the address. No filter means it receives
every release.

```csharp
EventParticipantV2 bingeBox = await client.V2.EventParticipantV2Client.AddEventParticipantV2Async(/* … */);

await client.V2.EventListenerV2Client.RegisterEventListenerV2Async(new EventListenerV2
{
    Id = Guid.NewGuid(),
    Name = "BingeBox New Releases Listener",
    HandlerId = bingeBoxHandler.Id,
    HandlerName = bingeBoxHandler.Name,
    EventAddressId = newReleases.Id,
    ParticipantId = bingeBox.Id,
    CreatedDate = now,
    UpdatedDate = now
});
```

### Step 6 — Subscribe Joe (good movies only)

Joe's listener adds two things on top of BingeBox's:

- **`PromotedProperties`** — a comma-separated list of fields to lift out of the event
  content so they can be tested (`"Title,Type,Rating"`).
- **`FilterCriteria`** — a boolean expression over those promoted properties. Joe is only
  invoked when it evaluates to `true`.

```csharp
await client.V2.EventListenerV2Client.RegisterEventListenerV2Async(new EventListenerV2
{
    Id = Guid.NewGuid(),
    Name = "Joe Good Movies Listener",
    HandlerId = joeHandler.Id,
    HandlerName = joeHandler.Name,
    EventAddressId = newReleases.Id,
    ParticipantId = joe.Id,
    PromotedProperties = "Title,Type,Rating",
    FilterCriteria = "meta(\"Type\") == \"Movie\" && double.Parse(meta(\"Rating\")) >= 8.0",
    CreatedDate = now,
    UpdatedDate = now
});
```

`meta("…")` reads a promoted property by name. When Joe's filter does not match, the event
is recorded as `SkippedNotMatchingFilter` and Joe's handler is never called.

### Step 7 — NFlix publishes the releases

Each release is sent as an `EventV2`. The content is the JSON of a `MediaItem`, signed with
NFlix's participant id and secret. `ScheduledDate` decides *when* it runs:

- `null` (or a past time) → **immediate**: delivered as part of the submit call.
- a future time → **scheduled**: stored and delivered later when you fire pending events.

```csharp
var eventV2 = new EventV2
{
    Id = Guid.NewGuid(),
    Content = JsonSerializer.Serialize(item, MediaJsonOptions),
    EventName = item.Title,
    EventAddressId = newReleases.Id,
    ScheduledDate = scheduled ? DateTimeOffset.UtcNow.AddSeconds(1) : null,
    ParticipantId = nflix.Id,           // who is sending
    ParticipantSecret = "NFlix",        // proof it is really NFlix
    CreatedDate = now,
    UpdatedDate = now
};

await client.V2.EventV2Client.SubmitEventV2Async(eventV2);
```

This single call is where the safety nets fire:

- The **four Top Gun** submissions share identical content, so after the first one the rest
  trip loop detection and throw `LoopDetectedEventV2CoordinationException`.
- The **John Wick** submission has `ParticipantId = null` but a secret, which is rejected up
  front ("a participant secret requires a participant id").

The sample wraps each submit in a `try/catch` and prints whether it was accepted or blocked.

### Step 8 — Fire the scheduled releases

Immediate events have already been delivered. Scheduled ones (Yellowstone, the surviving
Top Gun) are waiting until their time passes; firing pending events delivers them.

```csharp
await client.V2.EventV2Client.FireScheduledPendingEventV2sAsync();
```

In a real service this runs on a timer or background job.

### Step 9 — Read what each subscriber did

Every delivery — success, skip, or error — is recorded as a `ListenerEventV2`. The sample
retrieves them and groups by subscriber to produce the final summary.

```csharp
IQueryable<ListenerEventV2> all =
    await client.V2.ListenerEventV2Client.RetrieveAllListenerEventV2sAsync();

foreach (ListenerEventV2 listenerEvent in all.Where(e => e.EventListenerId == bingeBoxListener.Id))
{
    // listenerEvent.Status, ResponseCode, ResponseMessage, Response
}
```

---

## 3. The content schema

The event payload is a plain object. NFlix and its subscribers simply agree on its shape:

```csharp
public class MediaItem
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Type { get; set; }      // "Movie" or "Series"
    public List<string> Genres { get; set; } = new();
    public double Rating { get; set; }
}
```

> **A note on `Rating`.** Promoted properties are read out of the JSON as strings, so the
> sample serialises numbers as strings (`JsonNumberHandling.WriteAsString`) and reads them
> back as numbers (`AllowReadingFromString`). That is why Joe's filter parses the rating
> with `double.Parse(meta("Rating"))` before comparing it.

---

## 4. The actors at a glance

| Actor | EventHighway concept | Role in the story |
|---|---|---|
| NFlix | Participant + secret | Authenticated publisher of releases |
| `NFlix-NewReleases` | Event address | The single channel releases flow through |
| BingeBox | Participant + listener (no filter) | Wants every release |
| Joe | Participant + listener (promoted props + filter) | Wants only Movies rated ≥ 8.0 |
| A release | Event (`EventV2`) | One announcement, immediate or scheduled |
| The handler delegate | `DelegateEventHandler` | The subscriber's reaction — console here, **your REST call / business logic in production** |
| Loop detection | Configuration | Blocks the same release announced repeatedly |
| Participant validation | Built-in | Blocks unauthenticated senders |
| `ListenerEventV2` | Result record | The audit trail of who handled what |
