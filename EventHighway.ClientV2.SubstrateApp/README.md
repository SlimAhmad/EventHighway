# EventHighway Client V2 — The NFlix Sample (Substrate + REST)

This sample runs **exactly the same NFlix story** as
[`EventHighway.ClientV2.BasicApp`](../EventHighway.ClientV2.BasicApp/README.md) — NFlix
publishes new releases, BingeBox wants everything, Joe wants only good movies, loop
detection blocks repeats, an unauthenticated send is rejected, Ann joins late and is
back-filled by a targeted replay, and Joe asks for one event to be re-sent.

For the full walkthrough of *that* scenario — participants, secrets, the address, promoted
properties and filter criteria, loop detection, scheduled vs immediate events, archiving
and targeted replay — read the **BasicApp README**. This document only covers the **two
things this sample does differently**.

---

## Difference 1 — everything goes through an `EventSubstrateBroker`

The BasicApp talks to `EventHighwayClient` directly (`client.V2.EventV2Client…`,
`client.V2.ReplayingEventV2Client…`, and so on). This sample never does. Instead it depends
only on an [`IEventSubstrateBroker`](Brokers/EventSubstrates/IEventSubstrateBroker.cs) — a
single seam that wraps the EventHighway dependency:

```csharp
public interface IEventSubstrateBroker
{
    IEventSubstrateBroker RegisterEventHandler(IEventHandler eventHandler);
    ValueTask<EventParticipantV2> AddParticipantAsync(EventParticipantV2 participant, …);
    ValueTask<EventParticipantSecretV2> AddParticipantSecretAsync(EventParticipantSecretV2 secret, …);
    ValueTask<EventAddressV2> RetrieveOrRegisterAddressAsync(EventAddressV2 address, …);
    ValueTask<EventListenerV2> RegisterListenerAsync(EventListenerV2 listener, …);
    ValueTask<EventV2> SubmitEventAsync(EventV2 eventV2, …);
    ValueTask FirePendingEventsAsync(…);
    ValueTask ArchiveEventsAsync(…);
    ValueTask ReplayEventToListenersAsync(Guid eventV2Id, Guid? addressId, IEnumerable<Guid> listenerIds, bool allowReplayOfQuarantinedItem = false, …);
    ValueTask ProcessReplayedEventsAsync(…);
    ValueTask<IQueryable<ListenerEventV2>> RetrieveAllListenerEventsAsync(…);
    ValueTask<IEnumerable<HealthCheckItemV2>> RetrieveHealthRagStatusAsync(…);
}
```

The implementation,
[`EventSubstrateBroker`](Brokers/EventSubstrates/EventSubstrateBroker.cs), constructs the
`EventHighwayClient` internally and forwards each call to the matching V2 client — for
example:

```csharp
public EventSubstrateBroker(string connectionString, EventHighwayConfiguration configuration) =>
    this.eventHighwayClient = new EventHighwayClient(connectionString, configuration);

public ValueTask<EventV2> SubmitEventAsync(EventV2 eventV2, CancellationToken ct = default) =>
    this.eventHighwayClient.V2.EventV2Client.SubmitEventV2Async(eventV2, ct);
```

So the application code reads as plain intent and never names `EventHighwayClient` — it just
takes an `IEventSubstrateBroker`:

```csharp
await broker.AddParticipantAsync(nflix);
await broker.RetrieveOrRegisterAddressAsync(newReleases);
await broker.SubmitEventAsync(eventV2);
await broker.FirePendingEventsAsync();
await broker.ArchiveEventsAsync();
await broker.ReplayEventToListenersAsync(eventId, newReleases.Id, new[] { annListener.Id });
await broker.ProcessReplayedEventsAsync();
```

**Why this matters.** The broker is the only place that knows about EventHighway. This is
the standard "broker wraps the external dependency" pattern: the application is decoupled
from the substrate, the dependency can be swapped or mocked in one place, and the call sites
stay readable. (Per The Standard, brokers are thin pass-throughs with no business logic and
no unit tests of their own.)

### Composition with a DI container

`Program.cs` is a thin composition root, like a web app's. It builds the container, then
drives the sequence: set up participants and their secrets, set up the event addresses,
set up the listeners and their handlers, and finally submit contributions:

```csharp
IServiceCollection services = new ServiceCollection();

services.AddSubstrateApp();

IServiceProvider serviceProvider = services.BuildServiceProvider();

IExternalMediaItemService externalMediaItemService =
    serviceProvider.GetRequiredService<IExternalMediaItemService>();

// ...resolve broker, handlers, IMediaItemService, then:
await SetupParticipantsAndSecretsAsync();
await SetupEventAddressesAsync();
await SetupEventListenersAsync();

await externalMediaItemService.AddExternalMediaItemAsync(externalMediaItem);
```

Registration lives in
[`SubstrateAppRegistration.AddSubstrateApp`](Infrastructure/SubstrateAppRegistration.cs),
which wires the configuration, the WireMock server, the brokers
(`IDateTimeBroker`, `ILoggingBroker`, `IJsonSerializationBroker`, `IEventSubstrateBroker`),
and the foundation services (`IMediaItemService`, `IExternalMediaItemService`) — and registers
the event handlers on the substrate broker as it is built. The end-to-end story itself is
orchestrated in [`Program.cs`](Program.cs), which only depends on the abstractions it
resolves from the container.

### Two foundation services, bridged by the substrate

`ExternalMediaItemService` is the authenticated public intake: it receives an
`ExternalMediaItem` (a media item plus a **mandatory** participant id and secret), validates
the credentials are present, and submits the item onto the **`ExternalMediaContributions`**
address carrying those credentials. Because they are present, EventHighway.Core verifies they
are valid — no extra work is needed.

A listener on `ExternalMediaContributions` hands each contribution to
`MediaItemService.AddMediaItemAsync`, which submits it onto **`NFlix-NewReleases`**, where the
BingeBox / Joe / Ann listeners deliver it to their participants:

```
ExternalMediaItemService (participantId + secret)
   └─submit→ [ExternalMediaContributions]
                └─listener→ MediaItemService.AddMediaItemAsync
                               └─submit→ [NFlix-NewReleases]
                                            └─listeners→ BingeBox / Joe / Ann
```

---

## Difference 2 — Joe and Ann forward releases to a REST API

In the BasicApp every handler just writes to the console. Here, **BingeBox still logs to the
console**, but **Joe and Ann forward each release to a REST API** — exactly the kind of real
integration a `DelegateEventHandler` body is meant for. The endpoint is a
[WireMock](https://github.com/WireMock-Net/WireMock.Net) server stood up in-process
(`SetupWireMock`) that answers an OAuth token request and an events `POST`.

Both REST listeners share one handler factory. It performs the OAuth client-credentials
dance, posts the event `content` to `/events`, and returns the HTTP outcome as the
`EventHandlerResult` (so the result — status code, body — is captured on the
`ListenerEventV2`). The only per-subscriber difference is the label it prints, which matches
the BasicApp's wording:

```csharp
private static DelegateEventHandler CreateRestHandler(string label, WireMockServer wireMock) =>
    new DelegateEventHandler(
        Guid.NewGuid(),
        async (content, cancellationToken) =>
        {
            MediaItem item = Deserialize(content);
            string baseUrl = wireMock.Url ?? string.Empty;
            using var http = new HttpClient();

            // 1) OAuth client-credentials token
            HttpResponseMessage tokenResponse =
                await http.PostAsync($"{baseUrl}/token", /* client_id/secret/scope/grant_type */, cancellationToken);
            string token = /* read access_token from the JSON */;
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // 2) POST the release content to the API
            var eventRequest = new StringContent(content, Encoding.UTF8, "application/json");
            HttpResponseMessage response =
                await http.PostAsync($"{baseUrl}/events", eventRequest, cancellationToken);
            string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            // 3) Same console wording as the BasicApp
            Console.WriteLine(
                $"[{label}] New Release - {item.Title} ({item.Type} with rating of {item.Rating})");

            return new EventHandlerResult
            {
                IsSuccess = response.IsSuccessStatusCode,
                Response = responseBody,
                ResponseCode = ((int)response.StatusCode).ToString(),
                ResponseMessage = response.ReasonPhrase ?? string.Empty
            };
        },
        name: label);
```

```csharp
var joeHandler = CreateRestHandler("Joe", wireMock);
var annHandler = CreateRestHandler("Ann", wireMock);
```

Everything else — Joe's `Type == "Movie" && Rating >= 8.0` filter, the loop-detection
config, the unauthorised-send block, Ann's late-joiner replay, and Joe's single-event
re-run — behaves exactly as in the BasicApp.

---

## Running it

```
dotnet run --project EventHighway.ClientV2.SubstrateApp
```

The output matches the BasicApp's, with Joe and Ann now logging via their REST handler:

```
[BingeBox] New Release - Spider-Man: Across the Spider-Verse (Movie with rating of 8.5)
[Joe] New Release - Spider-Man: Across the Spider-Verse (Movie with rating of 8.5)
...
  BingeBox: handled 4 event(s)
  Joe: handled 2 event(s)
  Ann: handled 4 event(s)
...
── Replaying Spider-Verse to Joe ──
[Joe] New Release - Spider-Man: Across the Spider-Verse (Movie with rating of 8.5)
  Joe: handled 1 event(s)
```
