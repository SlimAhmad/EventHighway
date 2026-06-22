// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.LoopDetections;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.Events.V2
{
    public partial class EventV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldReturnFalseOnIsLoopDetectedIfCountIsAtOrBelowThresholdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;
            int randomThreshold = GetRandomNumber();
            int atThresholdCount = randomThreshold;

            var loopDetectionConfig = new LoopDetection
            {
                Enabled = true,
                Threshold = randomThreshold
            };

            this.configurationBrokerMock
                .Setup(broker => broker.GetLoopDetectionConfiguration())
                    .Returns(loopDetectionConfig);

            this.eventV2ServiceMock
                .Setup(service => service.RetrieveEventV2CountBySignatureAsync(
                    inputEventV2,
                    randomCancellationToken))
                        .ReturnsAsync(atThresholdCount);

            // when
            bool actualIsLoopDetected =
                await this.eventV2ProcessingService
                    .IsLoopDetectedAsync(
                        inputEventV2,
                        randomCancellationToken);

            // then
            actualIsLoopDetected.Should().BeFalse();

            this.configurationBrokerMock.Verify(broker =>
                broker.GetLoopDetectionConfiguration(),
                    Times.Once);

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveEventV2CountBySignatureAsync(
                    inputEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnFalseOnIsLoopDetectedIfLoopDetectionIsDisabledAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;

            var loopDetectionConfig = new LoopDetection
            {
                Enabled = false,
                Threshold = GetRandomNumber()
            };

            this.configurationBrokerMock
                .Setup(broker => broker.GetLoopDetectionConfiguration())
                    .Returns(loopDetectionConfig);

            // when
            bool actualIsLoopDetected =
                await this.eventV2ProcessingService
                    .IsLoopDetectedAsync(
                        inputEventV2,
                        randomCancellationToken);

            // then
            actualIsLoopDetected.Should().BeFalse();

            this.configurationBrokerMock.Verify(broker =>
                broker.GetLoopDetectionConfiguration(),
                    Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
