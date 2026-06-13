// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using EventHighway.Abstractions.EventHandlers;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventHandlers.V2
{
    public partial class EventHandlerV2ServiceTests
    {
        [Fact]
        public void ShouldRetrieveAllEventHandlerV2s()
        {
            // given
            IEnumerable<IEventHandler> randomEventHandlers = CreateRandomEventHandlers();
            IEnumerable<IEventHandler> retrievedEventHandlers = randomEventHandlers;
            IEnumerable<IEventHandler> expectedEventHandlers = randomEventHandlers.DeepClone();

            this.eventHandlerBrokerMock.Setup(broker =>
                broker.GetAll())
                    .Returns(retrievedEventHandlers);

            // when
            IEnumerable<IEventHandler> actualEventHandlers =
                this.eventHandlerV2Service.RetrieveAllEventHandlerV2s();

            // then
            actualEventHandlers.Should().BeEquivalentTo(expectedEventHandlers);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.GetAll(),
                    Times.Once);

            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
        }
    }
}
