// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V2
{
    public partial class EventArchiveV2OrchestrationServiceTests
    {
        [Fact]
        public async Task
            ShouldThrowValidationExceptionOnBulkAddListenerEventArchiveV2sIfListenerEventArchiveV2sIsNullAndLogItAsync()
        {
            // given
            IEnumerable<ListenerEventArchiveV2> nullListenerEventArchiveV2s = null;

            var nullListenerEventArchiveV2sOrchestrationException =
                new NullListenerEventArchiveV2sOrchestrationException(
                    message: "Listener event archives are null.");

            var expectedEventArchiveV2OrchestrationValidationException =
                new EventArchiveV2OrchestrationValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: nullListenerEventArchiveV2sOrchestrationException);

            // when
            ValueTask<IEnumerable<ListenerEventArchiveV2>> bulkAddListenerEventArchiveV2sTask =
                this.eventArchiveV2OrchestrationService.BulkAddListenerEventArchiveV2sAsync(
                    nullListenerEventArchiveV2s,
                    TestContext.Current.CancellationToken);

            EventArchiveV2OrchestrationValidationException
                actualEventArchiveV2OrchestrationValidationException =
                    await Assert.ThrowsAsync<EventArchiveV2OrchestrationValidationException>(
                        bulkAddListenerEventArchiveV2sTask.AsTask);

            // then
            actualEventArchiveV2OrchestrationValidationException.Should().BeEquivalentTo(
                expectedEventArchiveV2OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2OrchestrationValidationException))),
                        Times.Once);

            this.listenerEventArchiveV2ProcessingServiceMock.Verify(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventArchiveV2>>(),
                    It.IsAny<System.Threading.CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
