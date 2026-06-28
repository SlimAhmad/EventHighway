// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Abstractions.EventHandlers;
using EventHighway.ClientV2.SubstrateApp.Models.MediaItems;

namespace EventHighway.ClientV2.SubstrateApp.Services.Foundations.MediaItems
{
    public interface IMediaItemService
    {
        // Delegate event handler that bridges the ExternalMediaContributions listener
        // into this service: it ingests each external contribution via AddMediaItemAsync.
        IEventHandler ExternalContributionsEventHandler { get; }

        ValueTask<MediaItem> AddMediaItemAsync(MediaItem mediaItem);
        ValueTask<IQueryable<MediaItem>> RetrieveAllMediaItemsAsync();
        ValueTask<MediaItem> RetrieveMediaItemByIdAsync(Guid mediaItemId);
        ValueTask<MediaItem> UpdateMediaItemAsync(MediaItem mediaItem);
        ValueTask<MediaItem> RemoveMediaItemByIdAsync(Guid mediaItemId);
    }
}
