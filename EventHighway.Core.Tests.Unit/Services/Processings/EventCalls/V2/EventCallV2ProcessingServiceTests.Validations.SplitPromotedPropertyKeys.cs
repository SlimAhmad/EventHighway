// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Processings.EventCalls.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventCalls.V2
{
    public partial class EventCallV2ProcessingServiceTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ShouldThrowValidationExceptionOnSplitPromotedPropertyKeysIfPromotedPropertiesIsNullOrWhitespaceAndLogItAsync(
            string invalidPromotedProperties)
        {
            // given
            var invalidEventCallV2ProcessingException =
                new InvalidEventCallV2ProcessingException(
                    message: "Event call is invalid.");

            invalidEventCallV2ProcessingException.AddData(
                key: "promotedProperties",
                values: "Text is required");

            var expectedEventCallV2ProcessingValidationException =
                new EventCallV2ProcessingValidationException(
                    message: "Event call validation error occurred, fix the errors and try again.",
                    innerException: invalidEventCallV2ProcessingException);

            // when
            ValueTask<IEnumerable<string>> splitPromotedPropertyKeysTask =
                this.eventCallV2ProcessingService.SplitPromotedPropertyKeysAsync(
                    invalidPromotedProperties,
                    TestContext.Current.CancellationToken);

            EventCallV2ProcessingValidationException
                actualEventCallV2ProcessingValidationException =
                    await Assert.ThrowsAsync<EventCallV2ProcessingValidationException>(
                        splitPromotedPropertyKeysTask.AsTask);

            // then
            actualEventCallV2ProcessingValidationException.Should()
                .BeEquivalentTo(expectedEventCallV2ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventCallV2ProcessingValidationException))),
                        Times.Once);

            this.eventCallV2ServiceMock.VerifyNoOtherCalls();
            this.jsonBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
