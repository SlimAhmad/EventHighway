// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Orchestrations.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ArchivingEvents.V2
{
    public partial class ArchivingEvent2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveDeadIfEventV2IsNullAndLogItAsync()
        {
            // given
            EventV2 nullEventV2 = null;

            var nullArchivingEvent2OrchestrationException =
                new NullArchivingEvent2OrchestrationException(
                    message: "Event is null.");

            var expectedArchivingEvent2OrchestrationValidationException =
                new ArchivingEvent2OrchestrationValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: nullArchivingEvent2OrchestrationException);

            // when
            ValueTask removeEventV2AndListenerEventV2sTask =
                this.archivingEvent2OrchestrationService.RemoveEventV2AndListenerEventV2sAsync(
                    nullEventV2,
                    TestContext.Current.CancellationToken);

            ArchivingEvent2OrchestrationValidationException
                actualArchivingEvent2OrchestrationValidationException =
                    await Assert.ThrowsAsync<ArchivingEvent2OrchestrationValidationException>(
                        removeEventV2AndListenerEventV2sTask.AsTask);

            // then
            actualArchivingEvent2OrchestrationValidationException.Should().BeEquivalentTo(
                expectedArchivingEvent2OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEvent2OrchestrationValidationException))),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RemoveListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RemoveEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
