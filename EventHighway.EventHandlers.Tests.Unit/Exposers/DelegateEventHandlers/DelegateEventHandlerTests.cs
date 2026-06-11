// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.EventHandlers.Models.Foundations.Delegates.Exceptions;
using EventHighway.EventHandlers.Services.Delegates;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.EventHandlers.Tests.Unit.Exposers.DelegateEventHandlers
{
    public partial class DelegateEventHandlerTests
    {
        private readonly Mock<IDelegateService> delegateServiceMock;
        private readonly DelegateEventHandler delegateEventHandler;

        public DelegateEventHandlerTests()
        {
            this.delegateServiceMock = new Mock<IDelegateService>();

            this.delegateEventHandler =
                new DelegateEventHandler(
                    delegateService: this.delegateServiceMock.Object);
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
                { GetRandomString(), GetRandomString() }
            };

        public static TheoryData<Xeption> DelegateValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new DelegateServiceValidationException(someMessage, someInnerException),
            };
        }

        public static TheoryData<Xeption> DelegateDependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            return new TheoryData<Xeption>
            {
                new DelegateServiceException(someMessage, someInnerException),
            };
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);
    }
}
