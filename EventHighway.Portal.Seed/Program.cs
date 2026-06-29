// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

// Sample-data seeder for the EventHighway health dashboard.
//
// Inserts a year of backdated EventV2 + ListenerEventV2 rows directly through the internal
// StorageBroker, bypassing foundation validation (which forbids backdated CreatedDate). The data
// is attributed to the participants/addresses/listeners that already exist in the database, so the
// dashboard's Day/Week/Month/Year windows, traffic, address, participant, loop, duplicate and retry
// panels all show realistic history. Run the ClientV2.SubstrateApp sample first to create the base
// participants/addresses/listeners, then run this seeder once.

using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;

const string connectionString =
    "Server=(localdb)\\MSSQLLocalDB;Database=EventHighwayDB;" +
    "Trusted_Connection=True;MultipleActiveResultSets=true";

var broker = new StorageBroker(connectionString);

var participants = (await broker.SelectAllEventParticipantV2sAsync()).ToList();
var addresses = (await broker.SelectAllEventAddressV2sAsync()).ToList();
var listeners = (await broker.SelectAllEventListenerV2sAsync()).ToList();

Console.WriteLine(
    $"Found {participants.Count} participants, {addresses.Count} addresses, " +
    $"{listeners.Count} listeners.");

EventParticipantV2? nflix =
    participants.FirstOrDefault(participant => participant.Name == "NFlix")
    ?? participants.FirstOrDefault();

var newReleases =
    addresses.FirstOrDefault(address => address.Name == "NFlix-NewReleases")
    ?? addresses.FirstOrDefault();

if (nflix is null || newReleases is null)
{
    Console.WriteLine(
        "No participants/addresses found. Run EventHighway.ClientV2.SubstrateApp first.");

    return;
}

List<EventListenerV2> addressListeners =
    listeners.Where(listener => listener.EventAddressId == newReleases.Id).ToList();

var rng = new Random(20260629);
var events = new List<EventV2>();
var listenerEvents = new List<ListenerEventV2>();

DateTimeOffset startDay = new DateTimeOffset(
    DateTimeOffset.UtcNow.UtcDateTime.Date, TimeSpan.Zero).AddDays(-365);

for (int day = 0; day <= 365; day++)
{
    DateTimeOffset dayStart = startDay.AddDays(day);

    if (dayStart > DateTimeOffset.UtcNow)
    {
        break;
    }

    int eventsToday = rng.Next(1, 6);
    string duplicateHash = "HASH-" + Guid.NewGuid().ToString("N")[..12];

    for (int index = 0; index < eventsToday; index++)
    {
        DateTimeOffset created =
            dayStart.AddHours(rng.Next(0, 24)).AddMinutes(rng.Next(0, 60));

        bool isDuplicate = index > 0 && rng.Next(0, 5) == 0;
        bool isQuarantined = rng.Next(0, 20) == 0;

        int remainingRetries = rng.Next(0, 12) switch
        {
            0 => 0,
            1 => 1,
            2 => 2,
            _ => 3
        };

        var eventV2 = new EventV2
        {
            Id = Guid.NewGuid(),
            Content = "{\"title\":\"Seeded Release\"}",
            EventName = $"Seeded Release {day}-{index}",
            ContentHash = isDuplicate
                ? duplicateHash
                : "HASH-" + Guid.NewGuid().ToString("N")[..12],
            Type = rng.Next(0, 2) == 0 ? EventTypeV2.Immediate : EventTypeV2.Scheduled,
            Status = isQuarantined ? EventStatusV2.Quarantined : EventStatusV2.Active,
            RemainingRetryAttempts = remainingRetries,
            EventAddressId = newReleases.Id,
            ParticipantId = nflix.Id,
            ParticipantSecret = "NFlix",
            CreatedDate = created,
            UpdatedDate = created
        };

        events.Add(eventV2);

        foreach (EventListenerV2 listener in addressListeners)
        {
            bool isError = rng.Next(0, 12) == 0;

            listenerEvents.Add(new ListenerEventV2
            {
                Id = Guid.NewGuid(),
                Status = isError ? ListenerEventStatusV2.Error : ListenerEventStatusV2.Success,
                Response = isError ? "Handler failed" : "Event received",
                ResponseCode = isError ? "500" : "200",
                ResponseMessage = isError ? "Error" : "Success",
                EventId = eventV2.Id,
                EventAddressId = newReleases.Id,
                EventListenerId = listener.Id,
                ParticipantId = listener.ParticipantId,
                CreatedDate = created.AddSeconds(3),
                UpdatedDate = created.AddSeconds(3)
            });
        }
    }
}

Console.WriteLine(
    $"Inserting {events.Count} events and {listenerEvents.Count} listener events " +
    $"attributed to {nflix.Name} on {newReleases.Name}...");

await broker.BulkInsertEventV2sAsync(events);
await broker.BulkInsertListenerEventV2sAsync(listenerEvents);

Console.WriteLine("Seed complete. Refresh the dashboard to see the history.");
