// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Brokers.EventHandlers;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations;
using EventHighway.Core.Services.Foundations.EventCalls.V2;
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
                this.eventCallV2Service.RunEventCallV2Async(nullEventCallV2, TestContext.Current.CancellationToken);

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

            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
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
                this.eventCallV2Service.RunEventCallV2Async(invalidEventCallV2, TestContext.Current.CancellationToken);

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

            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRunIfHandlerBrokerIsNullAndLogItAsync()
        {
            // given
            EventCallV2 someEventCallV2 = CreateRandomEventCallV2();

            IEventCallV2Service serviceWithNullBroker = new EventCallV2Service(
                eventHandlerBroker: null,
                loggingBroker: this.loggingBrokerMock.Object);

            var handlerNotFoundEventCallV2Exception =
                new HandlerNotFoundEventCallV2Exception(
                    message: "No event call handler was found, fix the errors and try again.");

            var expectedEventCallV2ValidationException =
                new EventCallV2ValidationException(
                    message: "Event call validation error occurred, fix the errors and try again.",
                    innerException: handlerNotFoundEventCallV2Exception);

            // when
            ValueTask<EventCallV2> runEventCallV2Task =
                serviceWithNullBroker.RunEventCallV2Async(someEventCallV2, TestContext.Current.CancellationToken);

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

            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(2)]
        public async Task ShouldThrowValidationExceptionOnRunIfHandlerRegistrationCountIsInvalidAndLogItAsync(
            int matchingHandlerCount)
        {
            // given
            string randomHandlerName = GetRandomString();
            EventCallV2 someEventCallV2 = CreateRandomEventCallV2();
            someEventCallV2.HandlerName = randomHandlerName;

            IEventHandler[] handlers;

            if (matchingHandlerCount == 0)
            {
                var noMatchHandlerMock = new Mock<IEventHandler>();
                noMatchHandlerMock.SetupGet(h => h.Name).Returns(GetRandomString());
                handlers = new[] { noMatchHandlerMock.Object };
            }
            else
            {
                var matchHandlerMock1 = new Mock<IEventHandler>();
                matchHandlerMock1.SetupGet(h => h.Name).Returns(randomHandlerName);

                var matchHandlerMock2 = new Mock<IEventHandler>();
                matchHandlerMock2.SetupGet(h => h.Name).Returns(randomHandlerName);

                handlers = new[] { matchHandlerMock1.Object, matchHandlerMock2.Object };
            }

            var localBrokerMock = new Mock<IEventHandlerBroker>();
            localBrokerMock.Setup(b => b.GetAll()).Returns(handlers);

            IEventCallV2Service localService = new EventCallV2Service(
                eventHandlerBroker: localBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);

            var invalidEventCallV2Exception =
                new InvalidEventCallV2Exception(
                    message: "EventHandlerBrokers on event call is invalid, fix the errors and try again.");

            invalidEventCallV2Exception.AddData(
                key: nameof(EventCallV2.HandlerName),
                values: matchingHandlerCount == 0
                    ? $"No handler found that matches '{randomHandlerName}', " +
                        $"fix registrations and try again."
                    : $"Multiple providers found that matches '{randomHandlerName}', " +
                        $"fix registrations and try again.");

            var expectedEventCallV2ValidationException =
                new EventCallV2ValidationException(
                    message: "Event call validation error occurred, fix the errors and try again.",
                    innerException: invalidEventCallV2Exception);

            // when
            ValueTask<EventCallV2> runEventCallV2Task =
                localService.RunEventCallV2Async(someEventCallV2, TestContext.Current.CancellationToken);

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

            localBrokerMock.Verify(b => b.GetAll(), Times.AtLeastOnce);
            localBrokerMock.VerifyNoOtherCalls();
            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnRunIfRequiredHandlerConfigurationIsInvalidAndLogItAsync(
            string invalidValue)
        {
            // given
            string randomHandlerName = GetRandomString();
            string requiredParam = GetRandomString();

            EventCallV2 inputEventCallV2 = CreateRandomEventCallV2();
            inputEventCallV2.HandlerName = randomHandlerName;

            inputEventCallV2.HandlerConfigurations = invalidValue is null
                ? new List<HandlerConfiguration>()
                : new List<HandlerConfiguration>
                {
                    new HandlerConfiguration { Name = requiredParam, Value = invalidValue }
                };

            this.eventHandlerBrokerMock.Setup(broker => broker.GetAll())
                .Returns(new[] { this.eventHandlerMock.Object });

            this.eventHandlerMock.SetupGet(handler => handler.Id)
                .Returns(inputEventCallV2.HandlerId);

            this.eventHandlerMock.SetupGet(handler => handler.Name)
                .Returns(randomHandlerName);

            this.eventHandlerMock.SetupGet(handler => handler.RequiredParams)
                .Returns(new[] { requiredParam });

            var invalidEventCallV2Exception =
                new InvalidEventCallV2Exception(
                    message: "Event call handler configuration is invalid, fix the errors and try again.");

            invalidEventCallV2Exception.AddData(
                key: $"HandlerConfiguration['{requiredParam}']",
                values: invalidValue is null ? "Config item required" : "Value required");

            var expectedEventCallV2ValidationException =
                new EventCallV2ValidationException(
                    message: "Event call validation error occurred, fix the errors and try again.",
                    innerException: invalidEventCallV2Exception);

            // when
            ValueTask<EventCallV2> runEventCallV2Task =
                this.eventCallV2Service.RunEventCallV2Async(inputEventCallV2, TestContext.Current.CancellationToken);

            EventCallV2ValidationException actualEventCallV2ValidationException =
                await Assert.ThrowsAsync<EventCallV2ValidationException>(
                    runEventCallV2Task.AsTask);

            // then
            actualEventCallV2ValidationException.Should().BeEquivalentTo(
                expectedEventCallV2ValidationException);

            this.eventHandlerBrokerMock.Verify(broker => broker.GetAll(),
                Times.AtLeastOnce);

            this.eventHandlerBrokerMock.VerifyNoOtherCalls();

            this.eventHandlerMock.VerifyGet(handler => handler.Id,
                Times.AtLeastOnce);

            this.eventHandlerMock.VerifyGet(handler => handler.Name,
                Times.AtLeastOnce);

            this.eventHandlerMock.VerifyGet(handler => handler.RequiredParams,
                Times.AtLeastOnce);

            this.eventHandlerMock.Verify(handler =>
                handler.HandleAsync(
                    It.IsAny<string>(),
                    It.IsAny<IReadOnlyDictionary<string, string>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventHandlerMock.VerifyNoOtherCalls();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventCallV2ValidationException))),
                        Times.Once);

            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
