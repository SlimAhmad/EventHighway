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

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;
            inputEventV2.ScheduledDate = null;
            EventV2 inputImmediateEventV2 = inputEventV2;
            inputImmediateEventV2.Type = EventTypeV2.Immediate;
            EventV2 submittedEventV2 = inputImmediateEventV2;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.SubmitEventV2Async(
                    inputImmediateEventV2,
                    randomCancellationToken))
                        .ReturnsAsync(submittedEventV2);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.IsLoopDetectedAsync(
                    submittedEventV2,
                    randomCancellationToken))
                        .ReturnsAsync(true);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.ModifyEventV2Async(
                    submittedEventV2,
                    randomCancellationToken))
                        .ReturnsAsync(submittedEventV2);

            // when
            ValueTask<EventV2> submitEventV2Task =
                this.eventV2CoordinationService.SubmitEventV2Async(
                    inputEventV2,
                    randomCancellationToken);

            await Assert.ThrowsAsync<LoopDetectedEventV2CoordinationException>(
                submitEventV2Task.AsTask);

            // then
            submittedEventV2.Status.Should().Be(EventStatusV2.Quarantined);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.SubmitEventV2Async(
                    inputImmediateEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.IsLoopDetectedAsync(
                    submittedEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.ModifyEventV2Async(
                    submittedEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.jsonBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
