// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V2
{
    public partial class EventArchiveV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfEventArchiveV2IsNullAndLogItAsync()
        {
            // given
            EventArchiveV2 nullEventArchiveV2 = null;

            var nullEventArchiveV2OrchestrationException =
                new NullEventArchiveV2OrchestrationException(
                    message: "Event archive is null.");

            var expectedEventArchiveV2OrchestrationValidationException =
                new EventArchiveV2OrchestrationValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: nullEventArchiveV2OrchestrationException);

            // when
            ValueTask addEventArchiveV2Task =
                this.eventArchiveV2OrchestrationService
                    .AddEventArchiveV2WithListenerEventArchiveV2sAsync(
                        nullEventArchiveV2,
                        TestContext.Current.CancellationToken);

            EventArchiveV2OrchestrationValidationException
                actualEventArchiveV2OrchestrationValidationException =
                    await Assert.ThrowsAsync<EventArchiveV2OrchestrationValidationException>(
                        addEventArchiveV2Task.AsTask);

            // then
            actualEventArchiveV2OrchestrationValidationException.Should().BeEquivalentTo(
                expectedEventArchiveV2OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2OrchestrationValidationException))),
                        Times.Once);

            this.listenerEventArchiveV2ProcessingServiceMock.Verify(service =>
                service.AddListenerEventArchiveV2Async(
                    It.IsAny<ListenerEventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventArchiveV2ProcessingServiceMock.Verify(broker =>
                broker.AddEventArchiveV2Async(
                    It.IsAny<EventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfListenerEventArchiveV2sAreNullAndLogItAsync()
        {
            // given
            var invalidEventArchiveV2 = new EventArchiveV2();
            invalidEventArchiveV2.ListenerEventArchiveV2s = null;

            var nullListenerEventArchiveV2sOrchestrationException =
                new NullListenerEventArchiveV2sOrchestrationException(
                    message: "Listener event archives are null.");

            var expectedEventArchiveV2OrchestrationValidationException =
                new EventArchiveV2OrchestrationValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: nullListenerEventArchiveV2sOrchestrationException);

            // when
            ValueTask addEventArchiveV2Task =
                this.eventArchiveV2OrchestrationService
                    .AddEventArchiveV2WithListenerEventArchiveV2sAsync(
                        invalidEventArchiveV2,
                        TestContext.Current.CancellationToken);

            EventArchiveV2OrchestrationValidationException
                actualEventArchiveV2OrchestrationValidationException =
                    await Assert.ThrowsAsync<EventArchiveV2OrchestrationValidationException>(
                        addEventArchiveV2Task.AsTask);

            // then
            actualEventArchiveV2OrchestrationValidationException.Should().BeEquivalentTo(
                expectedEventArchiveV2OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2OrchestrationValidationException))),
                        Times.Once);

            this.listenerEventArchiveV2ProcessingServiceMock.Verify(service =>
                service.AddListenerEventArchiveV2Async(
                    It.IsAny<ListenerEventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventArchiveV2ProcessingServiceMock.Verify(broker =>
                broker.AddEventArchiveV2Async(
                    It.IsAny<EventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
