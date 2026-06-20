// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Orchestrations.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ArchivingEvents.V2
{
    public partial class ArchivingEventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnBulkRemoveEventV2sIfEventV2sIsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEnumerable<EventV2> nullEventV2s = null;

            var nullArchivingEventV2sOrchestrationException =
                new NullArchivingEventV2sOrchestrationException(
                    message: "Events are null.");

            var expectedArchivingEventV2OrchestrationValidationException =
                new ArchivingEventV2OrchestrationValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: nullArchivingEventV2sOrchestrationException);

            // when
            ValueTask bulkRemoveEventV2sTask =
                this.archivingEventV2OrchestrationService.BulkRemoveEventV2sAsync(
                    nullEventV2s,
                    randomCancellationToken);

            ArchivingEventV2OrchestrationValidationException
                actualArchivingEventV2OrchestrationValidationException =
                    await Assert.ThrowsAsync<ArchivingEventV2OrchestrationValidationException>(
                        bulkRemoveEventV2sTask.AsTask);

            // then
            actualArchivingEventV2OrchestrationValidationException.Should()
                .BeEquivalentTo(expectedArchivingEventV2OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2OrchestrationValidationException))),
                        Times.Once);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.BulkRemoveEventV2sAsync(
                    It.IsAny<IEnumerable<EventV2>>(),
                    It.IsAny<System.Threading.CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
        }
    }
}
