// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.EventHandlers.Brokers.Apis;
using EventHighway.EventHandlers.Services.Rest;

namespace EventHighway.EventHandlers.Servies.Rest
{
    internal partial class RestService : IRestService
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
        TryCatch(async () =>
        {
            ValidatePostWithSecretParams(content, handlerParams);

            HttpResponseMessage response =
                await this.apiBroker.PostWithSecretAsync(
                    content,
                    url: handlerParams["url"],
                    secret: handlerParams["secret"]);

            return new EventHandlerResult
            {
                Response = await response.Content.ReadAsStringAsync(cancellationToken),
                ResponseCode = ((int)response.StatusCode).ToString(),
                ResponseMessage = response.ReasonPhrase,
                IsSuccess = response.IsSuccessStatusCode
            };
        });

        public ValueTask<EventHandlerResult> PostWithBearerTokenAsync(
            string content,
            IReadOnlyDictionary<string, string> handlerParams,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            HttpResponseMessage response =
                await this.apiBroker.PostWithBearerTokenAsync(
                    content,
                    url: handlerParams["url"],
                    clientId: handlerParams["clientId"],
                    clientSecret: handlerParams["clientSecret"],
                    scope: handlerParams["scope"],
                    grantType: handlerParams["grantType"],
                    tokenUrl: handlerParams["tokenUrl"]);

            return new EventHandlerResult
            {
                Response = await response.Content.ReadAsStringAsync(cancellationToken),
                ResponseCode = ((int)response.StatusCode).ToString(),
                ResponseMessage = response.ReasonPhrase,
                IsSuccess = response.IsSuccessStatusCode
            };
        });
    }
}
