// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.ReplayingEvents.V2.Exceptions;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.ReplayingEvents.V2
{
    public partial class ReplayingEventV2CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnReplayIfStartDateIsAfterEndDateAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid? eventAddressId = GetRandomId();
            IEnumerable<Guid> eventListenerIds = null;
            DateTimeOffset? endDate = GetRandomDateTimeOffset();
            DateTimeOffset? startDate = endDate.Value.AddDays(GetRandomNumber());

            var invalidReplayingEventV2CoordinationException =
                new InvalidReplayingEventV2CoordinationException(
                    message: "Replaying event is invalid, fix the errors and try again.");

            invalidReplayingEventV2CoordinationException.UpsertDataList(
                key: "startDate",
                value: "Date is later than endDate");

            var expectedException =
                new ReplayingEventV2CoordinationValidationException(
                    message: "Replaying event validation error occurred, fix the errors and try again.",
                    innerException: invalidReplayingEventV2CoordinationException);

            // when
            ValueTask replayTask =
                this.replayingEventV2CoordinationService.ReplayEventArchiveV2sAsync(
                    eventAddressId, eventListenerIds, startDate, endDate, randomCancellationToken);

            ReplayingEventV2CoordinationValidationException actualException =
                await Assert.ThrowsAsync<ReplayingEventV2CoordinationValidationException>(
                    replayTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Never);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.restoringEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
