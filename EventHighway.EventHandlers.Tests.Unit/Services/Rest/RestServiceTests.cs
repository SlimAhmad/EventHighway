// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
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

        private static IReadOnlyDictionary<string, string> CreateBearerTokenHandlerParams(
            string url,
            string clientId,
            string clientSecret,
            string scope,
            string grantType,
            string tokenUrl) =>
            new Dictionary<string, string>
            {
                { "url", url },
                { "clientId", clientId },
                { "clientSecret", clientSecret },
                { "scope", scope },
                { "grantType", grantType },
                { "tokenUrl", tokenUrl }
            };

        public static TheoryData<Xeption> CriticalDependencyExceptions()
        {
            var urlNotFoundException = new HttpResponseUrlNotFoundException();
            urlNotFoundException.Data.Add("ErrorCode", new List<string> { "UrlNotFound" });

            var unauthorizedException = new HttpResponseUnauthorizedException();
            unauthorizedException.Data.Add("ErrorCode", new List<string> { "Unauthorized" });

            var forbiddenException = new HttpResponseForbiddenException();
            forbiddenException.Data.Add("ErrorCode", new List<string> { "Forbidden" });

            var methodNotAllowedException = new HttpResponseMethodNotAllowedException();
            methodNotAllowedException.Data.Add("ErrorCode", new List<string> { "MethodNotAllowed" });

            return new TheoryData<Xeption>
            {
                urlNotFoundException,
                unauthorizedException,
                forbiddenException,
                methodNotAllowedException
            };
        }

        private static HttpResponseBadRequestException CreateHttpBadRequestException()
        {
            var httpResponseMessage = new HttpResponseMessage();
            var randomDictionary = new Dictionary<string, List<string>>();

            Enumerable.Range(start: 0, count: new IntRange(min: 2, max: 9).GetValue())
                .ToList().ForEach(_ => randomDictionary.TryAdd(
                    key: GetRandomString(),
                    value: Enumerable.Range(start: 0, count: new IntRange(min: 2, max: 9).GetValue())
                        .Select(_ => GetRandomString()).ToList()));

            var httpResponseBadRequestException =
                new HttpResponseBadRequestException(
                    httpResponseMessage,
                    GetRandomString());

            httpResponseBadRequestException.AddData(randomDictionary);

            return httpResponseBadRequestException;
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);
    }
}
