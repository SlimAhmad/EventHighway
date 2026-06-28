// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

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
        public async Task ShouldThrowValidationExceptionOnStampContentHashIfEventV2IsNullAndLogItAsync()
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
            ValueTask<EventV2> stampContentHashTask =
                this.eventV2OrchestrationService.StampContentHashAsync(
                    nullEventV2,
                    randomCancellationToken);

            EventV2OrchestrationValidationException
                actualEventV2OrchestrationValidationException =
                    await Assert.ThrowsAsync<EventV2OrchestrationValidationException>(
                        stampContentHashTask.AsTask);

            // then
            actualEventV2OrchestrationValidationException.Should().BeEquivalentTo(
                expectedEventV2OrchestrationValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2OrchestrationValidationException))),
                        Times.Once);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RemoveVolatilePathsAsync(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventAddressV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task ShouldThrowValidationExceptionOnStampContentHashIfCleanedContentIsInvalidAsync(
            string invalidCleanedContent)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 someEventV2 = CreateRandomEventV2();

            var invalidEventV2OrchestrationException =
                new InvalidEventV2OrchestrationException(
                    message: "Event is invalid, fix the errors and try again.");

            invalidEventV2OrchestrationException.UpsertDataList(
                key: "cleanedContent",
                value: "Required");

            var expectedEventV2OrchestrationValidationException =
                new EventV2OrchestrationValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV2OrchestrationException);

            this.eventV2ProcessingServiceMock.Setup(service =>
                service.RemoveVolatilePathsAsync(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(invalidCleanedContent);

            // when
            ValueTask<EventV2> stampContentHashTask =
                this.eventV2OrchestrationService.StampContentHashAsync(
                    someEventV2,
                    randomCancellationToken);

            EventV2OrchestrationValidationException
                actualEventV2OrchestrationValidationException =
                    await Assert.ThrowsAsync<EventV2OrchestrationValidationException>(
                        stampContentHashTask.AsTask);

            // then
            actualEventV2OrchestrationValidationException.Should()
                .BeEquivalentTo(expectedEventV2OrchestrationValidationException);

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

            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventAddressV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
