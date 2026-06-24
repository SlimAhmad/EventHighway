// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.ReplayingListenerEvents.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ReplayingListenerEvents.V2
{
    public partial class ReplayingListenerEventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnProcessReplayIfListenerEventV2IsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            ListenerEventV2 nullListenerEventV2 = null;

            var nullReplayingListenerEventV2OrchestrationException =
                new NullReplayingListenerEventV2OrchestrationException(
                    message: "Listener event is null.");

            var expectedReplayingListenerEventV2OrchestrationValidationException =
                new ReplayingListenerEventV2OrchestrationValidationException(
                    message: "Replaying listener event validation error occurred, fix the errors and try again.",
                    innerException: nullReplayingListenerEventV2OrchestrationException);

            // when
            ValueTask<ListenerEventV2> processReplayTask =
                this.replayingListenerEventV2OrchestrationService
                    .ProcessReplayListenerEventV2Async(
                        nullListenerEventV2,
                        randomCancellationToken);

            ReplayingListenerEventV2OrchestrationValidationException actualException =
                await Assert.ThrowsAsync<ReplayingListenerEventV2OrchestrationValidationException>(
                    processReplayTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedReplayingListenerEventV2OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedReplayingListenerEventV2OrchestrationValidationException))),
                        Times.Once);

            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
