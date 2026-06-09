// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.EventHandlers.Brokers.Apis;

namespace EventHighway.EventHandlers.Servies.Rest
{
    internal class RestService : IRestService
    {
        private readonly IApiBroker apiBroker;

        public RestService(IApiBroker apiBroker)
        {
            this.apiBroker = apiBroker;
        }

        public ValueTask<EventHandlerResult> PostWithSecretAsync(
            string content,
            IReadOnlyDictionary<string, string> handlerParams,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public ValueTask<EventHandlerResult> PostWithBearerTokenAsync(
            string content,
            IReadOnlyDictionary<string, string> handlerParams,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
