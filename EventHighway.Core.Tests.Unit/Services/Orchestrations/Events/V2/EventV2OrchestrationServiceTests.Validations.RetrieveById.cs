// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Orchestrations.Events.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.Events.V2
{
    public partial class EventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid invalidEventV2Id = Guid.Empty;

            var invalidEventV2Exception =
                new InvalidEventV2OrchestrationException(
                    message: "Event is invalid, fix the errors and try again.");

            invalidEventV2Exception.AddData(
                key: nameof(EventV2.Id),
                values: "Required");

            var expectedEventV2OrchestrationValidationException =
                new EventV2OrchestrationValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV2Exception);

            // when
            ValueTask<EventV2> retrieveEventV2ByIdTask =
                this.eventV2OrchestrationService
                    .RetrieveEventV2ByIdAsync(
                        invalidEventV2Id,
                        randomCancellationToken);

            EventV2OrchestrationValidationException
                actualEventV2OrchestrationValidationException =
                    await Assert.ThrowsAsync<EventV2OrchestrationValidationException>(
                        retrieveEventV2ByIdTask.AsTask);

            // then
            actualEventV2OrchestrationValidationException.Should()
                .BeEquivalentTo(expectedEventV2OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2OrchestrationValidationException))),
                        Times.Once);

            this.eventV2ProcessingServiceMock.Verify(broker =>
                broker.RetrieveEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
