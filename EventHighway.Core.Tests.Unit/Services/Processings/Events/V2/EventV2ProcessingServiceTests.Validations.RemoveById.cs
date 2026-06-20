// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
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
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid invalidEventV2Id = Guid.Empty;

            var invalidEventV2Exception =
                new InvalidEventV2ProcessingException(
                    message: "Event is invalid, fix the errors and try again.");

            invalidEventV2Exception.AddData(
                key: nameof(EventV2.Id),
                values: "Required");

            var expectedEventV2ProcessingValidationException =
                new EventV2ProcessingValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV2Exception);

            // when
            ValueTask<EventV2> removeEventV2ByIdTask =
                this.eventV2ProcessingService
                    .RemoveEventV2ByIdAsync(
                        invalidEventV2Id,
                        randomCancellationToken);

            EventV2ProcessingValidationException
                actualEventV2ProcessingValidationException =
                    await Assert.ThrowsAsync<EventV2ProcessingValidationException>(
                        removeEventV2ByIdTask.AsTask);

            // then
            actualEventV2ProcessingValidationException.Should()
                .BeEquivalentTo(expectedEventV2ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ProcessingValidationException))),
                        Times.Once);

            this.eventV2ServiceMock.Verify(broker =>
                broker.RemoveEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
