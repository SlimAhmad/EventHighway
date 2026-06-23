// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Processings.EventArchives.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventArchives.V2
{
    public partial class EventArchiveV2ProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnRetrieveBatchOlderThanIfValidationErrorOccursAndLogItAsync(
            Xeption eventArchiveV2ValidationException)
        {
            // given
            DateTimeOffset someOlderThan = GetRandomDateTimeOffset();
            int someTake = 0;

            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedEventArchiveV2ProcessingDependencyValidationException =
                new EventArchiveV2ProcessingDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: eventArchiveV2ValidationException.InnerException as Xeption);

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(eventArchiveV2ValidationException);

            // when
            ValueTask<IEnumerable<EventArchiveV2>> retrieveBatchOlderThanTask =
                this.eventArchiveV2ProcessingService
                    .RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                        someOlderThan,
                        someTake,
                        randomCancellationToken);

            EventArchiveV2ProcessingDependencyValidationException
                actualEventArchiveV2ProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<EventArchiveV2ProcessingDependencyValidationException>(
                        retrieveBatchOlderThanTask.AsTask);

            // then
            actualEventArchiveV2ProcessingDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventArchiveV2ProcessingDependencyValidationException);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2ProcessingDependencyValidationException))),
                        Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveBatchOlderThanIfDependencyErrorOccursAndLogItAsync(
            Xeption eventArchiveV2DependencyException)
        {
            // given
            DateTimeOffset someOlderThan = GetRandomDateTimeOffset();
            int someTake = 0;

            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedEventArchiveV2ProcessingDependencyException =
                new EventArchiveV2ProcessingDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: eventArchiveV2DependencyException.InnerException as Xeption);

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(eventArchiveV2DependencyException);

            // when
            ValueTask<IEnumerable<EventArchiveV2>> retrieveBatchOlderThanTask =
                this.eventArchiveV2ProcessingService
                    .RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                        someOlderThan,
                        someTake,
                        randomCancellationToken);

            EventArchiveV2ProcessingDependencyException
                actualEventArchiveV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<EventArchiveV2ProcessingDependencyException>(
                        retrieveBatchOlderThanTask.AsTask);

            // then
            actualEventArchiveV2ProcessingDependencyException.Should()
                .BeEquivalentTo(expectedEventArchiveV2ProcessingDependencyException);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2ProcessingDependencyException))),
                        Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveBatchOlderThanIfExceptionOccursAndLogItAsync()
        {
            // given
            DateTimeOffset someOlderThan = GetRandomDateTimeOffset();
            int someTake = 0;

            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var serviceException = new Exception();

            var failedEventArchiveV2ProcessingServiceException =
                new FailedEventArchiveV2ProcessingServiceException(
                    message: "Failed event archive service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventArchiveV2ProcessingServiceException =
                new EventArchiveV2ProcessingServiceException(
                    message: "Event archive service error occurred, contact support.",
                    innerException: failedEventArchiveV2ProcessingServiceException);

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IEnumerable<EventArchiveV2>> retrieveBatchOlderThanTask =
                this.eventArchiveV2ProcessingService
                    .RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                        someOlderThan,
                        someTake,
                        randomCancellationToken);

            EventArchiveV2ProcessingServiceException
                actualEventArchiveV2ProcessingServiceException =
                    await Assert.ThrowsAsync<EventArchiveV2ProcessingServiceException>(
                        retrieveBatchOlderThanTask.AsTask);

            // then
            actualEventArchiveV2ProcessingServiceException.Should()
                .BeEquivalentTo(expectedEventArchiveV2ProcessingServiceException);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2ProcessingServiceException))),
                        Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveBatchOlderThanIfTimeoutOccursAndLogItAsync()
        {
            // given
            DateTimeOffset someOlderThan = GetRandomDateTimeOffset();
            int someTake = 0;

            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutEventArchiveV2ProcessingException =
                new TimeoutEventArchiveV2ProcessingException(
                    message: "Failed event archive processing timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedEventArchiveV2ProcessingDependencyException =
                new EventArchiveV2ProcessingDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: timeoutEventArchiveV2ProcessingException);

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IEnumerable<EventArchiveV2>> retrieveBatchOlderThanTask =
                this.eventArchiveV2ProcessingService
                    .RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                        someOlderThan,
                        someTake,
                        TestContext.Current.CancellationToken);

            EventArchiveV2ProcessingDependencyException
                actualEventArchiveV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<EventArchiveV2ProcessingDependencyException>(
                        retrieveBatchOlderThanTask.AsTask);

            // then
            actualEventArchiveV2ProcessingDependencyException.Should()
                .BeEquivalentTo(expectedEventArchiveV2ProcessingDependencyException);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2ProcessingDependencyException))),
                        Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveBatchOlderThanAsync()
        {
            // given
            DateTimeOffset someOlderThan = GetRandomDateTimeOffset();
            int someTake = 0;

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<IEnumerable<EventArchiveV2>> retrieveBatchOlderThanTask =
                this.eventArchiveV2ProcessingService
                    .RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                        someOlderThan,
                        someTake,
                        cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveBatchOlderThanTask.AsTask);

            actualException.Should().NotBeOfType<EventArchiveV2ProcessingDependencyException>();
            actualException.Should().NotBeOfType<EventArchiveV2ProcessingServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
