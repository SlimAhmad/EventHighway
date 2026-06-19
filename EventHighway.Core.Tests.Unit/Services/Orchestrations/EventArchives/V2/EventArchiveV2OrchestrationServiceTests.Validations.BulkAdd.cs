// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V2
{
    public partial class EventArchiveV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnBulkAddIfEventArchiveV2sIsNullAndLogItAsync()
        {
            // given
            List<EventArchiveV2> nullEventArchiveV2s = null;

            var nullEventArchiveV2sOrchestrationException =
                new NullEventArchiveV2sOrchestrationException(
                    message: "Event archives are null.");

            var expectedEventArchiveV2OrchestrationValidationException =
                new EventArchiveV2OrchestrationValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: nullEventArchiveV2sOrchestrationException);

            // when
            ValueTask bulkAddEventArchiveV2sTask =
                this.eventArchiveV2OrchestrationService.BulkAddEventArchiveV2sAsync(
                    nullEventArchiveV2s,
                    TestContext.Current.CancellationToken);

            EventArchiveV2OrchestrationValidationException
                actualEventArchiveV2OrchestrationValidationException =
                    await Assert.ThrowsAsync<EventArchiveV2OrchestrationValidationException>(
                        bulkAddEventArchiveV2sTask.AsTask);

            // then
            actualEventArchiveV2OrchestrationValidationException.Should().BeEquivalentTo(
                expectedEventArchiveV2OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2OrchestrationValidationException))),
                        Times.Once);

            this.eventArchiveV2ProcessingServiceMock.Verify(service =>
                service.BulkAddEventArchiveV2sAsync(
                    It.IsAny<IEnumerable<EventArchiveV2>>(),
                    It.IsAny<System.Threading.CancellationToken>()),
                        Times.Never);

            this.listenerEventArchiveV2ProcessingServiceMock.Verify(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventArchiveV2>>(),
                    It.IsAny<System.Threading.CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
