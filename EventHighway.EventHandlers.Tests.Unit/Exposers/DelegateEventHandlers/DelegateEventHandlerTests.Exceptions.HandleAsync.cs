// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.EventHandlers.Models.Exposers.DelegateEventHandlers.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.EventHandlers.Tests.Unit.Exposers.DelegateEventHandlers
{
    public partial class DelegateEventHandlerTests
    {
        [Theory]
        [MemberData(nameof(DelegateValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnHandleIfDependencyValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            string randomContent = GetRandomString();
            var randomHandlerParams = CreateRandomHandlerParams();

            var expectedDelegateEventHandlerValidationException =
                new DelegateEventHandlerValidationException(
                    message: "Delegate event handler validation error occurred, fix errors and try again.",
                    innerException: validationException.InnerException as Xeption,
                    data: (validationException.InnerException as Xeption).Data);

            this.delegateServiceMock.Setup(service =>
                service.InvokeAsync(
                    randomContent,
                    randomHandlerParams,
                    TestContext.Current.CancellationToken))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<EventHandlerResult> handleTask =
                this.delegateEventHandler.HandleAsync(
                    content: randomContent,
                    handlerParams: randomHandlerParams,
                    cancellationToken: TestContext.Current.CancellationToken);

            DelegateEventHandlerValidationException
                actualDelegateEventHandlerValidationException =
                    await Assert.ThrowsAsync<DelegateEventHandlerValidationException>(
                        handleTask.AsTask);

            // then
            actualDelegateEventHandlerValidationException.Should()
                .BeEquivalentTo(expectedDelegateEventHandlerValidationException);

            this.delegateServiceMock.Verify(service =>
                service.InvokeAsync(
                    randomContent,
                    randomHandlerParams,
                    TestContext.Current.CancellationToken),
                        Times.Once);

            this.delegateServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DelegateDependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnHandleIfDependencyErrorOccursAsync(
            Xeption dependencyException)
        {
            // given
            string randomContent = GetRandomString();
            var randomHandlerParams = CreateRandomHandlerParams();

            var expectedDelegateEventHandlerDependencyException =
                new DelegateEventHandlerDependencyException(
                    message: "Delegate event handler dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption,
                    data: (dependencyException.InnerException as Xeption).Data);

            this.delegateServiceMock.Setup(service =>
                service.InvokeAsync(
                    randomContent,
                    randomHandlerParams,
                    TestContext.Current.CancellationToken))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<EventHandlerResult> handleTask =
                this.delegateEventHandler.HandleAsync(
                    content: randomContent,
                    handlerParams: randomHandlerParams,
                    cancellationToken: TestContext.Current.CancellationToken);

            DelegateEventHandlerDependencyException
                actualDelegateEventHandlerDependencyException =
                    await Assert.ThrowsAsync<DelegateEventHandlerDependencyException>(
                        handleTask.AsTask);

            // then
            actualDelegateEventHandlerDependencyException.Should()
                .BeEquivalentTo(expectedDelegateEventHandlerDependencyException);

            this.delegateServiceMock.Verify(service =>
                service.InvokeAsync(
                    randomContent,
                    randomHandlerParams,
                    TestContext.Current.CancellationToken),
                        Times.Once);

            this.delegateServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnHandleIfExceptionOccursAsync()
        {
            // given
            string randomContent = GetRandomString();
            var randomHandlerParams = CreateRandomHandlerParams();

            var someInnerXeption = new Xeption(message: GetRandomString());
            someInnerXeption.Data.Add("ErrorCode", new List<string> { "ServiceError" });
            var serviceException = new Exception(GetRandomString(), someInnerXeption);

            var expectedDelegateEventHandlerServiceException =
                new DelegateEventHandlerServiceException(
                    message: "Delegate event handler service error occurred, contact support.",
                    innerException: someInnerXeption,
                    data: someInnerXeption.Data);

            this.delegateServiceMock.Setup(service =>
                service.InvokeAsync(
                    randomContent,
                    randomHandlerParams,
                    TestContext.Current.CancellationToken))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<EventHandlerResult> handleTask =
                this.delegateEventHandler.HandleAsync(
                    content: randomContent,
                    handlerParams: randomHandlerParams,
                    cancellationToken: TestContext.Current.CancellationToken);

            DelegateEventHandlerServiceException
                actualDelegateEventHandlerServiceException =
                    await Assert.ThrowsAsync<DelegateEventHandlerServiceException>(
                        handleTask.AsTask);

            // then
            actualDelegateEventHandlerServiceException.Should()
                .BeEquivalentTo(expectedDelegateEventHandlerServiceException);

            this.delegateServiceMock.Verify(service =>
                service.InvokeAsync(
                    randomContent,
                    randomHandlerParams,
                    TestContext.Current.CancellationToken),
                        Times.Once);

            this.delegateServiceMock.VerifyNoOtherCalls();
        }
    }
}
