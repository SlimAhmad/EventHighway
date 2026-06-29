// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.ClientV2.SubstrateApp.Brokers.DateTimes;
using EventHighway.ClientV2.SubstrateApp.Brokers.EventSubstrates;
using EventHighway.ClientV2.SubstrateApp.Brokers.Loggings;
using EventHighway.ClientV2.SubstrateApp.Brokers.Serializations;
using EventHighway.ClientV2.SubstrateApp.Models.ExternalMediaItems;
using EventHighway.ClientV2.SubstrateApp.Models.MediaItems;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;

namespace EventHighway.ClientV2.SubstrateApp.Services.Foundations.ExternalMediaItems
{
    // The authenticated public intake. A media item arrives with the contributing
    // participant's id and secret; once validated as present, it is published directly onto the
    // "NFlix-NewReleases" address carrying those credentials (which EventHighway.Core verifies).
    // The participant attribution lives on that EventV2 record — no downstream handler needs it.
    public partial class ExternalMediaItemService : IExternalMediaItemService
    {
        private const string NewReleasesAddressName = "NFlix-NewReleases";

        private readonly IEventSubstrateBroker eventSubstrateBroker;
        private readonly IJsonSerializationBroker jsonSerializationBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public ExternalMediaItemService(
            IEventSubstrateBroker eventSubstrateBroker,
            IJsonSerializationBroker jsonSerializationBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventSubstrateBroker = eventSubstrateBroker;
            this.jsonSerializationBroker = jsonSerializationBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public async ValueTask AddExternalMediaItemAsync(ExternalMediaItem externalMediaItem) =>
        await TryCatch(async () =>
        {
            ValidateExternalMediaItemOnAdd(externalMediaItem);

            DateTimeOffset now =
                await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

            EventAddressV2 eventAddress =
                await this.eventSubstrateBroker.RetrieveOrRegisterAddressAsync(
                    new EventAddressV2
                    {
                        Id = SeedIdentifiers.StableId("address:" + NewReleasesAddressName),
                        Name = NewReleasesAddressName,
                        Description = NewReleasesAddressName,
                        CreatedDate = now,
                        UpdatedDate = now
                    });

            string content =
                await this.jsonSerializationBroker.SerializeAsync(externalMediaItem.MediaItem);

            var eventV2 = new EventV2
            {
                Id = Guid.NewGuid(),
                Content = content,
                EventName = "MediaItem Added",
                EventAddressId = eventAddress.Id,
                ParticipantId = externalMediaItem.ParticipantId,
                ParticipantSecret = externalMediaItem.Secret,
                CreatedDate = now,
                UpdatedDate = now
            };

            await this.eventSubstrateBroker.SubmitEventAsync(eventV2);

            return externalMediaItem.MediaItem;
        });
    }
}
