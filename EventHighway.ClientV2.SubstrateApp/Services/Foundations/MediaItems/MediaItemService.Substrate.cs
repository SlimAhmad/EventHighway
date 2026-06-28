// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Abstractions.EventHandlers;
using EventHighway.ClientV2.SubstrateApp.Models.MediaItems;
using EventHighway.EventHandlers;

namespace EventHighway.ClientV2.SubstrateApp.Services.Foundations.MediaItems
{
    public partial class MediaItemService
    {
        private DelegateEventHandler? externalContributionsEventHandler;

        // The substrate seam: a delegate handler the ExternalMediaContributions listener
        // is wired to. Each contribution is deserialized and ingested via AddMediaItemAsync,
        // which re-emits it onto NFlix-NewReleases for the downstream listeners.
        public IEventHandler ExternalContributionsEventHandler =>
            this.externalContributionsEventHandler ??= new DelegateEventHandler(
                Guid.NewGuid(),
                HandleExternalContributionAsync,
                name: "MediaItemService");

        private async ValueTask<EventHandlerResult> HandleExternalContributionAsync(
            string content,
            CancellationToken cancellationToken)
        {
            MediaItem mediaItem = await this.jsonSerializationBroker.DeserializeAsync<MediaItem>(content);
            await AddMediaItemAsync(mediaItem);

            return new EventHandlerResult
            {
                IsSuccess = true,
                Response = mediaItem.Title,
                ResponseCode = "200",
                ResponseMessage = "OK"
            };
        }
    }
}
