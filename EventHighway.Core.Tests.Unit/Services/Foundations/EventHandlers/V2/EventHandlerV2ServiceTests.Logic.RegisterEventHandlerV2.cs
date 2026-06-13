// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Abstractions.EventHandlers;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventHandlers.V2
{
    public partial class EventHandlerV2ServiceTests
    {
        [Fact]
        public void ShouldRegisterEventHandlerV2()
        {
            // given
            IEventHandler randomEventHandler = CreateRandomEventHandler();
            IEventHandler inputEventHandler = randomEventHandler;

            // when
            this.eventHandlerV2Service.RegisterEventHandlerV2(inputEventHandler);

            // then
            this.eventHandlerBrokerMock.Verify(broker =>
                broker.Register(inputEventHandler),
                Times.Once);

            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
        }
    }
}
