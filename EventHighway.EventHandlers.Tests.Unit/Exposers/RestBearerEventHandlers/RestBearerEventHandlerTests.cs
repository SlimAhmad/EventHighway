// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.EventHandlers.Models.Foundations.Rest.Exceptions;
using EventHighway.EventHandlers.Services.Rest;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.EventHandlers.Tests.Unit.Exposers.RestBearerEventHandlers
{
    public partial class RestBearerEventHandlerTests
    {
        private readonly Mock<IRestService> restServiceMock;
        private readonly RestBearerEventHandler restBearerEventHandler;

        public RestBearerEventHandlerTests()
        {
            this.restServiceMock = new Mock<IRestService>();
            Guid identifier = Guid.NewGuid();

            this.restBearerEventHandler =
                new RestBearerEventHandler(
                    id: identifier,
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
                { "ClientId", GetRandomString() },
                { "ClientSecret", GetRandomString() },
                { "Scope", GetRandomString() },
                { "GrantType", GetRandomString() },
                { "TokenUrl", GetRandomString() }
            };

        public static TheoryData<Xeption> RestBearerValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new RestServiceValidationException(someMessage, someInnerException),
                new RestServiceDependencyValidationException(someMessage, someInnerException),
            };
        }

        public static TheoryData<Xeption> RestBearerDependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new RestServiceDependencyException(someMessage, someInnerException),
                new RestServiceException(someMessage, someInnerException),
            };
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);
    }
}
