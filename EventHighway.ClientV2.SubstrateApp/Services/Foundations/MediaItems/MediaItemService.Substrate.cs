// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Abstractions.EventHandlers;
using EventHighway.ClientV2.Seed;
using EventHighway.ClientV2.SubstrateApp.Models.MediaItems;
using EventHighway.EventHandlers;

namespace EventHighway.ClientV2.SubstrateApp.Services.Foundations.MediaItems
{
    public partial class MediaItemService
    {
        private DelegateEventHandler? mediaItemReceivedEventHandler;

        // The substrate seam: a delegate handler wired to a listener on "NFlix-NewReleases".
        // It is given only the event content (the serialized MediaItem) — never the publishing
        // participant's id or secret — and simply reports each received media item.
        public IEventHandler MediaItemReceivedEventHandler =>
            this.mediaItemReceivedEventHandler ??= new DelegateEventHandler(
                SeedIdentifiers.MediaItemServiceHandler,
                HandleMediaItemReceivedAsync,
                name: "MediaItemService");

        private async ValueTask<EventHandlerResult> HandleMediaItemReceivedAsync(
            string content,
            CancellationToken cancellationToken)
        {
            MediaItem mediaItem =
                await this.jsonSerializationBroker.DeserializeAsync<MediaItem>(content);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("  [SUCCESS]");
            Console.ResetColor();

            Console.WriteLine(
                $" Internal process received a new media item: {mediaItem.Title}, " +
                $"({mediaItem.Type} - {mediaItem.Rating} rating)");

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
