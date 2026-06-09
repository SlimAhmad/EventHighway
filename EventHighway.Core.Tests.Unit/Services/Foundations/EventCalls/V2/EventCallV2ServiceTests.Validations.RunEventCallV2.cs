// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventCalls.V2
{
    public partial class EventCallV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRunIfEventCallV2IsNullAndLogItAsync()
        {
            // given
            EventCallV2 nullEventCallV2 = null;

            var nullEventCallV2Exception =
                new NullEventCallV2Exception(message: "Event call is null.");

            var expectedEventCallV2ValidationException =
                new EventCallV2ValidationException(
                    message: "Event call validation error occurred, fix the errors and try again.",
                    innerException: nullEventCallV2Exception);

            // when
            ValueTask<EventCallV2> runEventCallV2Task =
                this.eventCallV2Service.RunEventCallV2Async(nullEventCallV2);

            EventCallV2ValidationException actualEventCallV2ValidationException =
                await Assert.ThrowsAsync<EventCallV2ValidationException>(
                    runEventCallV2Task.AsTask);

            // then
            actualEventCallV2ValidationException.Should().BeEquivalentTo(
                expectedEventCallV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventCallV2ValidationException))),
                        Times.Once);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.HandleAsync(
                    It.IsAny<string>(),
                    It.IsAny<System.Collections.Generic.IReadOnlyDictionary<string, string>>(),
                    It.IsAny<System.Threading.CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnRunIfEventCallV2IsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            var invalidEventCallV2 = new EventCallV2
            {
                HandlerName = invalidText,
                HandlerConfigurations = null,
                Content = invalidText
            };

            var invalidEventCallV2Exception =
                new InvalidEventCallV2Exception(
                    message: "Event call is invalid, fix the errors and try again.");

            invalidEventCallV2Exception.AddData(
                key: nameof(EventCallV2.HandlerName),
                values: "Text required");

            invalidEventCallV2Exception.AddData(
                key: nameof(EventCallV2.HandlerConfigurations),
                values: "Configuration required");

            invalidEventCallV2Exception.AddData(
                key: nameof(EventCallV2.Content),
                values: "Payload required");

            var expectedEventCallV2ValidationException =
                new EventCallV2ValidationException(
                    message: "Event call validation error occurred, fix the errors and try again.",
                    innerException: invalidEventCallV2Exception);

            // when
            ValueTask<EventCallV2> runEventCallV2Task =
                this.eventCallV2Service.RunEventCallV2Async(invalidEventCallV2);

            EventCallV2ValidationException actualEventCallV2ValidationException =
                await Assert.ThrowsAsync<EventCallV2ValidationException>(
                    runEventCallV2Task.AsTask);

            // then
            actualEventCallV2ValidationException.Should().BeEquivalentTo(
                expectedEventCallV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventCallV2ValidationException))),
                        Times.Once);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.HandleAsync(
                    It.IsAny<string>(),
                    It.IsAny<IReadOnlyDictionary<string, string>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
        }
    }
}
