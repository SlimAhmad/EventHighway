// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Abstractions.EventHandlers;
using EventHighway.ClientV2.SubstrateApp;
using EventHighway.ClientV2.SubstrateApp.Brokers.EventSubstrates;
using EventHighway.ClientV2.SubstrateApp.Infrastructure;
using EventHighway.ClientV2.SubstrateApp.Models.ExternalMediaItems;
using EventHighway.ClientV2.SubstrateApp.Models.MediaItems;
using EventHighway.ClientV2.SubstrateApp.Services.Foundations.ExternalMediaItems;
using EventHighway.ClientV2.SubstrateApp.Services.Foundations.MediaItems;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using Microsoft.Extensions.DependencyInjection;

public partial class Program
{
    private static async Task Main(string[] args)
    {
        // =========================================================
        // 1) Service collection / DI registration
        // =========================================================
        IServiceCollection services = new ServiceCollection();

        services.AddSubstrateApp();

        IServiceProvider serviceProvider = services.BuildServiceProvider();

        IEventSubstrateBroker broker =
            serviceProvider.GetRequiredService<IEventSubstrateBroker>();

        MediaEventHandlers handlers =
            serviceProvider.GetRequiredService<MediaEventHandlers>();

        IMediaItemService mediaItemService =
            serviceProvider.GetRequiredService<IMediaItemService>();

        var mediaItemServiceImpl = (MediaItemService)mediaItemService;

        IExternalMediaItemService externalMediaItemService =
            serviceProvider.GetRequiredService<IExternalMediaItemService>();

        // =========================================================
        // 2) Participants and their secrets
        // =========================================================
        (EventParticipantV2 nflix,
            EventParticipantV2 mediaService,
            EventParticipantV2 bingeBox,
            EventParticipantV2 joe) =
                await SetupParticipantsAndSecretsAsync();

        // Attribute the internal release events MediaItemService emits to NFlix so they are not
        // reported as an Unknown participant on the health dashboard.
        mediaItemServiceImpl.PublisherParticipantId = nflix.Id;

        // =========================================================
        // 3) Event addresses
        // =========================================================
        (EventAddressV2 externalContributions, EventAddressV2 newReleases) =
            await SetupEventAddressesAsync();

        // =========================================================
        // 4) Event listeners and their handlers
        // =========================================================
        (EventListenerV2 bingeBoxListener, EventListenerV2 joeListener) =
            await SetupEventListenersAsync();

        // =========================================================
        // 5) External contributions flow through to internal releases
        //    ExternalMediaItemService -> [ExternalMediaContributions]
        //        -> MediaItemService -> [NFlix-NewReleases] -> listeners
        // =========================================================
        Console.WriteLine("\n── Submitting external contributions ──");

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

        await ContributeAsync(yellowstone, participantId: nflix.Id, secret: "NFlix");
        await ContributeAsync(spiderVerse, participantId: nflix.Id, secret: "NFlix");
        await ContributeAsync(guardians, participantId: nflix.Id, secret: "NFlix");

        // Unauthorised: credentials are present (so they are checked) but the secret is wrong.
        var johnWick = new MediaItem
        {
            Id = Guid.NewGuid(),
            Title = "John Wick: Chapter 4",
            Type = "Movie",
            Rating = 7.6
        };

        await ContributeAsync(johnWick, participantId: nflix.Id, secret: Guid.NewGuid().ToString());

        await PrintListenerSummaryAsync(
            (bingeBoxListener.Id, "BingeBox"),
            (joeListener.Id, "Joe"));

        // =========================================================
        // 6) Scheduled releases submitted straight onto the substrate
        //    (minted ids so they can be replayed later)
        // =========================================================
        Console.WriteLine("\n── Submitting scheduled releases ──");

        var topGun = new MediaItem
        {
            Id = Guid.NewGuid(),
            Title = "Top Gun: Maverick",
            Type = "Movie",
            Rating = 8.2
        };

        var acceptedEventIds = new List<Guid>();

        // Submitted 4 times to simulate a loop; loop detection quarantines the duplicates.
        for (int attempt = 1; attempt <= 4; attempt++)
        {
            Guid? acceptedId = await SubmitScheduledReleaseAsync(
                topGun, participantId: nflix.Id, secret: "NFlix", attempt: attempt);

            if (acceptedId.HasValue)
                acceptedEventIds.Add(acceptedId.Value);
        }

        Console.WriteLine("\n── Firing scheduled events ──");
        await Task.Delay(TimeSpan.FromSeconds(3));
        await broker.FirePendingEventsAsync();

        await PrintListenerSummaryAsync(
            (bingeBoxListener.Id, "BingeBox"),
            (joeListener.Id, "Joe"));

        // =========================================================
        // 7) Ann joins late and back-fills via a targeted replay
        // =========================================================
        await broker.ArchiveEventsAsync();

        DateTimeOffset lateNow = DateTimeOffset.UtcNow;

        EventParticipantV2 ann =
            await broker.AddParticipantAsync(
                new EventParticipantV2
                {
                    Name = "Ann",
                    Description = "Ann",
                    IsActive = true,
                    CreatedDate = lateNow,
                    UpdatedDate = lateNow
                });

        EventListenerV2 annListener =
            await broker.RegisterListenerAsync(
                new EventListenerV2
                {
                    Id = Guid.NewGuid(),
                    Name = "Ann New Releases Listener",
                    Description = "Ann, a late joiner who wants the back-catalogue.",
                    HandlerId = handlers.Ann.Id,
                    HandlerName = handlers.Ann.Name,
                    EventAddressId = newReleases.Id,
                    ParticipantId = ann.Id,
                    CreatedDate = lateNow,
                    UpdatedDate = lateNow
                });

        Console.WriteLine("\n── Replaying archived releases to Ann ──");

        foreach (Guid eventId in acceptedEventIds)
        {
            await broker.ReplayEventToListenersAsync(
                eventV2Id: eventId,
                eventAddressId: newReleases.Id,
                eventListenerIds: new[] { annListener.Id },
                allowReplayOfQuarantinedItem: false);
        }

        await broker.ProcessReplayedEventsAsync();

        await PrintListenerSummaryAsync((annListener.Id, "Ann"));

        // =========================================================
        // 8) Health summary
        // =========================================================
        await broker.ArchiveEventsAsync();
        await PrintHealthSummaryAsync();

        // ====================================================================================
        // Setup steps
        // ====================================================================================

        async Task<(EventParticipantV2 Nflix, EventParticipantV2 MediaService,
            EventParticipantV2 BingeBox, EventParticipantV2 Joe)> SetupParticipantsAndSecretsAsync()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            EventParticipantV2 nflixParticipant =
                await broker.AddParticipantAsync(
                    new EventParticipantV2
                    {
                        Name = "NFlix",
                        Description = "NFlix streaming platform.",
                        IsActive = true,
                        CreatedDate = now,
                        UpdatedDate = now
                    });

            await broker.AddParticipantSecretAsync(
                new EventParticipantSecretV2
                {
                    Secret = "NFlix",
                    ParticipantId = nflixParticipant.Id,
                    IsActive = true,
                    CreatedDate = now,
                    UpdatedDate = now
                });

            EventParticipantV2 mediaServiceParticipant =
                await broker.AddParticipantAsync(
                    new EventParticipantV2
                    {
                        Name = "MediaItemService",
                        Description = "Internal service that ingests external contributions.",
                        IsActive = true,
                        CreatedDate = now,
                        UpdatedDate = now
                    });

            EventParticipantV2 bingeBoxParticipant =
                await broker.AddParticipantAsync(
                    new EventParticipantV2
                    {
                        Name = "BingeBox",
                        Description = "BingeBox a NFlix affiliate",
                        IsActive = true,
                        CreatedDate = now,
                        UpdatedDate = now
                    });

            EventParticipantV2 joeParticipant =
                await broker.AddParticipantAsync(
                    new EventParticipantV2
                    {
                        Name = "Joe",
                        Description = "Joe, a movie buff.",
                        IsActive = true,
                        CreatedDate = now,
                        UpdatedDate = now
                    });

            return (nflixParticipant, mediaServiceParticipant, bingeBoxParticipant, joeParticipant);
        }

        async Task<(EventAddressV2 ExternalContributions, EventAddressV2 NewReleases)>
            SetupEventAddressesAsync()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            EventAddressV2 externalContributionsAddress =
                await broker.RetrieveOrRegisterAddressAsync(
                    new EventAddressV2
                    {
                        Id = Guid.NewGuid(),
                        Name = "ExternalMediaContributions",
                        Description = "External media contributions intake.",
                        CreatedDate = now,
                        UpdatedDate = now
                    });

            EventAddressV2 newReleasesAddress =
                await broker.RetrieveOrRegisterAddressAsync(
                    new EventAddressV2
                    {
                        Id = Guid.NewGuid(),
                        Name = "NFlix-NewReleases",
                        Description = "NFlix New Releases",
                        CreatedDate = now,
                        UpdatedDate = now
                    });

            return (externalContributionsAddress, newReleasesAddress);
        }

        async Task<(EventListenerV2 BingeBoxListener, EventListenerV2 JoeListener)>
            SetupEventListenersAsync()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            // Bridge: external contributions are handled by MediaItemService (via its own
            // substrate delegate), which in turn submits the media item onto NFlix-NewReleases.
            IEventHandler contributionsHandler = mediaItemService.ExternalContributionsEventHandler;

            broker.RegisterEventHandler(contributionsHandler);

            await broker.RegisterListenerAsync(
                new EventListenerV2
                {
                    Id = Guid.NewGuid(),
                    Name = "MediaItemService Contributions Listener",
                    Description = "Ingests external contributions into MediaItemService.",
                    HandlerId = contributionsHandler.Id,
                    HandlerName = contributionsHandler.Name,
                    EventAddressId = externalContributions.Id,
                    ParticipantId = mediaService.Id,
                    CreatedDate = now,
                    UpdatedDate = now
                });

            EventListenerV2 bingeBoxListenerResult =
                await broker.RegisterListenerAsync(
                    new EventListenerV2
                    {
                        Id = Guid.NewGuid(),
                        Name = "BingeBox New Releases Listener",
                        Description = "Receives every NFlix new release.",
                        HandlerId = handlers.BingeBox.Id,
                        HandlerName = handlers.BingeBox.Name,
                        EventAddressId = newReleases.Id,
                        ParticipantId = bingeBox.Id,
                        CreatedDate = now,
                        UpdatedDate = now
                    });

            EventListenerV2 joeListenerResult =
                await broker.RegisterListenerAsync(
                    new EventListenerV2
                    {
                        Id = Guid.NewGuid(),
                        Name = "Joe Good Movies Listener",
                        Description = "Forwards movies rated 8.0 or higher to Joe's API.",
                        HandlerId = handlers.Joe.Id,
                        HandlerName = handlers.Joe.Name,
                        EventAddressId = newReleases.Id,
                        ParticipantId = joe.Id,
                        PromotedProperties = "Title,Type,Rating",
                        FilterCriteria =
                            "meta(\"Type\") == \"Movie\" && double.Parse(meta(\"Rating\")) >= 8.0",
                        CreatedDate = now,
                        UpdatedDate = now
                    });

            return (bingeBoxListenerResult, joeListenerResult);
        }

        // ====================================================================================
        // Submission helpers
        // ====================================================================================

        async Task ContributeAsync(MediaItem item, Guid participantId, string secret)
        {
            try
            {
                await externalMediaItemService.AddExternalMediaItemAsync(
                    new ExternalMediaItem
                    {
                        MediaItem = item,
                        ParticipantId = participantId,
                        Secret = secret
                    });

                WriteMarker("  [Success]", ConsoleColor.Green, $" accepted  {item.Title}");
            }
            catch (Exception exception)
            {
                WriteMarker(
                    "  [Fail]   ", ConsoleColor.Red,
                    $" blocked   {item.Title} - {RootMessage(exception)}");
            }
        }

        async Task<Guid?> SubmitScheduledReleaseAsync(
            MediaItem item, Guid participantId, string secret, int attempt)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            var eventV2 = new EventV2
            {
                Id = Guid.NewGuid(),
                Content = MediaItemSerializer.Serialize(item),
                EventName = item.Title,
                EventAddressId = newReleases.Id,
                ScheduledDate = now.AddSeconds(1),
                ParticipantId = participantId,
                ParticipantSecret = secret,
                CreatedDate = now,
                UpdatedDate = now
            };

            try
            {
                await broker.SubmitEventAsync(eventV2);

                WriteMarker(
                    "  [Success]", ConsoleColor.Green,
                    $" accepted  {item.Title} (attempt {attempt}) [scheduled]");

                return eventV2.Id;
            }
            catch (Exception exception)
            {
                WriteMarker(
                    "  [Fail]   ", ConsoleColor.Red,
                    $" blocked   {item.Title} (attempt {attempt}) [scheduled] - {RootMessage(exception)}");

                return null;
            }
        }

        // ====================================================================================
        // Reporting helpers
        // ====================================================================================

        async Task PrintListenerSummaryAsync(params (Guid ListenerId, string Participant)[] listeners)
        {
            IQueryable<ListenerEventV2> all =
                await broker.RetrieveAllListenerEventsAsync();

            Console.WriteLine("\n── Listener results ──");

            foreach ((Guid listenerId, string participant) in listeners)
            {
                List<ListenerEventV2> events =
                    all.Where(listenerEvent => listenerEvent.EventListenerId == listenerId)
                        .ToList();

                int handled = events.Count(listenerEvent => listenerEvent.ResponseCode == "200");

                Console.WriteLine($"\n  {participant}: handled {handled} event(s)");

                foreach (ListenerEventV2 listenerEvent in events)
                {
                    ConsoleColor color = listenerEvent.Status switch
                    {
                        ListenerEventStatusV2.Success => ConsoleColor.Green,
                        ListenerEventStatusV2.Error => ConsoleColor.Red,
                        ListenerEventStatusV2.Pending => ConsoleColor.Yellow,
                        _ => ConsoleColor.Gray,
                    };

                    WriteMarker(
                        $"    [{listenerEvent.Status}]", color,
                        $" {listenerEvent.ResponseCode} " +
                        $"{listenerEvent.ResponseMessage} {listenerEvent.Response}");
                }
            }

            Console.WriteLine();
        }

        async Task PrintHealthSummaryAsync()
        {
            IEnumerable<HealthCheckItemV2> summary =
                await broker.RetrieveHealthRagStatusAsync();

            Console.WriteLine("── Health summary ──");

            string? currentGrouping = null;

            foreach (HealthCheckItemV2 item in summary)
            {
                if (item.Grouping != currentGrouping)
                {
                    currentGrouping = item.Grouping;
                    Console.WriteLine($"\n  {currentGrouping}");
                }

                ConsoleColor color = item.Status switch
                {
                    nameof(HealthStatusV2.Green) => ConsoleColor.Green,
                    nameof(HealthStatusV2.Amber) => ConsoleColor.Yellow,
                    nameof(HealthStatusV2.Red) => ConsoleColor.Red,
                    _ => ConsoleColor.Gray,
                };

                WriteMarker($"    [{item.Status,-5}]", color, $" {item.Item}: {item.Value}");
            }

            Console.WriteLine();
        }

        static void WriteMarker(string marker, ConsoleColor color, string text)
        {
            Console.ForegroundColor = color;
            Console.Write(marker);
            Console.ResetColor();
            Console.WriteLine(text);
        }

        static string RootMessage(Exception exception)
        {
            Exception current = exception;

            while (current.InnerException is not null)
                current = current.InnerException;

            return current.Message;
        }
    }
}