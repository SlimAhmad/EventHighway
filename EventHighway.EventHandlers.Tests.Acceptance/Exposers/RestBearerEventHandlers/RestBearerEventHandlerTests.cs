// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EventHighway.Abstractions.EventHandlers;
using Tynamix.ObjectFiller;
using WireMock.Server;

namespace EventHighway.EventHandlers.Tests.Acceptance.Exposers.RestBearerEventHandlers
{
    public partial class RestBearerEventHandlerTests : IDisposable
    {
        private readonly WireMockServer wireMockServer;
        private readonly RestBearerEventHandler restBearerEventHandler;

        public RestBearerEventHandlerTests()
        {
            this.wireMockServer = WireMockServer.Start();
            this.restBearerEventHandler = new RestBearerEventHandler();
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static IReadOnlyDictionary<string, string> CreateHandlerParams(
            string url,
            string tokenUrl,
            string clientId,
            string clientSecret,
            string scope,
            string grantType) =>
            new Dictionary<string, string>
            {
                { "Url", url },
                { "TokenUrl", tokenUrl },
                { "ClientId", clientId },
                { "ClientSecret", clientSecret },
                { "Scope", scope },
                { "GrantType", grantType }
            };

        private static EventHandlerResult CreateExpectedEventHandlerResult(
            string responseBody) =>
            new EventHandlerResult
            {
                Response = responseBody,
                ResponseCode = "200",
                ResponseMessage = "OK",
                IsSuccess = true
            };

        public void Dispose() =>
            this.wireMockServer.Stop();
    }
}
