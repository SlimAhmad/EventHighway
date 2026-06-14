// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Orchestrations.Events.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.Events.V2
{
    public partial class EventV2OrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(EventV2DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAllEventV2sIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            var expectedEventV2OrchestrationDependencyException =
                new EventV2OrchestrationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync())
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<IQueryable<EventV2>> retrieveAllTask =
                this.eventV2OrchestrationService.RetrieveAllEventV2sAsync(
                    TestContext.Current.CancellationToken);

            EventV2OrchestrationDependencyException actualException =
                await Assert.ThrowsAsync<EventV2OrchestrationDependencyException>(
                    retrieveAllTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedEventV2OrchestrationDependencyException);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventAddressV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
