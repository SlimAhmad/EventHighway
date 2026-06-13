// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventHandlers.V2
{
    public partial class EventHandlerV2ServiceTests
    {
        [Fact]
        public void ShouldThrowValidationExceptionOnRegisterIfEventHandlerIsNull()
        {
            // given
            IEventHandler nullEventHandler = null;

            var nullEventHandlerV2Exception =
                new NullEventHandlerV2Exception(
                    message: "Event handler is null.");

            var expectedEventHandlerV2ValidationException =
                new EventHandlerV2ValidationException(
                    message: "Event handler validation error occurred, fix the errors and try again.",
                    innerException: nullEventHandlerV2Exception);

            // when
            Action registerEventHandlerV2Action = () =>
                this.eventHandlerV2Service.RegisterEventHandlerV2(nullEventHandler);

            EventHandlerV2ValidationException actualEventHandlerV2ValidationException =
                Assert.Throws<EventHandlerV2ValidationException>(registerEventHandlerV2Action);

            // then
            actualEventHandlerV2ValidationException.Should().BeEquivalentTo(
                expectedEventHandlerV2ValidationException);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.Register(It.IsAny<IEventHandler>()),
                Times.Never);

            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void ShouldThrowValidationExceptionOnRegisterIfEventHandlerIsInvalid(
            string invalidName)
        {
            // given
            var invalidEventHandlerMock = new Mock<IEventHandler>();
            invalidEventHandlerMock.SetupGet(h => h.Id).Returns(Guid.Empty);
            invalidEventHandlerMock.SetupGet(h => h.Name).Returns(invalidName);
            IEventHandler invalidEventHandler = invalidEventHandlerMock.Object;

            var invalidEventHandlerV2Exception =
                new InvalidEventHandlerV2Exception(
                    message: "Event handler is invalid, fix the errors and try again.");

            invalidEventHandlerV2Exception.AddData(
                key: nameof(IEventHandler.Id),
                values: "Id required");

            invalidEventHandlerV2Exception.AddData(
                key: nameof(IEventHandler.Name),
                values: "Text required");

            var expectedEventHandlerV2ValidationException =
                new EventHandlerV2ValidationException(
                    message: "Event handler validation error occurred, fix the errors and try again.",
                    innerException: invalidEventHandlerV2Exception);

            // when
            Action registerEventHandlerV2Action = () =>
                this.eventHandlerV2Service.RegisterEventHandlerV2(invalidEventHandler);

            EventHandlerV2ValidationException actualEventHandlerV2ValidationException =
                Assert.Throws<EventHandlerV2ValidationException>(registerEventHandlerV2Action);

            // then
            actualEventHandlerV2ValidationException.Should().BeEquivalentTo(
                expectedEventHandlerV2ValidationException);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.Register(It.IsAny<IEventHandler>()),
                Times.Never);

            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
        }
    }
}
