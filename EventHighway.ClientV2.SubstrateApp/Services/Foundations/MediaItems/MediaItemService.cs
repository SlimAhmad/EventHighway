// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.ClientV2.SubstrateApp.Brokers.DateTimes;
using EventHighway.ClientV2.SubstrateApp.Brokers.EventSubstrates;
using EventHighway.ClientV2.SubstrateApp.Brokers.Loggings;
using EventHighway.ClientV2.SubstrateApp.Brokers.Serializations;
using EventHighway.ClientV2.SubstrateApp.Models.MediaItems;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;

namespace EventHighway.ClientV2.SubstrateApp.Services.Foundations.MediaItems
{
    public partial class MediaItemService : IMediaItemService
    {
        private readonly IEventSubstrateBroker eventSubstrateBroker;
        private readonly IJsonSerializationBroker jsonSerializationBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public MediaItemService(
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

        public ValueTask<MediaItem> AddMediaItemAsync(MediaItem mediaItem) =>
        TryCatch(async () =>
        {
            ValidateMediaItemOnAdd(mediaItem);

            DateTimeOffset now =
                await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

            // A storage broker would persist the new media item here:
            // MediaItem addedMediaItem =
            //     await this.storageBroker.InsertMediaItemAsync(mediaItem);

            // Emit to the public releases channel, which fans out to the listeners.
            await EmitMediaItemEventAsync(
                eventAddressName: "NFlix-NewReleases",
                eventName: "MediaItem Added",
                mediaItem: mediaItem,
                now: now);

            return mediaItem;
        });

        public ValueTask<IQueryable<MediaItem>> RetrieveAllMediaItemsAsync() =>
        TryCatch(async () =>
        {
            // A storage broker would return the persisted media items here:
            // return await this.storageBroker.SelectAllMediaItemsAsync();

            return await new ValueTask<IQueryable<MediaItem>>(
                Enumerable.Empty<MediaItem>().AsQueryable());
        });

        public ValueTask<MediaItem> RetrieveMediaItemByIdAsync(Guid mediaItemId) =>
        TryCatch(async () =>
        {
            ValidateMediaItemId(mediaItemId);

            // A storage broker would load and validate the media item here:
            // MediaItem maybeMediaItem =
            //     await this.storageBroker.SelectMediaItemByIdAsync(mediaItemId);
            // ValidateStorageMediaItem(maybeMediaItem, mediaItemId);
            // return maybeMediaItem;

            return await new ValueTask<MediaItem>(new MediaItem { Id = mediaItemId });
        });

        public ValueTask<MediaItem> UpdateMediaItemAsync(MediaItem mediaItem) =>
        TryCatch(async () =>
        {
            ValidateMediaItemOnUpdate(mediaItem);

            DateTimeOffset now =
                await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

            // A storage broker would load, validate and update the media item here:
            // MediaItem maybeMediaItem =
            //     await this.storageBroker.SelectMediaItemByIdAsync(mediaItem.Id);
            // ValidateStorageMediaItem(maybeMediaItem, mediaItem.Id);
            // MediaItem updatedMediaItem =
            //     await this.storageBroker.UpdateMediaItemAsync(mediaItem);

            await EmitMediaItemEventAsync(
                eventAddressName: "NFlix-MediaItemUpdated",
                eventName: "MediaItem Updated",
                mediaItem: mediaItem,
                now: now);

            return mediaItem;
        });

        public ValueTask<MediaItem> RemoveMediaItemByIdAsync(Guid mediaItemId) =>
        TryCatch(async () =>
        {
            ValidateMediaItemId(mediaItemId);

            DateTimeOffset now =
                await this.dateTimeBroker.GetCurrentDateTimeOffsetAsync();

            // A storage broker would load, validate and delete the media item here:
            // MediaItem maybeMediaItem =
            //     await this.storageBroker.SelectMediaItemByIdAsync(mediaItemId);
            // ValidateStorageMediaItem(maybeMediaItem, mediaItemId);
            // MediaItem removedMediaItem =
            //     await this.storageBroker.DeleteMediaItemAsync(maybeMediaItem);

            var removedMediaItem = new MediaItem { Id = mediaItemId };

            await EmitMediaItemEventAsync(
                eventAddressName: "NFlix-MediaItemRemoved",
                eventName: "MediaItem Removed",
                mediaItem: removedMediaItem,
                now: now);

            return removedMediaItem;
        });

        // Emits a media item as an event onto a named substrate address. This is the
        // seam that lets an internal service publish to — and (via listeners) receive
        // from — the event substrate without ever touching EventHighwayClient.
        private async ValueTask EmitMediaItemEventAsync(
            string eventAddressName,
            string eventName,
            MediaItem mediaItem,
            DateTimeOffset now)
        {
            EventAddressV2 eventAddress =
                await this.eventSubstrateBroker.RetrieveOrRegisterAddressAsync(
                    new EventAddressV2
                    {
                        Id = Guid.NewGuid(),
                        Name = eventAddressName,
                        Description = eventAddressName,
                        CreatedDate = now,
                        UpdatedDate = now
                    });

            string content = await this.jsonSerializationBroker.SerializeAsync(mediaItem);

            var eventV2 = new EventV2
            {
                Id = Guid.NewGuid(),
                Content = content,
                EventName = eventName,
                EventAddressId = eventAddress.Id,
                CreatedDate = now,
                UpdatedDate = now
            };

            await this.eventSubstrateBroker.SubmitEventAsync(eventV2);
        }
    }
}
