// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EventHighway.Abstractions.EventHandlers;
using Tynamix.ObjectFiller;
using WireMock.Server;

namespace EventHighway.EventHandlers.Tests.Acceptance.Exposers.RestSecretEventHandlers
{
    public partial class RestSecretEventHandlerTests : IDisposable
    {
        private readonly WireMockServer wireMockServer;
        private readonly RestSecretEventHandler restSecretEventHandler;

        public RestSecretEventHandlerTests()
        {
            this.wireMockServer = WireMockServer.Start();
            Guid identifier = Guid.NewGuid();
            this.restSecretEventHandler = new RestSecretEventHandler(id: identifier);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static IReadOnlyDictionary<string, string> CreateHandlerParams(
            string url,
            string secret) =>
            new Dictionary<string, string>
            {
                { "Url", url },
                { "Secret", secret }
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
