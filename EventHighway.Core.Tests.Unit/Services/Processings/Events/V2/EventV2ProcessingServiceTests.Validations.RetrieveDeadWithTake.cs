// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Processings.Events.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.Events.V2
{
    public partial class EventV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveDeadWithTakeIfTakeIsInvalidAndLogItAsync()
        {
            // given
            int invalidTake = GetNegativeRandomNumber();

            var invalidEventV2Exception =
                new InvalidEventV2ProcessingException(
                    message: "Event is invalid, fix the errors and try again.");

            invalidEventV2Exception.AddData(
                key: "take",
                values: "Value must be greater than or equal to 0");

            var expectedEventV2ProcessingValidationException =
                new EventV2ProcessingValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV2Exception);

            // when
            ValueTask<IEnumerable<EventV2>> retrieveAllDeadEventV2sWithListenersTask =
                this.eventV2ProcessingService
                    .RetrieveAllDeadEventV2sWithListenersAsync(invalidTake);

            EventV2ProcessingValidationException
                actualEventV2ProcessingValidationException =
                    await Assert.ThrowsAsync<EventV2ProcessingValidationException>(
                        retrieveAllDeadEventV2sWithListenersTask.AsTask);

            // then
            actualEventV2ProcessingValidationException.Should()
                .BeEquivalentTo(expectedEventV2ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ProcessingValidationException))),
                        Times.Once);

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveAllEventV2sWithListenerEventV2sAsync(),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
