// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.EventHandlers.Brokers.Apis;
using EventHighway.EventHandlers.Services.Rest;
using EventHighway.EventHandlers.Servies.Rest;
using Moq;
using RESTFulSense.Exceptions;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.EventHandlers.Tests.Unit.Services.Rest
{
    public partial class RestServiceTests
    {
        private readonly Mock<IApiBroker> apiBrokerMock;
        private readonly IRestService restService;

        public RestServiceTests()
        {
            this.apiBrokerMock = new Mock<IApiBroker>();

            this.restService =
                new RestService(
                    apiBroker: this.apiBrokerMock.Object);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static EventHandlerResult CreateRandomEventHandlerResult() =>
            new EventHandlerResult
            {
                Response = GetRandomString(),
                ResponseCode = GetRandomString(),
                ResponseMessage = GetRandomString(),
                IsSuccess = true
            };

        private static IReadOnlyDictionary<string, string> CreateSecretHandlerParams(
            string url,
            string secret) =>
            new Dictionary<string, string>
            {
                { "url", url },
                { "secret", secret }
            };

        public static TheoryData<Xeption> CriticalDependencyExceptions() =>
            new TheoryData<Xeption>
            {
                new HttpResponseUrlNotFoundException(),
                new HttpResponseUnauthorizedException(),
                new HttpResponseForbiddenException(),
                new HttpResponseMethodNotAllowedException()
            };

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);
    }
}
