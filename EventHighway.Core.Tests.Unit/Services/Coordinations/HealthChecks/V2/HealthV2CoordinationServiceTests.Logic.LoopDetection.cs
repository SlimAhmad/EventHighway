// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.Healths;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.HealthChecks.V2
{
    public partial class HealthV2CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldIncludeLoopDetectionGroupingWithQuarantinedCountsInHealthSummaryAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            const int quarantinedEventCount = 2;
            const int quarantinedArchiveCount = 3;

            var randomEventAddressV2s = CreateRandomEventAddressV2s();
            var randomEventListenerV2s = CreateRandomEventListenerV2s();

            var randomEventV2s = CreateRandomEventV2s(
                immediateCount: 2,
                scheduledCount: 1,
                deadCount: 0,
                quarantinedCount: quarantinedEventCount);

            var randomListenerEventV2s = CreateRandomListenerEventV2s(
                successCount: 5,
                pendingCount: 0,
                errorCount: 0);

            var randomHandlers = CreateRandomEventHandlers(count: 1);

            var randomEventArchiveV2s = CreateRandomEventArchiveV2s(
                quarantinedCount: quarantinedArchiveCount);

            var randomListenerEventArchiveV2s = CreateRandomListenerEventArchiveV2s(
                successCount: 2,
                errorCount: 0);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventV2s);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventAddressV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventListenerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventListenerV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomListenerEventV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomHandlers);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventArchiveV2s);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomListenerEventArchiveV2s);

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.healthV2CoordinationService
                    .RetrieveHealthRagStatusV2Async(randomCancellationToken);

            // then
            actualResult.Single(i =>
                i.Grouping == "Loop Detection" && i.Item == "Quarantined Events")
                    .Value.Should().Be(quarantinedEventCount.ToString());

            actualResult.Single(i =>
                i.Grouping == "Loop Detection" && i.Item == "Quarantined Events")
                    .StatusCode.Should().Be((int)HealthStatusV2.Amber);

            actualResult.Single(i =>
                i.Grouping == "Loop Detection" && i.Item == "Quarantined Archives")
                    .Value.Should().Be(quarantinedArchiveCount.ToString());

            actualResult.Single(i =>
                i.Grouping == "Loop Detection" && i.Item == "Quarantined Archives")
                    .StatusCode.Should().Be((int)HealthStatusV2.NA);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventListenerV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetHealthConfiguration(),
                    Times.Once);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnRedForLoopsDetectedWhenQuarantinedEventCountIsSixOrMoreAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var randomEventAddressV2s = CreateRandomEventAddressV2s();
            var randomEventListenerV2s = CreateRandomEventListenerV2s();

            var randomEventV2s = CreateRandomEventV2s(
                immediateCount: 2,
                scheduledCount: 1,
                deadCount: 0,
                quarantinedCount: 6);

            var randomListenerEventV2s = CreateRandomListenerEventV2s(
                successCount: 5,
                pendingCount: 0,
                errorCount: 0);

            var randomHandlers = CreateRandomEventHandlers(count: 1);
            var randomEventArchiveV2s = CreateRandomEventArchiveV2s();
            var randomListenerEventArchiveV2s = CreateRandomListenerEventArchiveV2s(
                successCount: 2,
                errorCount: 0);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventV2s);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventAddressV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventListenerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventListenerV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomListenerEventV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomHandlers);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventArchiveV2s);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomListenerEventArchiveV2s);

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.healthV2CoordinationService
                    .RetrieveHealthRagStatusV2Async(randomCancellationToken);

            // then
            actualResult.Single(i =>
                i.Grouping == "Loop Detection" && i.Item == "Quarantined Events")
                    .StatusCode.Should().Be((int)HealthStatusV2.Red);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventListenerV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken), Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken), Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken), Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetHealthConfiguration(),
                    Times.Once);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnNAForLoopsDetectedWhenNoThresholdIsConfiguredAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var configWithoutLoopsDetected = new HealthConfiguration();

            configWithoutLoopsDetected.Thresholds.RemoveAll(
                t => t.Metric == HealthMetric.LoopsDetected);

            var randomEventAddressV2s = CreateRandomEventAddressV2s();
            var randomEventListenerV2s = CreateRandomEventListenerV2s();

            var randomEventV2s = CreateRandomEventV2s(
                immediateCount: 2,
                scheduledCount: 1,
                deadCount: 0,
                quarantinedCount: 3);

            var randomListenerEventV2s = CreateRandomListenerEventV2s(
                successCount: 5,
                pendingCount: 0,
                errorCount: 0);

            var randomHandlers = CreateRandomEventHandlers(count: 1);
            var randomEventArchiveV2s = CreateRandomEventArchiveV2s();

            var randomListenerEventArchiveV2s = CreateRandomListenerEventArchiveV2s(
                successCount: 2,
                errorCount: 0);

            this.configurationBrokerMock
                .Setup(broker => broker.GetHealthConfiguration())
                .Returns(configWithoutLoopsDetected);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventV2s);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventAddressV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventListenerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventListenerV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomListenerEventV2s);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomHandlers);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomEventArchiveV2s);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(randomListenerEventArchiveV2s);

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.healthV2CoordinationService
                    .RetrieveHealthRagStatusV2Async(randomCancellationToken);

            // then
            actualResult.Single(i =>
                i.Grouping == "Loop Detection" && i.Item == "Quarantined Events")
                    .StatusCode.Should().Be((int)HealthStatusV2.NA);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventListenerV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2sAsync(randomCancellationToken), Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken), Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken), Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetHealthConfiguration(),
                    Times.Once);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
