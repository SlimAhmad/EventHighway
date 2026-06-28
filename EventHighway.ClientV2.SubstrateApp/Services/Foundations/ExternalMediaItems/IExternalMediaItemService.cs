// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.ClientV2.SubstrateApp.Models.ExternalMediaItems;
using EventHighway.ClientV2.SubstrateApp.Models.MediaItems;

namespace EventHighway.ClientV2.SubstrateApp.Services.Foundations.ExternalMediaItems
{
    public interface IExternalMediaItemService
    {
        ValueTask<MediaItem> AddExternalMediaItemAsync(ExternalMediaItem externalMediaItem);
    }
}
