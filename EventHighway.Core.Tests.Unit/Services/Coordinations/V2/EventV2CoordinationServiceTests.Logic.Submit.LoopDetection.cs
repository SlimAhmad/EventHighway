// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Coordinations.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.V2
{
    public partial class EventV2CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldQuarantineAndThrowLoopDetectedExceptionWhenLoopIsDetectedOnSubmitAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var mockSequence = new MockSequence();

            SetupValidateEventParticipantsInSequence(mockSequence);

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;
            inputEventV2.ScheduledDate = null;
            EventV2 inputImmediateEventV2 = inputEventV2;
            inputImmediateEventV2.Type = EventTypeV2.Immediate;
            EventV2 submittedEventV2 = inputImmediateEventV2;

            var loopDetectedEventV2CoordinationException =
                new LoopDetectedEventV2CoordinationException(
                    message: "Event loop detected, event quarantined.");

            var expectedEventV2CoordinationValidationException =
                new EventV2CoordinationValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: loopDetectedEventV2CoordinationException);

            this.dateTimeBrokerMock.InSequence(mockSequence).Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.eventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.IsLoopDetectedAsync(
                        inputImmediateEventV2,
                        randomCancellationToken))
                            .ReturnsAsync(true);

            this.eventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.SubmitEventV2Async(
                        inputImmediateEventV2,
                        randomCancellationToken))
                            .ReturnsAsync(submittedEventV2);

            // when
            ValueTask<EventV2> submitEventV2Task =
                this.eventV2CoordinationService.SubmitEventV2Async(
                    inputEventV2,
                    randomCancellationToken);

            EventV2CoordinationValidationException
                actualEventV2CoordinationValidationException =
                    await Assert.ThrowsAsync<EventV2CoordinationValidationException>(
                        submitEventV2Task.AsTask);

            // then
            actualEventV2CoordinationValidationException.Should()
                .BeEquivalentTo(expectedEventV2CoordinationValidationException);

            submittedEventV2.Status.Should().Be(EventStatusV2.Quarantined);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.IsLoopDetectedAsync(
                    inputImmediateEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.SubmitEventV2Async(
                    inputImmediateEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2CoordinationValidationException))),
                        Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            VerifyValidateEventParticipantsCalledOnce();

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventParticipantV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
