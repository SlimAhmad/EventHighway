// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Text.Json;
using System.Text.Json.Serialization;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Clients.EventHighways;
using EventHighway.Core.Models.Configurations;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.EventHandlers;

public partial class Program
{
    // Rating is written as a JSON string so it can be used as a promoted property
    // (promotion reads JSON values as strings) and read back into a double by handlers.
    private static readonly JsonSerializerOptions MediaJsonOptions = new()
    {
        NumberHandling =
            JsonNumberHandling.WriteAsString | JsonNumberHandling.AllowReadingFromString
    };

    private static async Task Main(string[] args)
    {
        string connectionString = string.Concat(
            "Server=(localdb)\\MSSQLLocalDB;Database=EventHighwayDB;",
            "Trusted_Connection=True;MultipleActiveResultSets=true");

        // =========================================================
        // 1) Configure loop detection: only allow 1 identical item per minute
        // =========================================================
        var configuration = new EventHighwayConfiguration();
        configuration.LoopDetection.Enabled = true;
        configuration.LoopDetection.Threshold = 0;
        configuration.LoopDetection.Window = TimeSpan.FromMinutes(1);

        var client = new EventHighwayClient(connectionString, configuration);

        // =========================================================
        // 2) Create and register the handlers
        // =========================================================
        var bingeBoxHandler = new DelegateEventHandler(
            Guid.NewGuid(),
            (content, cancellationToken) =>
            {
                MediaItem item = Deserialize(content);

                Console.WriteLine(
                    $"[BingeBox] New Release - {item.Title} " +
                    $"({item.Type} with rating of {item.Rating})");

                return ValueTask.FromResult(new EventHandlerResult
                {
                    IsSuccess = true,
                    Response = item.Title,
                    ResponseCode = "200",
                    ResponseMessage = "OK"
                });
            },
            name: "BingeBox");

        var joeHandler = new DelegateEventHandler(
            Guid.NewGuid(),
            (content, cancellationToken) =>
            {
                MediaItem item = Deserialize(content);

                Console.WriteLine(
                    $"[Joe] New Release - {item.Title} " +
                    $"({item.Type} with rating of {item.Rating})");

                return ValueTask.FromResult(new EventHandlerResult
                {
                    IsSuccess = true,
                    Response = item.Title,
                    ResponseCode = "200",
                    ResponseMessage = "OK"
                });
            },
            name: "Joe");

        client.V2
            .RegisterEventHandler(bingeBoxHandler)
            .RegisterEventHandler(joeHandler);

        DateTimeOffset now = DateTimeOffset.UtcNow;

        // =========================================================
        // 3) Register the publishing participant (NFlix) and its secret
        // =========================================================
        EventParticipantV2 nflix =
            await client.V2.EventParticipantV2Client.AddEventParticipantV2Async(
                new EventParticipantV2
                {
                    Name = "NFlix",
                    Description = "NFlix streaming platform.",
                    IsActive = true,
                    CreatedDate = now,
                    UpdatedDate = now
                });

        await client.V2.EventParticipantSecretV2Client.AddEventParticipantSecretV2Async(
            new EventParticipantSecretV2
            {
                Secret = "NFlix",
                ParticipantId = nflix.Id,
                IsActive = true,
                CreatedDate = now,
                UpdatedDate = now
            });

        // =========================================================
        // 4) Register (or add) the event address
        // =========================================================
        EventAddressV2 newReleases =
            await client.V2.EventAddressV2Client.RetrieveOrRegisterEventAddressV2Async(
                new EventAddressV2
                {
                    Id = Guid.NewGuid(),
                    Name = "NFlix-NewReleases",
                    Description = "NFlix New Releases",
                    CreatedDate = now,
                    UpdatedDate = now
                });

        // =========================================================
        // 5) BingeBox participant + listener (receives every release)
        // =========================================================
        EventParticipantV2 bingeBox =
            await client.V2.EventParticipantV2Client.AddEventParticipantV2Async(
                new EventParticipantV2
                {
                    Name = "BingeBox",
                    Description = "BingeBox a NFlix affiliate",
                    IsActive = true,
                    CreatedDate = now,
                    UpdatedDate = now
                });

        var bingeBoxListener =
            await client.V2.EventListenerV2Client.RegisterEventListenerV2Async(
                new EventListenerV2
                {
                    Id = Guid.NewGuid(),
                    Name = "BingeBox New Releases Listener",
                    Description = "Receives every NFlix new release.",
                    HandlerId = bingeBoxHandler.Id,
                    HandlerName = bingeBoxHandler.Name,
                    EventAddressId = newReleases.Id,
                    ParticipantId = bingeBox.Id,
                    CreatedDate = now,
                    UpdatedDate = now
                });

        // =========================================================
        // 6) Joe participant + listener (only good movies)
        // =========================================================
        EventParticipantV2 joe =
            await client.V2.EventParticipantV2Client.AddEventParticipantV2Async(
                new EventParticipantV2
                {
                    Name = "Joe",
                    Description = "Joe, a movie buff.",
                    IsActive = true,
                    CreatedDate = now,
                    UpdatedDate = now
                });

        var joeListener =
            await client.V2.EventListenerV2Client.RegisterEventListenerV2Async(
                new EventListenerV2
                {
                    Id = Guid.NewGuid(),
                    Name = "Joe Good Movies Listener",
                    Description = "Receives movies rated 8.0 or higher.",
                    HandlerId = joeHandler.Id,
                    HandlerName = joeHandler.Name,
                    EventAddressId = newReleases.Id,
                    ParticipantId = joe.Id,
                    PromotedProperties = "Title,Type,Rating",
                    FilterCriteria =
                        "meta(\"Type\") == \"Movie\" && double.Parse(meta(\"Rating\")) >= 8.0",
                    CreatedDate = now,
                    UpdatedDate = now
                });

        // =========================================================
        // 7) Submit events as NFlix (with participant id + secret)
        // =========================================================
        Console.WriteLine("\n── Submitting events ──");

        var yellowstone = new MediaItem
        {
            Id = Guid.NewGuid(),
            Title = "Yellowstone",
            Type = "Series",
            Rating = 8.6
        };

        var spiderVerse = new MediaItem
        {
            Id = Guid.NewGuid(),
            Title = "Spider-Man: Across the Spider-Verse",
            Type = "Movie",
            Rating = 8.5
        };

        var guardians = new MediaItem
        {
            Id = Guid.NewGuid(),
            Title = "Guardians of the Galaxy Vol. 3",
            Type = "Movie",
            Rating = 7.9
        };

        var topGun = new MediaItem
        {
            Id = Guid.NewGuid(),
            Title = "Top Gun: Maverick",
            Type = "Movie",
            Rating = 8.2
        };

        // 1) Yellowstone — scheduled
        await SubmitMediaAsync(client, newReleases.Id, yellowstone,
            scheduled: true, participantId: nflix.Id, secret: "NFlix");

        // 2) Spider-Verse — immediate
        await SubmitMediaAsync(client, newReleases.Id, spiderVerse,
            scheduled: false, participantId: nflix.Id, secret: "NFlix");

        // 3) Guardians — immediate
        await SubmitMediaAsync(client, newReleases.Id, guardians,
            scheduled: false, participantId: nflix.Id, secret: "NFlix");

        // 4) Top Gun — scheduled, submitted 4 times to simulate a loop
        for (int attempt = 1; attempt <= 4; attempt++)
        {
            await SubmitMediaAsync(client, newReleases.Id, topGun,
                scheduled: true, participantId: nflix.Id, secret: "NFlix",
                attempt: attempt);
        }

        // 5) John Wick — unauthorised: null participant id with a random secret
        var johnWick = new MediaItem
        {
            Id = Guid.NewGuid(),
            Title = "John Wick: Chapter 4",
            Type = "Movie",
            Rating = 7.6
        };

        await SubmitMediaAsync(client, newReleases.Id, johnWick,
            scheduled: false, participantId: null, secret: Guid.NewGuid().ToString());

        // =========================================================
        // 8) Fire the scheduled (pending) events
        // =========================================================
        Console.WriteLine("\n── Firing scheduled events ──");
        await Task.Delay(TimeSpan.FromSeconds(3));
        await client.V2.EventV2Client.FireScheduledPendingEventV2sAsync();

        // =========================================================
        // 9) Summary of what each listener actually recorded
        // =========================================================
        await PrintListenerSummaryAsync(
            client,
            (bingeBoxListener.Id, "BingeBox"),
            (joeListener.Id, "Joe"));
    }

    private static async Task SubmitMediaAsync(
        EventHighwayClient client,
        Guid eventAddressId,
        MediaItem item,
        bool scheduled,
        Guid? participantId,
        string secret,
        int attempt = 0)
    {
        DateTimeOffset now = DateTimeOffset.UtcNow;

        var eventV2 = new EventV2
        {
            Id = Guid.NewGuid(),
            Content = JsonSerializer.Serialize(item, MediaJsonOptions),
            EventName = item.Title,
            EventAddressId = eventAddressId,
            ScheduledDate = scheduled ? now.AddSeconds(1) : null,
            ParticipantId = participantId,
            ParticipantSecret = secret,
            CreatedDate = now,
            UpdatedDate = now
        };

        string label = attempt > 0 ? $"{item.Title} (attempt {attempt})" : item.Title;
        string kind = scheduled ? "scheduled" : "immediate";

        try
        {
            await client.V2.EventV2Client.SubmitEventV2Async(eventV2);
            Console.WriteLine($"  ✓ accepted  {label} [{kind}]");
        }
        catch (Exception exception)
        {
            Console.WriteLine($"  ✗ blocked   {label} [{kind}] — {RootMessage(exception)}");
        }
    }

    private static async Task PrintListenerSummaryAsync(
        EventHighwayClient client,
        params (Guid ListenerId, string Participant)[] listeners)
    {
        IQueryable<ListenerEventV2> all =
            await client.V2.ListenerEventV2Client.RetrieveAllListenerEventV2sAsync();

        Console.WriteLine("\n── Listener results ──");

        foreach ((Guid listenerId, string participant) in listeners)
        {
            List<ListenerEventV2> events =
                all.Where(listenerEvent => listenerEvent.EventListenerId == listenerId)
                    .ToList();

            int handled = events.Count(listenerEvent =>
                listenerEvent.ResponseCode == "200");

            Console.WriteLine($"\n  {participant}: handled {handled} event(s)");

            foreach (ListenerEventV2 listenerEvent in events)
            {
                Console.WriteLine(
                    $"    [{listenerEvent.Status}] {listenerEvent.ResponseCode} " +
                    $"{listenerEvent.ResponseMessage} {listenerEvent.Response}");
            }
        }

        Console.WriteLine();
    }

    private static MediaItem Deserialize(string content) =>
        JsonSerializer.Deserialize<MediaItem>(content, MediaJsonOptions)
            ?? new MediaItem();

    private static string RootMessage(Exception exception)
    {
        Exception current = exception;

        while (current.InnerException is not null)
            current = current.InnerException;

        return current.Message;
    }
}

public class MediaItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "Movie" or "Series"
    public List<string> Genres { get; set; } = new();
    public double Rating { get; set; }
}
