// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventListeners.V2
{
    public partial class EventListenerV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventHandlerV2sAsync()
        {
            // given
            IEnumerable<IEventHandler> randomEventHandlers = CreateRandomEventHandlers();
            IEnumerable<IEventHandler> retrievedEventHandlers = randomEventHandlers;
            IEnumerable<IEventHandler> expectedEventHandlers = randomEventHandlers.DeepClone();

            this.eventHandlerV2ServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2s())
                    .Returns(retrievedEventHandlers);

            // when
            IEnumerable<IEventHandler> actualEventHandlers =
                await this.eventListenerV2OrchestrationService
                    .RetrieveAllEventHandlerV2sAsync(
                        TestContext.Current.CancellationToken);

            // then
            actualEventHandlers.Should().BeEquivalentTo(expectedEventHandlers);

            this.eventHandlerV2ServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2s(),
                    Times.Once);

            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventHandlerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
