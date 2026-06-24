// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.ReplayingListenerEvents.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ReplayingListenerEvents.V2
{
    public partial class ReplayingListenerEventV2OrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptionsForRetrieveBatchReplay))]
        public async Task
            ShouldThrowDependencyValidationExceptionOnRetrieveBatchReplayIfDependencyValidationErrorOccursAndLogItAsync(
                Xeption dependencyValidationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int randomTake = GetRandomNumber();

            var expectedReplayingListenerEventV2OrchestrationDependencyValidationException =
                new ReplayingListenerEventV2OrchestrationDependencyValidationException(
                    message: "Replaying listener event validation error occurred, fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfReplayListenerEventV2sAsync(randomTake, randomCancellationToken))
                    .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> retrieveBatchTask =
                this.replayingListenerEventV2OrchestrationService
                    .RetrieveBatchOfReplayListenerEventV2sAsync(randomTake, randomCancellationToken);

            ReplayingListenerEventV2OrchestrationDependencyValidationException actualException =
                await Assert.ThrowsAsync<ReplayingListenerEventV2OrchestrationDependencyValidationException>(
                    retrieveBatchTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedReplayingListenerEventV2OrchestrationDependencyValidationException);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfReplayListenerEventV2sAsync(randomTake, randomCancellationToken),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedReplayingListenerEventV2OrchestrationDependencyValidationException))),
                        Times.Once);

            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
