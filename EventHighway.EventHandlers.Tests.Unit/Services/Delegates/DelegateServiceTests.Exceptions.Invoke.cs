// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.EventHandlers.Models.Foundations.Delegates.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.EventHandlers.Tests.Unit.Services.Delegates
{
    public partial class DelegateServiceTests
    {
        [Fact]
        public async Task ShouldThrowServiceExceptionOnInvokeIfExceptionOccursAsync()
        {
            // given
            string randomContent = GetRandomString();
            IReadOnlyDictionary<string, string> inputHandlerParams = CreateHandlerParams();

            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new System.Collections.Generic.List<string> { "ServiceError" });

            var failedDelegateServiceException =
                new FailedDelegateServiceException(
                    message: "Failed delegate service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedDelegateServiceException =
                new DelegateServiceException(
                    message: "Delegate service error occurred, contact support.",
                    innerException: failedDelegateServiceException);

            this.delegateServiceMock
                .Setup(service => service.ValidateInvokeParams(
                    It.IsAny<string>(),
                    It.IsAny<Func<string, IReadOnlyDictionary<string, string>, CancellationToken, ValueTask<EventHandlerResult>>>()))
                .Throws(serviceException);

            // when
            ValueTask<EventHandlerResult> invokeTask =
                this.delegateService.InvokeAsync(
                    content: randomContent,
                    handlerParams: inputHandlerParams,
                    cancellationToken: TestContext.Current.CancellationToken);

            DelegateServiceException actualDelegateServiceException =
                await Assert.ThrowsAsync<DelegateServiceException>(
                    invokeTask.AsTask);

            // then
            actualDelegateServiceException.Should()
                .BeEquivalentTo(expectedDelegateServiceException);

            this.delegateServiceMock.Verify(service =>
                service.ValidateInvokeParams(
                    It.IsAny<string>(),
                    It.IsAny<Func<string, IReadOnlyDictionary<string, string>, CancellationToken, ValueTask<EventHandlerResult>>>()),
                Times.Once);

            this.handlerFuncMock.Verify(handler =>
                handler(
                    It.IsAny<string>(),
                    It.IsAny<IReadOnlyDictionary<string, string>>(),
                    It.IsAny<CancellationToken>()),
                Times.Never);

            this.handlerFuncMock.VerifyNoOtherCalls();
        }
    }
}
