// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventHandlers.V2
{
    public partial class EventHandlerV2ServiceTests
    {
        [Fact]
        public void ShouldThrowServiceExceptionOnRegisterIfUnexpectedExceptionOccurs()
        {
            // given
            IEventHandler someEventHandler = CreateRandomEventHandler();
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            this.eventHandlerBrokerMock
                .Setup(broker => broker.Register(It.IsAny<IEventHandler>()))
                .Throws(serviceException);

            var failedEventHandlerV2ServiceException =
                new FailedEventHandlerV2ServiceException(
                    message: "Failed event handler service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventHandlerV2ServiceException =
                new EventHandlerV2ServiceException(
                    message: "Event handler service error occurred, contact support.",
                    innerException: failedEventHandlerV2ServiceException);

            // when
            Action registerEventHandlerV2Action = () =>
                this.eventHandlerV2Service.RegisterEventHandlerV2(someEventHandler);

            EventHandlerV2ServiceException actualEventHandlerV2ServiceException =
                Assert.Throws<EventHandlerV2ServiceException>(registerEventHandlerV2Action);

            // then
            actualEventHandlerV2ServiceException.Should()
                .BeEquivalentTo(expectedEventHandlerV2ServiceException);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.Register(someEventHandler),
                Times.Once);

            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
        }
    }
}
