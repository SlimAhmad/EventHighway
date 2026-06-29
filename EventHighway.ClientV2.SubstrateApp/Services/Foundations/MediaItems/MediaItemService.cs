// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.ClientV2.SubstrateApp.Brokers.Serializations;

namespace EventHighway.ClientV2.SubstrateApp.Services.Foundations.MediaItems
{
    // A sample internal subscriber. It owns no CRUD — its only job is to demonstrate how an
    // internal service consumes events off the substrate, via a delegate event handler wired to a
    // listener on "NFlix-NewReleases" (see the .Substrate partial).
    public partial class MediaItemService : IMediaItemService
    {
        private readonly IJsonSerializationBroker jsonSerializationBroker;

        public MediaItemService(IJsonSerializationBroker jsonSerializationBroker) =>
            this.jsonSerializationBroker = jsonSerializationBroker;
    }
}
