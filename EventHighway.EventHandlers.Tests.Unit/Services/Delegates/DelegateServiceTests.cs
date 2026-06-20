// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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
        private readonly Mock<Func<string, CancellationToken, ValueTask<EventHandlerResult>>> handlerFuncMock;
        private readonly Mock<DelegateService> delegateServiceMock;
        private readonly IDelegateService delegateService;

        public DelegateServiceTests()
        {
            this.handlerFuncMock =
                new Mock<Func<string, CancellationToken, ValueTask<EventHandlerResult>>>();

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

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);
    }
}
