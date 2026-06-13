// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventHandlers.V2
{
    public partial class EventHandlerV2ServiceTests
    {
        [Fact]
        public void ShouldThrowServiceExceptionOnRetrieveAllIfUnexpectedExceptionOccurs()
        {
            // given
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventHandlerV2ServiceException =
                new FailedEventHandlerV2ServiceException(
                    message: "Failed event handler service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventHandlerV2ServiceException =
                new EventHandlerV2ServiceException(
                    message: "Event handler service error occurred, contact support.",
                    innerException: failedEventHandlerV2ServiceException);

            this.eventHandlerBrokerMock
                .Setup(broker => broker.GetAll())
                .Throws(serviceException);

            // when
            Action retrieveAllAction = () =>
                this.eventHandlerV2Service.RetrieveAllEventHandlerV2s();

            EventHandlerV2ServiceException actualEventHandlerV2ServiceException =
                Assert.Throws<EventHandlerV2ServiceException>(retrieveAllAction);

            // then
            actualEventHandlerV2ServiceException.Should()
                .BeEquivalentTo(expectedEventHandlerV2ServiceException);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.GetAll(),
                Times.Once);

            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
        }
    }
}
