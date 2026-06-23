// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V2
{
    public partial class EventArchiveV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveEventArchivesByIdsIfIdsIsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEnumerable<Guid> nullEventArchiveIds = null;

            var nullEventArchiveV2sOrchestrationException =
                new NullEventArchiveV2sOrchestrationException(
                    message: "Event archive ids are null.");

            var expectedEventArchiveV2OrchestrationValidationException =
                new EventArchiveV2OrchestrationValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: nullEventArchiveV2sOrchestrationException);

            // when
            ValueTask<IEnumerable<EventArchiveV2>> retrieveEventArchiveV2sByIdsTask =
                this.eventArchiveV2OrchestrationService.RetrieveEventArchiveV2sByIdsAsync(
                    nullEventArchiveIds,
                    randomCancellationToken);

            EventArchiveV2OrchestrationValidationException
                actualEventArchiveV2OrchestrationValidationException =
                    await Assert.ThrowsAsync<EventArchiveV2OrchestrationValidationException>(
                        retrieveEventArchiveV2sByIdsTask.AsTask);

            // then
            actualEventArchiveV2OrchestrationValidationException.Should().BeEquivalentTo(
                expectedEventArchiveV2OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2OrchestrationValidationException))),
                        Times.Once);

            this.eventArchiveV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
