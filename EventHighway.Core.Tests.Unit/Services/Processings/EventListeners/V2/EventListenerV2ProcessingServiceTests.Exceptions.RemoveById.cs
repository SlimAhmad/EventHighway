// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Processings.EventListeners.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventListeners.V2
{
    public partial class EventListenerV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveByIdIfTimeoutOccursAndLogItAsync()
        {
            // given
            Guid someEventListenerV2Id = GetRandomId();
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutEventListenerV2ProcessingException =
                new TimeoutEventListenerV2ProcessingException(
                    message: "Failed event listener processing timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedEventListenerV2ProcessingDependencyException =
                new EventListenerV2ProcessingDependencyException(
                    message: "Event listener dependency error occurred, contact support.",
                    innerException: timeoutEventListenerV2ProcessingException);

            this.eventListenerV2ServiceMock.Setup(service =>
                service.RemoveEventListenerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<EventListenerV2> removeEventListenerV2ByIdTask =
                this.eventListenerV2ProcessingService.RemoveEventListenerV2ByIdAsync(
                    someEventListenerV2Id,
                    TestContext.Current.CancellationToken);

            EventListenerV2ProcessingDependencyException actualEventListenerV2ProcessingDependencyException =
                await Assert.ThrowsAsync<EventListenerV2ProcessingDependencyException>(
                    removeEventListenerV2ByIdTask.AsTask);

            // then
            actualEventListenerV2ProcessingDependencyException.Should().BeEquivalentTo(
                expectedEventListenerV2ProcessingDependencyException);

            this.eventListenerV2ServiceMock.Verify(service =>
                service.RemoveEventListenerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2ProcessingDependencyException))),
                        Times.Once);

            this.eventListenerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRemoveByIdAsync()
        {
            // given
            Guid someEventListenerV2Id = GetRandomId();
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<EventListenerV2> removeEventListenerV2ByIdTask =
                this.eventListenerV2ProcessingService.RemoveEventListenerV2ByIdAsync(
                    someEventListenerV2Id,
                    cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    removeEventListenerV2ByIdTask.AsTask);

            actualException.Should().NotBeOfType<EventListenerV2ProcessingDependencyException>();
            actualException.Should().NotBeOfType<EventListenerV2ProcessingServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.eventListenerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnRemoveByIdIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventListenerV2Id = GetRandomId();

            var expectedEventListenerV2ProcessingDependencyValidationException =
                new EventListenerV2ProcessingDependencyValidationException(
                    message: "Event listener validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventListenerV2ServiceMock.Setup(service =>
                service.RemoveEventListenerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<EventListenerV2> removeEventListenerV2ByIdTask =
                this.eventListenerV2ProcessingService.RemoveEventListenerV2ByIdAsync(
                    someEventListenerV2Id,
                    randomCancellationToken);

            EventListenerV2ProcessingDependencyValidationException
                actualEventListenerV2ProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<EventListenerV2ProcessingDependencyValidationException>(
                        removeEventListenerV2ByIdTask.AsTask);

            // then
            actualEventListenerV2ProcessingDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventListenerV2ProcessingDependencyValidationException);

            this.eventListenerV2ServiceMock.Verify(service =>
                service.RemoveEventListenerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2ProcessingDependencyValidationException))),
                        Times.Once);

            this.eventListenerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRemoveByIdIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventListenerV2Id = GetRandomId();

            var expectedEventListenerV2ProcessingDependencyException =
                new EventListenerV2ProcessingDependencyException(
                    message: "Event listener dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventListenerV2ServiceMock.Setup(service =>
                service.RemoveEventListenerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<EventListenerV2> removeEventListenerV2ByIdTask =
                this.eventListenerV2ProcessingService.RemoveEventListenerV2ByIdAsync(
                    someEventListenerV2Id,
                    randomCancellationToken);

            EventListenerV2ProcessingDependencyException
                actualEventListenerV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<EventListenerV2ProcessingDependencyException>(
                        removeEventListenerV2ByIdTask.AsTask);

            // then
            actualEventListenerV2ProcessingDependencyException.Should()
                .BeEquivalentTo(expectedEventListenerV2ProcessingDependencyException);

            this.eventListenerV2ServiceMock.Verify(service =>
                service.RemoveEventListenerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2ProcessingDependencyException))),
                        Times.Once);

            this.eventListenerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveByIdIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventListenerV2Id = GetRandomId();
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventListenerV2ProcessingServiceException =
                new FailedEventListenerV2ProcessingServiceException(
                    message: "Failed event listener service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventListenerV2ProcessingExceptionException =
                new EventListenerV2ProcessingServiceException(
                    message: "Event listener service error occurred, contact support.",
                    innerException: failedEventListenerV2ProcessingServiceException);

            this.eventListenerV2ServiceMock.Setup(service =>
                service.RemoveEventListenerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<EventListenerV2> removeEventListenerV2ByIdTask =
                this.eventListenerV2ProcessingService.RemoveEventListenerV2ByIdAsync(
                    someEventListenerV2Id,
                    randomCancellationToken);

            EventListenerV2ProcessingServiceException
                actualEventListenerV2ProcessingServiceException =
                    await Assert.ThrowsAsync<EventListenerV2ProcessingServiceException>(
                        removeEventListenerV2ByIdTask.AsTask);

            // then
            actualEventListenerV2ProcessingServiceException.Should()
                .BeEquivalentTo(expectedEventListenerV2ProcessingExceptionException);

            this.eventListenerV2ServiceMock.Verify(service =>
                service.RemoveEventListenerV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2ProcessingExceptionException))),
                        Times.Once);

            this.eventListenerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
