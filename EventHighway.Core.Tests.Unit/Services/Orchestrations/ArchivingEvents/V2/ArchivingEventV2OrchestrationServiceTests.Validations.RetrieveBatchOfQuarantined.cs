// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Configurations.LoopDetections;
using EventHighway.Core.Models.Orchestrations.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ArchivingEvents.V2
{
    public partial class ArchivingEventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveBatchOfQuarantinedIfTakeIsInvalidAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var invalidBatchConfiguration = new BatchConfiguration
            {
                BatchSizeForBulkProcessing = -1 * GetRandomNumber()
            };

            var invalidArchivingEventV2OrchestrationException =
                new InvalidArchivingEventV2OrchestrationException(
                    message: "Event is invalid, fix the errors and try again.");

            invalidArchivingEventV2OrchestrationException.AddData(
                key: nameof(BatchConfiguration.BatchSizeForBulkProcessing),
                values: "Value must be greater than or equal to 0");

            var expectedArchivingEventV2OrchestrationValidationException =
                new ArchivingEventV2OrchestrationValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: invalidArchivingEventV2OrchestrationException);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(invalidBatchConfiguration);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetLoopDetectionConfiguration())
                    .Returns(new LoopDetection());

            // when
            ValueTask<IEnumerable<EventV2>> retrieveBatchOfQuarantinedEventV2sTask =
                this.archivingEventV2OrchestrationService
                    .RetrieveBatchOfQuarantinedEventV2sAsync(
                        randomCancellationToken);

            ArchivingEventV2OrchestrationValidationException
                actualArchivingEventV2OrchestrationValidationException =
                    await Assert.ThrowsAsync<ArchivingEventV2OrchestrationValidationException>(
                        retrieveBatchOfQuarantinedEventV2sTask.AsTask);

            // then
            actualArchivingEventV2OrchestrationValidationException.Should()
                .BeEquivalentTo(expectedArchivingEventV2OrchestrationValidationException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetLoopDetectionConfiguration(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2OrchestrationValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveBatchOfQuarantinedIfLoopDetectionIsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            BatchConfiguration randomBatchConfiguration = CreateRandomBatchConfiguration();
            LoopDetection nullLoopDetection = null;

            var invalidArchivingEventV2OrchestrationException =
                new InvalidArchivingEventV2OrchestrationException(
                    message: "Event is invalid, fix the errors and try again.");

            invalidArchivingEventV2OrchestrationException.AddData(
                key: nameof(LoopDetection),
                values: "Value is required");

            var expectedArchivingEventV2OrchestrationValidationException =
                new ArchivingEventV2OrchestrationValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: invalidArchivingEventV2OrchestrationException);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(randomBatchConfiguration);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetLoopDetectionConfiguration())
                    .Returns(nullLoopDetection);

            // when
            ValueTask<IEnumerable<EventV2>> retrieveBatchOfQuarantinedEventV2sTask =
                this.archivingEventV2OrchestrationService
                    .RetrieveBatchOfQuarantinedEventV2sAsync(
                        randomCancellationToken);

            ArchivingEventV2OrchestrationValidationException
                actualArchivingEventV2OrchestrationValidationException =
                    await Assert.ThrowsAsync<ArchivingEventV2OrchestrationValidationException>(
                        retrieveBatchOfQuarantinedEventV2sTask.AsTask);

            // then
            actualArchivingEventV2OrchestrationValidationException.Should()
                .BeEquivalentTo(expectedArchivingEventV2OrchestrationValidationException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetLoopDetectionConfiguration(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2OrchestrationValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
