// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V2
{
    public partial class EventArchiveV2OrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(EventArchiveV2DependencyExceptions))]
        public async Task
            ShouldThrowDependencyExceptionOnRetrieveAllEventArchivesIfDependencyExceptionOccursAndLogItAsync(
                Xeption dependencyException)
        {
            // given
            var expectedEventArchiveV2OrchestrationDependencyException =
                new EventArchiveV2OrchestrationDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventArchiveV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync())
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<IQueryable<EventArchiveV2>> retrieveAllTask =
                this.eventArchiveV2OrchestrationService.RetrieveAllEventArchiveV2sAsync(
                    TestContext.Current.CancellationToken);

            EventArchiveV2OrchestrationDependencyException actualException =
                await Assert.ThrowsAsync<EventArchiveV2OrchestrationDependencyException>(
                    retrieveAllTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedEventArchiveV2OrchestrationDependencyException);

            this.eventArchiveV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
