// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Orchestrations.Events.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.Events.V2
{
    public partial class EventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnSubmitIfEventV2IsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 nullEventV2 = null;

            var nullEventV2OrchestrationException =
                new NullEventV2OrchestrationException(message: "Event is null.");

            var expectedEventV2OrchestrationValidationException =
                new EventV2OrchestrationValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: nullEventV2OrchestrationException);

            // when
            ValueTask<EventV2> submitEventV2Task =
                this.eventV2OrchestrationService.SubmitEventV2Async(
                    nullEventV2,
                    randomCancellationToken);

            EventV2OrchestrationValidationException
                actualEventV2OrchestrationValidationException =
                    await Assert.ThrowsAsync<EventV2OrchestrationValidationException>(
                        submitEventV2Task.AsTask);

            // then
            actualEventV2OrchestrationValidationException.Should().BeEquivalentTo(
                expectedEventV2OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2OrchestrationValidationException))),
                        Times.Once);

            this.eventAddressV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventV2ProcessingServiceMock.Verify(broker =>
                broker.AddEventV2Async(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventAddressV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ShouldThrowValidationExceptionOnSubmitIfCleanedContentIsInvalidAsync(
            string invalidCleanedContent)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 someEventV2 = CreateRandomEventV2();
            EventAddressV2 randomEventAddressV2 = CreateRandomEventAddressV2();
            EventAddressV2 retrievedEventAddressV2 = randomEventAddressV2;

            var invalidEventV2OrchestrationException =
                new InvalidEventV2OrchestrationException(
                    message: "Event is invalid, fix the errors and try again.");

            invalidEventV2OrchestrationException.UpsertDataList(
                key: nameof(EventV2.Content),
                value: "Required");

            var expectedEventV2OrchestrationValidationException =
                new EventV2OrchestrationValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV2OrchestrationException);

            this.eventAddressV2ProcessingServiceMock.Setup(service =>
                service.RetrieveEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(retrievedEventAddressV2);

            this.eventV2ProcessingServiceMock.Setup(service =>
                service.RemoveVolatilePathsAsync(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(invalidCleanedContent);

            // when
            ValueTask<EventV2> submitEventV2Task =
                this.eventV2OrchestrationService.SubmitEventV2Async(
                    someEventV2,
                    randomCancellationToken);

            EventV2OrchestrationValidationException
                actualEventV2OrchestrationValidationException =
                    await Assert.ThrowsAsync<EventV2OrchestrationValidationException>(
                        submitEventV2Task.AsTask);

            // then
            actualEventV2OrchestrationValidationException.Should()
                .BeEquivalentTo(expectedEventV2OrchestrationValidationException);

            this.eventAddressV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RemoveVolatilePathsAsync(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2OrchestrationValidationException))),
                        Times.Once);

            this.hashBrokerMock.Verify(broker =>
                broker.GenerateSha256Hash(It.IsAny<string>()),
                    Times.Never);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.AddEventV2Async(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventAddressV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnSubmitIfEventAddressV2IsNotFoundAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 someEventV2 = CreateRandomEventV2();
            Guid nonExistingEventAddressV2Id = someEventV2.EventAddressId;
            EventAddressV2 nullEventAddressV2 = null;

            var notFoundEventAddressV2OrchestrationException =
                new NotFoundEventAddressV2OrchestrationException(
                    message: $"Could not find event address with id: {nonExistingEventAddressV2Id}.");

            var expectedEventV2OrchestrationValidationException =
                new EventV2OrchestrationValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: notFoundEventAddressV2OrchestrationException);

            this.eventAddressV2ProcessingServiceMock.Setup(broker =>
                broker.RetrieveEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(nullEventAddressV2);

            // when
            ValueTask<EventV2> submitEventV2Task =
                this.eventV2OrchestrationService.SubmitEventV2Async(
                    someEventV2,
                    randomCancellationToken);

            EventV2OrchestrationValidationException actualEventV2OrchestrationValidationException =
                await Assert.ThrowsAsync<EventV2OrchestrationValidationException>(
                    submitEventV2Task.AsTask);

            // then
            actualEventV2OrchestrationValidationException.Should()
                .BeEquivalentTo(expectedEventV2OrchestrationValidationException);

            this.eventAddressV2ProcessingServiceMock.Verify(broker =>
                broker.RetrieveEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2OrchestrationValidationException))),
                        Times.Once);

            this.eventV2ProcessingServiceMock.Verify(broker =>
                broker.AddEventV2Async(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventAddressV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
