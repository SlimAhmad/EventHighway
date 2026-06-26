// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Processings.EventAddresses.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventAddresses.V2
{
    public partial class EventAddressV2ProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnRetrieveByIdIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventAddressV2Id = GetRandomId();

            var expectedEventAddressV2ProcessingDependencyValidationException =
                new EventAddressV2ProcessingDependencyValidationException(
                    message: "Event address validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(),
                    randomCancellationToken))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<EventAddressV2> retrieveEventAddressV2ByIdTask =
                this.eventAddressV2ProcessingService.RetrieveEventAddressV2ByIdAsync(
                    someEventAddressV2Id,
                    randomCancellationToken);

            EventAddressV2ProcessingDependencyValidationException
                actualEventAddressV2ProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<EventAddressV2ProcessingDependencyValidationException>(
                        retrieveEventAddressV2ByIdTask.AsTask);

            // then
            actualEventAddressV2ProcessingDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventAddressV2ProcessingDependencyValidationException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(),
                    randomCancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV2ProcessingDependencyValidationException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveByIdIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventAddressV2Id = GetRandomId();

            var expectedEventAddressV2ProcessingDependencyException =
                new EventAddressV2ProcessingDependencyException(
                    message: "Event address dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(),
                    randomCancellationToken))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<EventAddressV2> retrieveEventAddressV2ByIdTask =
                this.eventAddressV2ProcessingService.RetrieveEventAddressV2ByIdAsync(
                    someEventAddressV2Id,
                    randomCancellationToken);

            EventAddressV2ProcessingDependencyException
                actualEventAddressV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<EventAddressV2ProcessingDependencyException>(
                        retrieveEventAddressV2ByIdTask.AsTask);

            // then
            actualEventAddressV2ProcessingDependencyException.Should()
                .BeEquivalentTo(expectedEventAddressV2ProcessingDependencyException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(),
                    randomCancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV2ProcessingDependencyException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventAddressV2Id = GetRandomId();
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventAddressV2ProcessingServiceException =
                new FailedEventAddressV2ProcessingServiceException(
                    message: "Failed event address service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventAddressV2ProcessingExceptionException =
                new EventAddressV2ProcessingServiceException(
                    message: "Event address service error occurred, contact support.",
                    innerException: failedEventAddressV2ProcessingServiceException);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(),
                    randomCancellationToken))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<EventAddressV2> retrieveEventAddressV2ByIdTask =
                this.eventAddressV2ProcessingService.RetrieveEventAddressV2ByIdAsync(
                    someEventAddressV2Id,
                    randomCancellationToken);

            EventAddressV2ProcessingServiceException
                actualEventAddressV2ProcessingServiceException =
                    await Assert.ThrowsAsync<EventAddressV2ProcessingServiceException>(
                        retrieveEventAddressV2ByIdTask.AsTask);

            // then
            actualEventAddressV2ProcessingServiceException.Should()
                .BeEquivalentTo(expectedEventAddressV2ProcessingExceptionException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(),
                    randomCancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV2ProcessingExceptionException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveByIdIfTimeoutOccursAndLogItAsync()
        {
            // given
            Guid someEventAddressV2Id = GetRandomId();
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutEventAddressV2ProcessingException =
                new TimeoutEventAddressV2ProcessingException(
                    message: "Failed event address processing timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedEventAddressV2ProcessingDependencyException =
                new EventAddressV2ProcessingDependencyException(
                    message: "Event address dependency error occurred, contact support.",
                    innerException: timeoutEventAddressV2ProcessingException);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<EventAddressV2> retrieveEventAddressV2ByIdTask =
                this.eventAddressV2ProcessingService.RetrieveEventAddressV2ByIdAsync(
                    someEventAddressV2Id,
                    TestContext.Current.CancellationToken);

            EventAddressV2ProcessingDependencyException
                actualEventAddressV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<EventAddressV2ProcessingDependencyException>(
                        retrieveEventAddressV2ByIdTask.AsTask);

            // then
            actualEventAddressV2ProcessingDependencyException.Should().BeEquivalentTo(
                expectedEventAddressV2ProcessingDependencyException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV2ProcessingDependencyException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveByIdAsync()
        {
            // given
            Guid someEventAddressV2Id = GetRandomId();
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<EventAddressV2> retrieveEventAddressV2ByIdTask =
                this.eventAddressV2ProcessingService.RetrieveEventAddressV2ByIdAsync(
                    someEventAddressV2Id,
                    cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveEventAddressV2ByIdTask.AsTask);

            actualException.Should().NotBeOfType<EventAddressV2ProcessingDependencyException>();
            actualException.Should().NotBeOfType<EventAddressV2ProcessingServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
