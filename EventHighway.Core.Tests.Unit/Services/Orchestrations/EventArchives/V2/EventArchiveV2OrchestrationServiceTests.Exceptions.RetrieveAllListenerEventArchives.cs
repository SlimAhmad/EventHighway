// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V2;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V2
{
    public partial class EventArchiveV2OrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(ListenerEventArchiveV2DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAllListenerEventArchivesIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            var expectedEventArchiveV2OrchestrationDependencyException =
                new EventArchiveV2OrchestrationDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync())
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<IQueryable<ListenerEventArchiveV2>> retrieveAllTask =
                this.eventArchiveV2OrchestrationService.RetrieveAllListenerEventArchiveV2sAsync(
                    TestContext.Current.CancellationToken);

            EventArchiveV2OrchestrationDependencyException actualException =
                await Assert.ThrowsAsync<EventArchiveV2OrchestrationDependencyException>(
                    retrieveAllTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedEventArchiveV2OrchestrationDependencyException);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
