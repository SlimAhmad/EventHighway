// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.EventHandlers.Services.Delegates;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.EventHandlers.Tests.Unit.Services.Delegates
{
    public partial class DelegateServiceTests
    {
        private readonly Mock<Func<string, IReadOnlyDictionary<string, string>, CancellationToken, ValueTask<EventHandlerResult>>> handlerFuncMock;
        private readonly Mock<DelegateService> delegateServiceMock;
        private readonly IDelegateService delegateService;

        public DelegateServiceTests()
        {
            this.handlerFuncMock =
                new Mock<Func<string, IReadOnlyDictionary<string, string>, CancellationToken, ValueTask<EventHandlerResult>>>();

            this.delegateServiceMock =
                new Mock<DelegateService>(this.handlerFuncMock.Object) { CallBase = true };

            this.delegateService = this.delegateServiceMock.Object;
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

        private static IReadOnlyDictionary<string, string> CreateHandlerParams() =>
            new Dictionary<string, string>
            {
                { GetRandomString(), GetRandomString() }
            };

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);
    }
}
