// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventListeners.V2
{
    public partial class EventListenerV2OrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(EventHandlerV2DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAllEventHandlersIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            var expectedEventListenerV2OrchestrationDependencyException =
                new EventListenerV2OrchestrationDependencyException(
                    message: "Event listener dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventHandlerV2ServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2s())
                    .Throws(dependencyException);

            // when
            ValueTask<IEnumerable<IEventHandler>> retrieveAllTask =
                this.eventListenerV2OrchestrationService.RetrieveAllEventHandlerV2sAsync(
                    TestContext.Current.CancellationToken);

            EventListenerV2OrchestrationDependencyException actualException =
                await Assert.ThrowsAsync<EventListenerV2OrchestrationDependencyException>(
                    retrieveAllTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedEventListenerV2OrchestrationDependencyException);

            this.eventHandlerV2ServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2s(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
