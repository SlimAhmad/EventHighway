// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Processings.Traffics.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.Traffics.V2
{
    public partial class TrafficV2ProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveTrafficSnapshotIfDependencyExceptionOccursAndLogItAsync(
            Xeption eventV2DependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var someWindowStart = new DateTimeOffset(2026, 6, 24, 0, 0, 0, TimeSpan.Zero);

            var expectedTrafficV2ProcessingDependencyException =
                new TrafficV2ProcessingDependencyException(
                    message: "Traffic dependency error occurred, contact support.",
                    innerException: eventV2DependencyException.InnerException as Xeption);

            this.eventV2ServiceMock.Setup(service =>
                service.RetrieveAllEventV2sWithListenerEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(eventV2DependencyException);

            // when
            ValueTask<TrafficSnapshotV2> retrieveTrafficSnapshotTask =
                this.trafficV2ProcessingService.RetrieveTrafficSnapshotV2Async(
                    TrafficPeriodV2.Day, someWindowStart, randomCancellationToken);

            TrafficV2ProcessingDependencyException
                actualTrafficV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<TrafficV2ProcessingDependencyException>(
                        retrieveTrafficSnapshotTask.AsTask);

            // then
            actualTrafficV2ProcessingDependencyException.Should()
                .BeEquivalentTo(expectedTrafficV2ProcessingDependencyException);

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveAllEventV2sWithListenerEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedTrafficV2ProcessingDependencyException))),
                        Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveTrafficSnapshotIfTimeoutOccursAndLogItAsync()
        {
            // given
            var someWindowStart = new DateTimeOffset(2026, 6, 24, 0, 0, 0, TimeSpan.Zero);
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.", operationCanceledException);

            var timeoutTrafficV2ProcessingException =
                new TimeoutTrafficV2ProcessingException(
                    message: "Failed traffic processing timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: operationCanceledException.Data);

            var expectedTrafficV2ProcessingDependencyException =
                new TrafficV2ProcessingDependencyException(
                    message: "Traffic dependency error occurred, contact support.",
                    innerException: timeoutTrafficV2ProcessingException);

            this.eventV2ServiceMock.Setup(service =>
                service.RetrieveAllEventV2sWithListenerEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<TrafficSnapshotV2> retrieveTrafficSnapshotTask =
                this.trafficV2ProcessingService.RetrieveTrafficSnapshotV2Async(
                    TrafficPeriodV2.Day, someWindowStart, TestContext.Current.CancellationToken);

            TrafficV2ProcessingDependencyException
                actualTrafficV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<TrafficV2ProcessingDependencyException>(
                        retrieveTrafficSnapshotTask.AsTask);

            // then
            actualTrafficV2ProcessingDependencyException.Should()
                .BeEquivalentTo(expectedTrafficV2ProcessingDependencyException);

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveAllEventV2sWithListenerEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedTrafficV2ProcessingDependencyException))),
                        Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveTrafficSnapshotIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var someWindowStart = new DateTimeOffset(2026, 6, 24, 0, 0, 0, TimeSpan.Zero);
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedTrafficV2ProcessingServiceException =
                new FailedTrafficV2ProcessingServiceException(
                    message: "Failed traffic service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedTrafficV2ProcessingServiceException =
                new TrafficV2ProcessingServiceException(
                    message: "Traffic service error occurred, contact support.",
                    innerException: failedTrafficV2ProcessingServiceException);

            this.eventV2ServiceMock.Setup(service =>
                service.RetrieveAllEventV2sWithListenerEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<TrafficSnapshotV2> retrieveTrafficSnapshotTask =
                this.trafficV2ProcessingService.RetrieveTrafficSnapshotV2Async(
                    TrafficPeriodV2.Day, someWindowStart, randomCancellationToken);

            TrafficV2ProcessingServiceException
                actualTrafficV2ProcessingServiceException =
                    await Assert.ThrowsAsync<TrafficV2ProcessingServiceException>(
                        retrieveTrafficSnapshotTask.AsTask);

            // then
            actualTrafficV2ProcessingServiceException.Should()
                .BeEquivalentTo(expectedTrafficV2ProcessingServiceException);

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveAllEventV2sWithListenerEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedTrafficV2ProcessingServiceException))),
                        Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveTrafficSnapshotAsync()
        {
            // given
            var someWindowStart = new DateTimeOffset(2026, 6, 24, 0, 0, 0, TimeSpan.Zero);

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<TrafficSnapshotV2> retrieveTrafficSnapshotTask =
                this.trafficV2ProcessingService.RetrieveTrafficSnapshotV2Async(
                    TrafficPeriodV2.Day, someWindowStart, cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveTrafficSnapshotTask.AsTask);

            actualException.Should().NotBeOfType<TrafficV2ProcessingDependencyException>();
            actualException.Should().NotBeOfType<TrafficV2ProcessingServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
