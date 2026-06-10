// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.EventHandlers.Services.Rest;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.EventHandlers.Tests.Unit.Exposers.RestSecretEventHandlers
{
    public partial class RestSecretEventHandlerTests
    {
        private readonly Mock<IRestService> restServiceMock;
        private readonly RestSecretEventHandler restSecretEventHandler;

        public RestSecretEventHandlerTests()
        {
            this.restServiceMock = new Mock<IRestService>();

            this.restSecretEventHandler =
                new RestSecretEventHandler(
                    restService: this.restServiceMock.Object);
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

        private static IReadOnlyDictionary<string, string> CreateRandomHandlerParams() =>
            new Dictionary<string, string>
            {
                { "Url", GetRandomString() },
                { "Secret", GetRandomString() }
            };

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);
    }
}
