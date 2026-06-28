// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.ClientV2.SubstrateApp.Brokers.EventSubstrates;
using EventHighway.ClientV2.SubstrateApp.Infrastructure;
using EventHighway.ClientV2.SubstrateApp.Models.MediaItems;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;

namespace EventHighway.ClientV2.SubstrateApp
{
    public sealed class NFlixSample
    {
        private readonly IEventSubstrateBroker broker;
        private readonly MediaEventHandlers handlers;

        public NFlixSample(IEventSubstrateBroker broker, MediaEventHandlers handlers)
        {
            this.broker = broker;
            this.handlers = handlers;
        }

        public async Task RunAsync()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            // =========================================================
            // 1) Register the publishing participant (NFlix) and its secret
            // =========================================================
            EventParticipantV2 nflix =
                await this.broker.AddParticipantAsync(
                    new EventParticipantV2
                    {
                        Name = "NFlix",
                        Description = "NFlix streaming platform.",
                        IsActive = true,
                        CreatedDate = now,
                        UpdatedDate = now
                    });

            await this.broker.AddParticipantSecretAsync(
                new EventParticipantSecretV2
                {
                    Secret = "NFlix",
                    ParticipantId = nflix.Id,
                    IsActive = true,
                    CreatedDate = now,
                    UpdatedDate = now
                });

            // =========================================================
            // 2) Register (or add) the event address
            // =========================================================
            EventAddressV2 newReleases =
                await this.broker.RetrieveOrRegisterAddressAsync(
                    new EventAddressV2
                    {
                        Id = Guid.NewGuid(),
                        Name = "NFlix-NewReleases",
                        Description = "NFlix New Releases",
                        CreatedDate = now,
                        UpdatedDate = now
                    });

            // =========================================================
            // 3) BingeBox participant + listener (receives every release)
            // =========================================================
            EventParticipantV2 bingeBox =
                await this.broker.AddParticipantAsync(
                    new EventParticipantV2
                    {
                        Name = "BingeBox",
                        Description = "BingeBox a NFlix affiliate",
                        IsActive = true,
                        CreatedDate = now,
                        UpdatedDate = now
                    });

            EventListenerV2 bingeBoxListener =
                await this.broker.RegisterListenerAsync(
                    new EventListenerV2
                    {
                        Id = Guid.NewGuid(),
                        Name = "BingeBox New Releases Listener",
                        Description = "Receives every NFlix new release.",
                        HandlerId = this.handlers.BingeBox.Id,
                        HandlerName = this.handlers.BingeBox.Name,
                        EventAddressId = newReleases.Id,
                        ParticipantId = bingeBox.Id,
                        CreatedDate = now,
                        UpdatedDate = now
                    });

            // =========================================================
            // 4) Joe participant + listener (only good movies, forwarded over REST)
            // =========================================================
            EventParticipantV2 joe =
                await this.broker.AddParticipantAsync(
                    new EventParticipantV2
                    {
                        Name = "Joe",
                        Description = "Joe, a movie buff.",
                        IsActive = true,
                        CreatedDate = now,
                        UpdatedDate = now
                    });

            EventListenerV2 joeListener =
                await this.broker.RegisterListenerAsync(
                    new EventListenerV2
                    {
                        Id = Guid.NewGuid(),
                        Name = "Joe Good Movies Listener",
                        Description = "Forwards movies rated 8.0 or higher to Joe's API.",
                        HandlerId = this.handlers.Joe.Id,
                        HandlerName = this.handlers.Joe.Name,
                        EventAddressId = newReleases.Id,
                        ParticipantId = joe.Id,
                        PromotedProperties = "Title,Type,Rating",
                        FilterCriteria =
                            "meta(\"Type\") == \"Movie\" && double.Parse(meta(\"Rating\")) >= 8.0",
                        CreatedDate = now,
                        UpdatedDate = now
                    });

            // =========================================================
            // 5) Submit events as NFlix (with participant id + secret)
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

            var acceptedEventIds = new List<Guid>();

            // We mint each event id up front so we can track and later replay a specific one.
            Guid spiderVerseEventId = Guid.NewGuid();

            // 1) Yellowstone — scheduled
            AddIfAccepted(acceptedEventIds, await SubmitMediaAsync(Guid.NewGuid(), newReleases.Id, yellowstone,
                scheduled: true, participantId: nflix.Id, secret: "NFlix"));

            // 2) Spider-Verse — immediate
            AddIfAccepted(acceptedEventIds, await SubmitMediaAsync(spiderVerseEventId, newReleases.Id, spiderVerse,
                scheduled: false, participantId: nflix.Id, secret: "NFlix"));

            // 3) Guardians — immediate
            AddIfAccepted(acceptedEventIds, await SubmitMediaAsync(Guid.NewGuid(), newReleases.Id, guardians,
                scheduled: false, participantId: nflix.Id, secret: "NFlix"));

            // 4) Top Gun — scheduled, submitted 4 times to simulate a loop
            for (int attempt = 1; attempt <= 4; attempt++)
            {
                AddIfAccepted(acceptedEventIds, await SubmitMediaAsync(Guid.NewGuid(), newReleases.Id, topGun,
                    scheduled: true, participantId: nflix.Id, secret: "NFlix",
                    attempt: attempt));
            }

            // 5) John Wick — unauthorised: null participant id with a random secret
            var johnWick = new MediaItem
            {
                Id = Guid.NewGuid(),
                Title = "John Wick: Chapter 4",
                Type = "Movie",
                Rating = 7.6
            };

            await SubmitMediaAsync(Guid.NewGuid(), newReleases.Id, johnWick,
                scheduled: false, participantId: null, secret: Guid.NewGuid().ToString());

            // =========================================================
            // 6) Fire the scheduled (pending) events
            // =========================================================
            Console.WriteLine("\n── Firing scheduled events ──");
            await Task.Delay(TimeSpan.FromSeconds(3));
            await this.broker.FirePendingEventsAsync();

            // =========================================================
            // 7) Summary of what the original subscribers recorded
            // =========================================================
            await PrintListenerSummaryAsync(
                (bingeBoxListener.Id, "BingeBox"),
                (joeListener.Id, "Joe"));

            // =========================================================
            // 8) Ann joins late and back-fills via a targeted replay
            // =========================================================
            // Replay sources events from the archive, so first archive the processed
            // events (successful + quarantined) to make them available to replay.
            await this.broker.ArchiveEventsAsync();

            DateTimeOffset lateNow = DateTimeOffset.UtcNow;

            EventParticipantV2 ann =
                await this.broker.AddParticipantAsync(
                    new EventParticipantV2
                    {
                        Name = "Ann",
                        Description = "Ann",
                        IsActive = true,
                        CreatedDate = lateNow,
                        UpdatedDate = lateNow
                    });

            EventListenerV2 annListener =
                await this.broker.RegisterListenerAsync(
                    new EventListenerV2
                    {
                        Id = Guid.NewGuid(),
                        Name = "Ann New Releases Listener",
                        Description = "Ann, a late joiner who wants the back-catalogue.",
                        HandlerId = this.handlers.Ann.Id,
                        HandlerName = this.handlers.Ann.Name,
                        EventAddressId = newReleases.Id,
                        ParticipantId = ann.Id,
                        CreatedDate = lateNow,
                        UpdatedDate = lateNow
                    });

            Console.WriteLine("\n── Replaying archived releases to Ann ──");

            // Targeted replay: re-deliver each archived release to Ann's listener only.
            // Quarantined (loop-detected) events are skipped unless explicitly allowed.
            foreach (Guid eventId in acceptedEventIds)
            {
                await this.broker.ReplayEventToListenersAsync(
                    eventV2Id: eventId,
                    eventAddressId: newReleases.Id,
                    eventListenerIds: new[] { annListener.Id },
                    allowReplayOfQuarantinedItem: false);
            }

            await this.broker.ProcessReplayedEventsAsync();

            await PrintListenerSummaryAsync((annListener.Id, "Ann"));

            // =========================================================
            // 9) Archive again (housekeeping)
            // =========================================================
            await this.broker.ArchiveEventsAsync();

            // =========================================================
            // 10) Joe asks to re-process one specific release he had trouble with
            // =========================================================
            Console.WriteLine("\n── Replaying Spider-Verse to Joe ──");

            await this.broker.ReplayEventToListenersAsync(
                eventV2Id: spiderVerseEventId,
                eventAddressId: newReleases.Id,
                eventListenerIds: new[] { joeListener.Id },
                allowReplayOfQuarantinedItem: false);

            await this.broker.ProcessReplayedEventsAsync();

            await PrintListenerSummaryAsync((joeListener.Id, "Joe"));

            // =========================================================
            // 11) Health summary
            // =========================================================
            await PrintHealthSummaryAsync();
        }

        private async Task<Guid?> SubmitMediaAsync(
            Guid eventV2Id,
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
                Id = eventV2Id,
                Content = MediaItemSerializer.Serialize(item),
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
                await this.broker.SubmitEventAsync(eventV2);

                WriteMarker(
                    "  [Success]", ConsoleColor.Green,
                    $" accepted  {label} [{kind}]");

                return eventV2.Id;
            }
            catch (Exception exception)
            {
                WriteMarker(
                    "  [Fail]   ", ConsoleColor.Red,
                    $" blocked   {label} [{kind}] - {RootMessage(exception)}");

                return null;
            }
        }

        private static void AddIfAccepted(List<Guid> acceptedEventIds, Guid? eventId)
        {
            if (eventId.HasValue)
                acceptedEventIds.Add(eventId.Value);
        }

        private static void WriteMarker(string marker, ConsoleColor color, string text)
        {
            Console.ForegroundColor = color;
            Console.Write(marker);
            Console.ResetColor();
            Console.WriteLine(text);
        }

        private async Task PrintListenerSummaryAsync(
            params (Guid ListenerId, string Participant)[] listeners)
        {
            IQueryable<ListenerEventV2> all =
                await this.broker.RetrieveAllListenerEventsAsync();

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

        private async Task PrintHealthSummaryAsync()
        {
            IEnumerable<HealthCheckItemV2> summary =
                await this.broker.RetrieveHealthRagStatusAsync();

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

                WriteMarker(
                    $"    [{item.Status,-5}]", color,
                    $" {item.Item}: {item.Value}");
            }

            Console.WriteLine();
        }

        private static string RootMessage(Exception exception)
        {
            Exception current = exception;

            while (current.InnerException is not null)
                current = current.InnerException;

            return current.Message;
        }
    }
}
