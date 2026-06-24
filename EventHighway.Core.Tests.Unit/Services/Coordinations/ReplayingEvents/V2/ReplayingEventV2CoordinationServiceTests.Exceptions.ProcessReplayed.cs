// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.ReplayingEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.ReplayingEvents.V2
{
    public partial class ReplayingEventV2CoordinationServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptionsForProcessReplayed))]
        public async Task
            ShouldThrowDependencyValidationExceptionOnProcessReplayedIfDependencyValidationErrorOccursAndLogItAsync(
                Xeption dependencyValidationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int randomTake = GetRandomNumber();

            var expectedReplayingEventV2CoordinationDependencyValidationException =
                new ReplayingEventV2CoordinationDependencyValidationException(
                    message: "Replaying event validation error occurred, fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(CreateBatchConfiguration(randomTake));

            this.replayingListenerEventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfReplayListenerEventV2sAsync(
                    randomTake, randomCancellationToken))
                .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask processReplayedTask =
                this.replayingEventV2CoordinationService
                    .ProcessReplayedListenerEventV2sAsync(randomCancellationToken);

            ReplayingEventV2CoordinationDependencyValidationException actualException =
                await Assert.ThrowsAsync<ReplayingEventV2CoordinationDependencyValidationException>(
                    processReplayedTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedReplayingEventV2CoordinationDependencyValidationException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(), Times.Once);

            this.replayingListenerEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfReplayListenerEventV2sAsync(
                    randomTake, randomCancellationToken),
                Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedReplayingEventV2CoordinationDependencyValidationException))),
                Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.restoringEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.replayingListenerEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
