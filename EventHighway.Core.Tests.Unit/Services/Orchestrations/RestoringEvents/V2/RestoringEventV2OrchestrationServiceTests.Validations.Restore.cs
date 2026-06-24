// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Orchestrations.RestoringEvents.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.RestoringEvents.V2
{
    public partial class RestoringEventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRestoreIfEventArchiveV2sIsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<EventArchiveV2> nullEventArchiveV2s = null;

            List<ListenerEventArchiveV2> someListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s();

            var nullRestoringEventV2OrchestrationException =
                new NullRestoringEventV2OrchestrationException(
                    message: "Event archives are null.");

            var expectedException =
                new RestoringEventV2OrchestrationValidationException(
                    message: "Restoring event validation error occurred, fix the errors and try again.",
                    innerException: nullRestoringEventV2OrchestrationException);

            // when
            ValueTask restoreTask =
                this.restoringEventV2OrchestrationService.RestoreAsync(
                    nullEventArchiveV2s,
                    someListenerEventArchiveV2s,
                    randomCancellationToken);

            RestoringEventV2OrchestrationValidationException actualException =
                await Assert.ThrowsAsync<RestoringEventV2OrchestrationValidationException>(
                    restoreTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Never);

            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRestoreIfListenerEventArchiveV2sIsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<EventArchiveV2> someEventArchiveV2s = CreateRandomEventArchiveV2s();

            List<ListenerEventArchiveV2> nullListenerEventArchiveV2s = null;

            var nullRestoringEventV2OrchestrationException =
                new NullRestoringEventV2OrchestrationException(
                    message: "Listener event archives are null.");

            var expectedException =
                new RestoringEventV2OrchestrationValidationException(
                    message: "Restoring event validation error occurred, fix the errors and try again.",
                    innerException: nullRestoringEventV2OrchestrationException);

            // when
            ValueTask restoreTask =
                this.restoringEventV2OrchestrationService.RestoreAsync(
                    someEventArchiveV2s,
                    nullListenerEventArchiveV2s,
                    randomCancellationToken);

            RestoringEventV2OrchestrationValidationException actualException =
                await Assert.ThrowsAsync<RestoringEventV2OrchestrationValidationException>(
                    restoreTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Never);

            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
