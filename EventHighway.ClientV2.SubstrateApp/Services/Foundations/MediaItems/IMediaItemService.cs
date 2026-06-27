// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.ClientV2.SubstrateApp.Models.MediaItems;

namespace EventHighway.ClientV2.SubstrateApp.Services.Foundations.MediaItems
{
    public interface IMediaItemService
    {
        ValueTask<MediaItem> AddMediaItemAsync(MediaItem mediaItem);
        ValueTask<IQueryable<MediaItem>> RetrieveAllMediaItemsAsync();
        ValueTask<MediaItem> RetrieveMediaItemByIdAsync(Guid mediaItemId);
        ValueTask<MediaItem> UpdateMediaItemAsync(MediaItem mediaItem);
        ValueTask<MediaItem> RemoveMediaItemByIdAsync(Guid mediaItemId);
    }
}
