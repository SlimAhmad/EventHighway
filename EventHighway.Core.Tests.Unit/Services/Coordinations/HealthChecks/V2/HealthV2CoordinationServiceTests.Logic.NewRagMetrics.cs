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

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.HealthChecks.V2
{
    public partial class HealthV2CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldReturnAmberForPendingBacklogWhenBetweenGreenAndRedAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            SetupHealthOrchestrationMocks(
                randomCancellationToken,
                CreateRandomEventV2s(immediateCount: 2, scheduledCount: 1, deadCount: 0),
                CreateRandomEventAddressV2s(),
                CreateRandomEventListenerV2s(),
                CreateRandomListenerEventV2s(successCount: 9, pendingCount: 100, errorCount: 0),
                CreateRandomEventHandlers(count: 1),
                CreateRandomEventArchiveV2s(),
                CreateRandomListenerEventArchiveV2s(successCount: 9, errorCount: 0));

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.healthV2CoordinationService
                    .RetrieveHealthSummaryV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Listener Events" && i.Item == "Pending Listener Events")
                .StatusCode.Should().Be((int)HealthStatusV2.Amber);

            VerifyHealthOrchestrationMocksOnce(randomCancellationToken);
        }

        [Fact]
        public async Task ShouldReturnRedForPendingBacklogWhenAtOrAboveRedAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            SetupHealthOrchestrationMocks(
                randomCancellationToken,
                CreateRandomEventV2s(immediateCount: 2, scheduledCount: 1, deadCount: 0),
                CreateRandomEventAddressV2s(),
                CreateRandomEventListenerV2s(),
                CreateRandomListenerEventV2s(successCount: 9, pendingCount: 500, errorCount: 0),
                CreateRandomEventHandlers(count: 1),
                CreateRandomEventArchiveV2s(),
                CreateRandomListenerEventArchiveV2s(successCount: 9, errorCount: 0));

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.healthV2CoordinationService
                    .RetrieveHealthSummaryV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Listener Events" && i.Item == "Pending Listener Events")
                .StatusCode.Should().Be((int)HealthStatusV2.Red);

            VerifyHealthOrchestrationMocksOnce(randomCancellationToken);
        }

        [Fact]
        public async Task ShouldReturnNAForPendingBacklogWhenNoThresholdIsConfiguredAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var configWithoutPendingBacklog = new HealthConfiguration();
            configWithoutPendingBacklog.Thresholds.RemoveAll(
                t => t.Metric == HealthMetric.PendingBacklog);

            this.configurationBrokerMock
                .Setup(broker => broker.GetHealthConfiguration())
                .Returns(configWithoutPendingBacklog);

            SetupHealthOrchestrationMocks(
                randomCancellationToken,
                CreateRandomEventV2s(immediateCount: 2, scheduledCount: 1, deadCount: 0),
                CreateRandomEventAddressV2s(),
                CreateRandomEventListenerV2s(),
                CreateRandomListenerEventV2s(successCount: 9, pendingCount: 100, errorCount: 0),
                CreateRandomEventHandlers(count: 1),
                CreateRandomEventArchiveV2s(),
                CreateRandomListenerEventArchiveV2s(successCount: 9, errorCount: 0));

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.healthV2CoordinationService
                    .RetrieveHealthSummaryV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Listener Events" && i.Item == "Pending Listener Events")
                .StatusCode.Should().Be((int)HealthStatusV2.NA);

            VerifyHealthOrchestrationMocksOnce(randomCancellationToken);
        }

        [Fact]
        public async Task ShouldReturnAmberForReplayRateWhenBetweenGreenAndRedAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            SetupHealthOrchestrationMocks(
                randomCancellationToken,
                CreateRandomEventV2s(immediateCount: 2, scheduledCount: 1, deadCount: 0),
                CreateRandomEventAddressV2s(),
                CreateRandomEventListenerV2s(),
                CreateRandomListenerEventV2s(successCount: 9, pendingCount: 0, errorCount: 0, replayCount: 1),
                CreateRandomEventHandlers(count: 1),
                CreateRandomEventArchiveV2s(),
                CreateRandomListenerEventArchiveV2s(successCount: 9, errorCount: 0));

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.healthV2CoordinationService
                    .RetrieveHealthSummaryV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Listener Events" && i.Item == "Replay Rate %")
                .StatusCode.Should().Be((int)HealthStatusV2.Amber);

            VerifyHealthOrchestrationMocksOnce(randomCancellationToken);
        }

        [Fact]
        public async Task ShouldReturnRedForReplayRateWhenAtOrAboveRedAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            SetupHealthOrchestrationMocks(
                randomCancellationToken,
                CreateRandomEventV2s(immediateCount: 2, scheduledCount: 1, deadCount: 0),
                CreateRandomEventAddressV2s(),
                CreateRandomEventListenerV2s(),
                CreateRandomListenerEventV2s(successCount: 7, pendingCount: 0, errorCount: 0, replayCount: 3),
                CreateRandomEventHandlers(count: 1),
                CreateRandomEventArchiveV2s(),
                CreateRandomListenerEventArchiveV2s(successCount: 9, errorCount: 0));

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.healthV2CoordinationService
                    .RetrieveHealthSummaryV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Listener Events" && i.Item == "Replay Rate %")
                .StatusCode.Should().Be((int)HealthStatusV2.Red);

            VerifyHealthOrchestrationMocksOnce(randomCancellationToken);
        }

        [Fact]
        public async Task ShouldReturnNAForReplayRateWhenNoThresholdIsConfiguredAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var configWithoutReplayRate = new HealthConfiguration();
            configWithoutReplayRate.Thresholds.RemoveAll(
                t => t.Metric == HealthMetric.ReplayRate);

            this.configurationBrokerMock
                .Setup(broker => broker.GetHealthConfiguration())
                .Returns(configWithoutReplayRate);

            SetupHealthOrchestrationMocks(
                randomCancellationToken,
                CreateRandomEventV2s(immediateCount: 2, scheduledCount: 1, deadCount: 0),
                CreateRandomEventAddressV2s(),
                CreateRandomEventListenerV2s(),
                CreateRandomListenerEventV2s(successCount: 7, pendingCount: 0, errorCount: 0, replayCount: 3),
                CreateRandomEventHandlers(count: 1),
                CreateRandomEventArchiveV2s(),
                CreateRandomListenerEventArchiveV2s(successCount: 9, errorCount: 0));

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.healthV2CoordinationService
                    .RetrieveHealthSummaryV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Listener Events" && i.Item == "Replay Rate %")
                .StatusCode.Should().Be((int)HealthStatusV2.NA);

            VerifyHealthOrchestrationMocksOnce(randomCancellationToken);
        }

        [Fact]
        public async Task ShouldReturnAmberForArchiveErrorRateWhenBetweenGreenAndRedAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            SetupHealthOrchestrationMocks(
                randomCancellationToken,
                CreateRandomEventV2s(immediateCount: 2, scheduledCount: 1, deadCount: 0),
                CreateRandomEventAddressV2s(),
                CreateRandomEventListenerV2s(),
                CreateRandomListenerEventV2s(successCount: 9, pendingCount: 0, errorCount: 0),
                CreateRandomEventHandlers(count: 1),
                CreateRandomEventArchiveV2s(),
                CreateRandomListenerEventArchiveV2s(successCount: 8, errorCount: 2));

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.healthV2CoordinationService
                    .RetrieveHealthSummaryV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Event Archives" && i.Item == "Archive Error Rate %")
                .StatusCode.Should().Be((int)HealthStatusV2.Amber);

            VerifyHealthOrchestrationMocksOnce(randomCancellationToken);
        }

        [Fact]
        public async Task ShouldReturnRedForArchiveErrorRateWhenAtOrAboveRedAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            SetupHealthOrchestrationMocks(
                randomCancellationToken,
                CreateRandomEventV2s(immediateCount: 2, scheduledCount: 1, deadCount: 0),
                CreateRandomEventAddressV2s(),
                CreateRandomEventListenerV2s(),
                CreateRandomListenerEventV2s(successCount: 9, pendingCount: 0, errorCount: 0),
                CreateRandomEventHandlers(count: 1),
                CreateRandomEventArchiveV2s(),
                CreateRandomListenerEventArchiveV2s(successCount: 7, errorCount: 3));

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.healthV2CoordinationService
                    .RetrieveHealthSummaryV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Event Archives" && i.Item == "Archive Error Rate %")
                .StatusCode.Should().Be((int)HealthStatusV2.Red);

            VerifyHealthOrchestrationMocksOnce(randomCancellationToken);
        }

        [Fact]
        public async Task ShouldReturnNAForArchiveErrorRateWhenNoThresholdIsConfiguredAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var configWithoutArchiveErrorRate = new HealthConfiguration();
            configWithoutArchiveErrorRate.Thresholds.RemoveAll(
                t => t.Metric == HealthMetric.ArchiveErrorRate);

            this.configurationBrokerMock
                .Setup(broker => broker.GetHealthConfiguration())
                .Returns(configWithoutArchiveErrorRate);

            SetupHealthOrchestrationMocks(
                randomCancellationToken,
                CreateRandomEventV2s(immediateCount: 2, scheduledCount: 1, deadCount: 0),
                CreateRandomEventAddressV2s(),
                CreateRandomEventListenerV2s(),
                CreateRandomListenerEventV2s(successCount: 9, pendingCount: 0, errorCount: 0),
                CreateRandomEventHandlers(count: 1),
                CreateRandomEventArchiveV2s(),
                CreateRandomListenerEventArchiveV2s(successCount: 7, errorCount: 3));

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.healthV2CoordinationService
                    .RetrieveHealthSummaryV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Event Archives" && i.Item == "Archive Error Rate %")
                .StatusCode.Should().Be((int)HealthStatusV2.NA);

            VerifyHealthOrchestrationMocksOnce(randomCancellationToken);
        }

        [Fact]
        public async Task ShouldReturnAmberForDeadArchivedEventsWhenBetweenGreenAndRedAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            SetupHealthOrchestrationMocks(
                randomCancellationToken,
                CreateRandomEventV2s(immediateCount: 2, scheduledCount: 1, deadCount: 0),
                CreateRandomEventAddressV2s(),
                CreateRandomEventListenerV2s(),
                CreateRandomListenerEventV2s(successCount: 9, pendingCount: 0, errorCount: 0),
                CreateRandomEventHandlers(count: 1),
                CreateRandomEventArchiveV2s(count: 2, quarantinedCount: 0, deadCount: 3),
                CreateRandomListenerEventArchiveV2s(successCount: 9, errorCount: 0));

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.healthV2CoordinationService
                    .RetrieveHealthSummaryV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Event Archives" && i.Item == "Dead Archived Events")
                .StatusCode.Should().Be((int)HealthStatusV2.Amber);

            VerifyHealthOrchestrationMocksOnce(randomCancellationToken);
        }

        [Fact]
        public async Task ShouldReturnRedForDeadArchivedEventsWhenAtOrAboveRedAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            SetupHealthOrchestrationMocks(
                randomCancellationToken,
                CreateRandomEventV2s(immediateCount: 2, scheduledCount: 1, deadCount: 0),
                CreateRandomEventAddressV2s(),
                CreateRandomEventListenerV2s(),
                CreateRandomListenerEventV2s(successCount: 9, pendingCount: 0, errorCount: 0),
                CreateRandomEventHandlers(count: 1),
                CreateRandomEventArchiveV2s(count: 2, quarantinedCount: 0, deadCount: 6),
                CreateRandomListenerEventArchiveV2s(successCount: 9, errorCount: 0));

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.healthV2CoordinationService
                    .RetrieveHealthSummaryV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Event Archives" && i.Item == "Dead Archived Events")
                .StatusCode.Should().Be((int)HealthStatusV2.Red);

            VerifyHealthOrchestrationMocksOnce(randomCancellationToken);
        }

        [Fact]
        public async Task ShouldReturnNAForDeadArchivedEventsWhenNoThresholdIsConfiguredAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var configWithoutDeadArchivedEvents = new HealthConfiguration();
            configWithoutDeadArchivedEvents.Thresholds.RemoveAll(
                t => t.Metric == HealthMetric.DeadArchivedEvents);

            this.configurationBrokerMock
                .Setup(broker => broker.GetHealthConfiguration())
                .Returns(configWithoutDeadArchivedEvents);

            SetupHealthOrchestrationMocks(
                randomCancellationToken,
                CreateRandomEventV2s(immediateCount: 2, scheduledCount: 1, deadCount: 0),
                CreateRandomEventAddressV2s(),
                CreateRandomEventListenerV2s(),
                CreateRandomListenerEventV2s(successCount: 9, pendingCount: 0, errorCount: 0),
                CreateRandomEventHandlers(count: 1),
                CreateRandomEventArchiveV2s(count: 2, quarantinedCount: 0, deadCount: 3),
                CreateRandomListenerEventArchiveV2s(successCount: 9, errorCount: 0));

            // when
            IEnumerable<HealthCheckItemV2> actualResult =
                await this.healthV2CoordinationService
                    .RetrieveHealthSummaryV2Async(randomCancellationToken);

            // then
            actualResult.Single(i => i.Grouping == "Event Archives" && i.Item == "Dead Archived Events")
                .StatusCode.Should().Be((int)HealthStatusV2.NA);

            VerifyHealthOrchestrationMocksOnce(randomCancellationToken);
        }
    }
}
