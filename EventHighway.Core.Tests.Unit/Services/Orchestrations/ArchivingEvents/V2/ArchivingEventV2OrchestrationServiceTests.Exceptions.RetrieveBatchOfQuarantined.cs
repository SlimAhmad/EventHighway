// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Configurations.LoopDetections;
using EventHighway.Core.Models.Orchestrations.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ArchivingEvents.V2
{
    public partial class ArchivingEventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveBatchOfQuarantinedIfTimeoutOccursAndLogItAsync()
        {
            // given
            BatchConfiguration randomBatchConfiguration = CreateRandomBatchConfiguration();
            DateTimeOffset retrievedDateTimeOffset = GetRandomDateTimeOffset();
            var loopDetection = new LoopDetection { Window = TimeSpan.FromDays(30) };
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutArchivingEventV2OrchestrationException =
                new TimeoutArchivingEventV2OrchestrationException(
                    message: "Failed archiving event orchestration timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedArchivingEventV2OrchestrationDependencyException =
                new ArchivingEventV2OrchestrationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: timeoutArchivingEventV2OrchestrationException);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(randomBatchConfiguration);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetLoopDetectionConfiguration())
                    .Returns(loopDetection);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(retrievedDateTimeOffset);

            this.eventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IEnumerable<EventV2>> retrieveBatchOfQuarantinedEventV2sTask =
                this.archivingEventV2OrchestrationService
                    .RetrieveBatchOfQuarantinedEventV2sAsync(TestContext.Current.CancellationToken);

            ArchivingEventV2OrchestrationDependencyException
                actualArchivingEventV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<ArchivingEventV2OrchestrationDependencyException>(
                        retrieveBatchOfQuarantinedEventV2sTask.AsTask);

            // then
            actualArchivingEventV2OrchestrationDependencyException.Should().BeEquivalentTo(
                expectedArchivingEventV2OrchestrationDependencyException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetLoopDetectionConfiguration(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2OrchestrationDependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveBatchOfQuarantinedAsync()
        {
            // given
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<IEnumerable<EventV2>> retrieveBatchOfQuarantinedEventV2sTask =
                this.archivingEventV2OrchestrationService
                    .RetrieveBatchOfQuarantinedEventV2sAsync(cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveBatchOfQuarantinedEventV2sTask.AsTask);

            actualException.Should().NotBeOfType<ArchivingEventV2OrchestrationDependencyException>();
            actualException.Should().NotBeOfType<ArchivingEventV2OrchestrationServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task
            ShouldThrowDependencyValidationExceptionOnRetrieveBatchOfQuarantinedIfValidationErrorOccursAndLogItAsync(
                Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            BatchConfiguration randomBatchConfiguration = CreateRandomBatchConfiguration();
            DateTimeOffset retrievedDateTimeOffset = GetRandomDateTimeOffset();
            var loopDetection = new LoopDetection { Window = TimeSpan.FromDays(30) };

            var expectedArchivingEventV2OrchestrationDependencyValidationException =
                new ArchivingEventV2OrchestrationDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(randomBatchConfiguration);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetLoopDetectionConfiguration())
                    .Returns(loopDetection);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(retrievedDateTimeOffset);

            this.eventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask<IEnumerable<EventV2>> retrieveBatchOfQuarantinedEventV2sTask =
                this.archivingEventV2OrchestrationService
                    .RetrieveBatchOfQuarantinedEventV2sAsync(randomCancellationToken);

            ArchivingEventV2OrchestrationDependencyValidationException
                actualArchivingEventV2OrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<ArchivingEventV2OrchestrationDependencyValidationException>(
                        retrieveBatchOfQuarantinedEventV2sTask.AsTask);

            // then
            actualArchivingEventV2OrchestrationDependencyValidationException.Should()
                .BeEquivalentTo(expectedArchivingEventV2OrchestrationDependencyValidationException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetLoopDetectionConfiguration(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2OrchestrationDependencyValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveBatchOfQuarantinedIfDependencyErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            BatchConfiguration randomBatchConfiguration = CreateRandomBatchConfiguration();
            DateTimeOffset retrievedDateTimeOffset = GetRandomDateTimeOffset();
            var loopDetection = new LoopDetection { Window = TimeSpan.FromDays(30) };

            var expectedArchivingEventV2OrchestrationDependencyException =
                new ArchivingEventV2OrchestrationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(randomBatchConfiguration);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetLoopDetectionConfiguration())
                    .Returns(loopDetection);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(retrievedDateTimeOffset);

            this.eventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<IEnumerable<EventV2>> retrieveBatchOfQuarantinedEventV2sTask =
                this.archivingEventV2OrchestrationService
                    .RetrieveBatchOfQuarantinedEventV2sAsync(randomCancellationToken);

            ArchivingEventV2OrchestrationDependencyException
                actualArchivingEventV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<ArchivingEventV2OrchestrationDependencyException>(
                        retrieveBatchOfQuarantinedEventV2sTask.AsTask);

            // then
            actualArchivingEventV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedArchivingEventV2OrchestrationDependencyException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetLoopDetectionConfiguration(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2OrchestrationDependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveBatchOfQuarantinedIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            BatchConfiguration randomBatchConfiguration = CreateRandomBatchConfiguration();
            DateTimeOffset retrievedDateTimeOffset = GetRandomDateTimeOffset();
            var loopDetection = new LoopDetection { Window = TimeSpan.FromDays(30) };
            var exception = new Exception();
            exception.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedArchivingEventV2OrchestrationServiceException =
                new FailedArchivingEventV2OrchestrationServiceException(
                    message: "Failed event service error occurred, contact support.",
                    innerException: exception,
                    data: exception.Data);

            var expectedArchivingEventV2OrchestrationServiceException =
                new ArchivingEventV2OrchestrationServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: failedArchivingEventV2OrchestrationServiceException);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(randomBatchConfiguration);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetLoopDetectionConfiguration())
                    .Returns(loopDetection);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(retrievedDateTimeOffset);

            this.eventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(exception);

            // when
            ValueTask<IEnumerable<EventV2>> retrieveBatchOfQuarantinedEventV2sTask =
                this.archivingEventV2OrchestrationService
                    .RetrieveBatchOfQuarantinedEventV2sAsync(randomCancellationToken);

            ArchivingEventV2OrchestrationServiceException
                actualArchivingEventV2OrchestrationServiceException =
                    await Assert.ThrowsAsync<ArchivingEventV2OrchestrationServiceException>(
                        retrieveBatchOfQuarantinedEventV2sTask.AsTask);

            // then
            actualArchivingEventV2OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedArchivingEventV2OrchestrationServiceException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetLoopDetectionConfiguration(),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2OrchestrationServiceException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
