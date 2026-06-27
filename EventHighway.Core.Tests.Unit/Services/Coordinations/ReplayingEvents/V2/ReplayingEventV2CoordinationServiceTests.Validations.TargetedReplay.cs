// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.ReplayingEvents.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.ReplayingEvents.V2
{
    public partial class ReplayingEventV2CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnTargetedReplayIfEventV2IdIsInvalidAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid invalidEventV2Id = Guid.Empty;
            Guid? eventAddressId = GetRandomId();
            List<Guid> eventListenerIds = new List<Guid> { GetRandomId() };

            var invalidReplayingEventV2CoordinationException =
                new InvalidReplayingEventV2CoordinationException(
                    message: "Replaying event is invalid, fix the errors and try again.");

            invalidReplayingEventV2CoordinationException.UpsertDataList(
                key: "eventV2Id",
                value: "Id is required");

            var expectedException =
                new ReplayingEventV2CoordinationValidationException(
                    message: "Replaying event validation error occurred, fix the errors and try again.",
                    innerException: invalidReplayingEventV2CoordinationException);

            // when
            ValueTask replayTask =
                this.replayingEventV2CoordinationService.ReplayEventArchiveV2sAsync(
                    invalidEventV2Id, eventAddressId, eventListenerIds, false, randomCancellationToken);

            ReplayingEventV2CoordinationValidationException actualException =
                await Assert.ThrowsAsync<ReplayingEventV2CoordinationValidationException>(
                    replayTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventArchiveV2sByIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.restoringEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnTargetedReplayIfEventListenerIdsIsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid eventV2Id = GetRandomId();
            Guid? eventAddressId = GetRandomId();
            IEnumerable<Guid> nullEventListenerIds = null;

            var invalidReplayingEventV2CoordinationException =
                new InvalidReplayingEventV2CoordinationException(
                    message: "Replaying event is invalid, fix the errors and try again.");

            invalidReplayingEventV2CoordinationException.UpsertDataList(
                key: "eventListenerIds",
                value: "Event listener ids are required");

            var expectedException =
                new ReplayingEventV2CoordinationValidationException(
                    message: "Replaying event validation error occurred, fix the errors and try again.",
                    innerException: invalidReplayingEventV2CoordinationException);

            // when
            ValueTask replayTask =
                this.replayingEventV2CoordinationService.ReplayEventArchiveV2sAsync(
                    eventV2Id, eventAddressId, nullEventListenerIds, false, randomCancellationToken);

            ReplayingEventV2CoordinationValidationException actualException =
                await Assert.ThrowsAsync<ReplayingEventV2CoordinationValidationException>(
                    replayTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventArchiveV2sByIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.restoringEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
