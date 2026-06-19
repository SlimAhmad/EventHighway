// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Orchestrations.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ArchivingEvents.V2
{
    public partial class ArchivingEventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveBatchOfListenerEventV2sIfEventV2IdsIsNullAndLogItAsync()
        {
            // given
            IEnumerable<Guid> nullEventV2Ids = null;
            BatchConfiguration randomBatchConfiguration = CreateRandomBatchConfiguration();

            var invalidArchivingEventV2OrchestrationException =
                new InvalidArchivingEventV2OrchestrationException(
                    message: "Event is invalid, fix the errors and try again.");

            invalidArchivingEventV2OrchestrationException.AddData(
                key: nameof(nullEventV2Ids),
                values: "Value is required");

            var expectedArchivingEventV2OrchestrationValidationException =
                new ArchivingEventV2OrchestrationValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: invalidArchivingEventV2OrchestrationException);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(randomBatchConfiguration);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> retrieveBatchOfListenerEventV2sTask =
                this.archivingEventV2OrchestrationService
                    .RetrieveBatchOfListenerEventV2sAsync(
                        nullEventV2Ids,
                        TestContext.Current.CancellationToken);

            ArchivingEventV2OrchestrationValidationException
                actualArchivingEventV2OrchestrationValidationException =
                    await Assert.ThrowsAsync<ArchivingEventV2OrchestrationValidationException>(
                        retrieveBatchOfListenerEventV2sTask.AsTask);

            // then
            actualArchivingEventV2OrchestrationValidationException.Should()
                .BeEquivalentTo(expectedArchivingEventV2OrchestrationValidationException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2OrchestrationValidationException))),
                        Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveBatchOfListenerEventV2sIfTakeIsInvalidAndLogItAsync()
        {
            // given
            IEnumerable<Guid> someEventV2Ids =
                Enumerable.Range(0, GetRandomNumber()).Select(_ => Guid.NewGuid()).ToList();

            var invalidBatchConfiguration = new BatchConfiguration
            {
                BatchSizeForBulkProcessing = -1
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

            // when
            ValueTask<IEnumerable<ListenerEventV2>> retrieveBatchOfListenerEventV2sTask =
                this.archivingEventV2OrchestrationService
                    .RetrieveBatchOfListenerEventV2sAsync(
                        someEventV2Ids,
                        TestContext.Current.CancellationToken);

            ArchivingEventV2OrchestrationValidationException
                actualArchivingEventV2OrchestrationValidationException =
                    await Assert.ThrowsAsync<ArchivingEventV2OrchestrationValidationException>(
                        retrieveBatchOfListenerEventV2sTask.AsTask);

            // then
            actualArchivingEventV2OrchestrationValidationException.Should()
                .BeEquivalentTo(expectedArchivingEventV2OrchestrationValidationException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2OrchestrationValidationException))),
                        Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
