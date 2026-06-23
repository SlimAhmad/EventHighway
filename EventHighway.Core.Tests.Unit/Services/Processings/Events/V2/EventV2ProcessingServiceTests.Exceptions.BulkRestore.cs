// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Processings.Events.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.Events.V2
{
    public partial class EventV2ProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnBulkRestoreIfValidationExceptionOccursAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEnumerable<EventV2> someEventV2s = CreateRandomEventV2s();

            var expectedException =
                new EventV2ProcessingDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            this.eventV2ServiceMock.Setup(service =>
                service.BulkRestoreEventV2sAsync(
                    It.IsAny<IEnumerable<EventV2>>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask<IEnumerable<EventV2>> bulkRestoreEventV2sTask =
                this.eventV2ProcessingService.BulkRestoreEventV2sAsync(
                    someEventV2s,
                    randomCancellationToken);

            EventV2ProcessingDependencyValidationException actualException =
                await Assert.ThrowsAsync<EventV2ProcessingDependencyValidationException>(
                    bulkRestoreEventV2sTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.eventV2ServiceMock.Verify(service =>
                service.BulkRestoreEventV2sAsync(
                    It.IsAny<IEnumerable<EventV2>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnBulkRestoreIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEnumerable<EventV2> someEventV2s = CreateRandomEventV2s();

            var expectedException =
                new EventV2ProcessingDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventV2ServiceMock.Setup(service =>
                service.BulkRestoreEventV2sAsync(
                    It.IsAny<IEnumerable<EventV2>>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<IEnumerable<EventV2>> bulkRestoreEventV2sTask =
                this.eventV2ProcessingService.BulkRestoreEventV2sAsync(
                    someEventV2s,
                    randomCancellationToken);

            EventV2ProcessingDependencyException actualException =
                await Assert.ThrowsAsync<EventV2ProcessingDependencyException>(
                    bulkRestoreEventV2sTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.eventV2ServiceMock.Verify(service =>
                service.BulkRestoreEventV2sAsync(
                    It.IsAny<IEnumerable<EventV2>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnBulkRestoreIfTimeoutOccursAndLogItAsync()
        {
            // given
            IEnumerable<EventV2> someEventV2s = CreateRandomEventV2s();
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutEventV2ProcessingException =
                new TimeoutEventV2ProcessingException(
                    message: "Failed event processing timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedException =
                new EventV2ProcessingDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: timeoutEventV2ProcessingException);

            this.eventV2ServiceMock.Setup(service =>
                service.BulkRestoreEventV2sAsync(
                    It.IsAny<IEnumerable<EventV2>>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IEnumerable<EventV2>> bulkRestoreEventV2sTask =
                this.eventV2ProcessingService.BulkRestoreEventV2sAsync(
                    someEventV2s, TestContext.Current.CancellationToken);

            EventV2ProcessingDependencyException actualException =
                await Assert.ThrowsAsync<EventV2ProcessingDependencyException>(
                    bulkRestoreEventV2sTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.eventV2ServiceMock.Verify(service =>
                service.BulkRestoreEventV2sAsync(
                    It.IsAny<IEnumerable<EventV2>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnBulkRestoreIfServiceErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEnumerable<EventV2> someEventV2s = CreateRandomEventV2s();
            var serviceException = new Exception();

            var failedEventV2ProcessingServiceException =
                new FailedEventV2ProcessingServiceException(
                    message: "Failed event service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedException =
                new EventV2ProcessingServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: failedEventV2ProcessingServiceException);

            this.eventV2ServiceMock.Setup(service =>
                service.BulkRestoreEventV2sAsync(
                    It.IsAny<IEnumerable<EventV2>>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<IEnumerable<EventV2>> bulkRestoreEventV2sTask =
                this.eventV2ProcessingService.BulkRestoreEventV2sAsync(
                    someEventV2s,
                    randomCancellationToken);

            EventV2ProcessingServiceException actualException =
                await Assert.ThrowsAsync<EventV2ProcessingServiceException>(
                    bulkRestoreEventV2sTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.eventV2ServiceMock.Verify(service =>
                service.BulkRestoreEventV2sAsync(
                    It.IsAny<IEnumerable<EventV2>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
