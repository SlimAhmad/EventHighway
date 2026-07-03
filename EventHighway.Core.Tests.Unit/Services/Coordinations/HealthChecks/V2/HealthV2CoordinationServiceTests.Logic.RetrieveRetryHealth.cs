// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.HealthChecks.V2
{
    public partial class HealthV2CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveRetryHealthV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid addressAId = Guid.NewGuid();
            Guid addressBId = Guid.NewGuid();
            string addressAName = GetRandomString();
            string addressBName = GetRandomString();
            DateTimeOffset anyDate = GetRandomDateTimeOffset();

            var addresses = new[]
            {
                CreateEventAddressV2(addressAId, addressAName, GetRandomString()),
                CreateEventAddressV2(addressBId, addressBName, GetRandomString())
            }.AsQueryable();

            var events = new[]
            {
                CreateEventV2ForAddress(addressAId, anyDate, "h1", 0, EventStatusV2.Active),
                CreateEventV2ForAddress(addressAId, anyDate, "h2", 0, EventStatusV2.Active),
                CreateEventV2ForAddress(addressAId, anyDate, "h3", 1, EventStatusV2.Active),
                CreateEventV2ForAddress(addressAId, anyDate, "h4", 5, EventStatusV2.Active),
                CreateEventV2ForAddress(addressBId, anyDate, "h5", 2, EventStatusV2.Active),
                CreateEventV2ForAddress(addressBId, anyDate, "h6", 4, EventStatusV2.Active),
                CreateEventV2ForAddress(addressAId, anyDate, "h7", 3, EventStatusV2.Quarantined)
            }.AsQueryable();

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken))
                    .ReturnsAsync(addresses);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(events);

            // when
            RetryHealthSummaryV2 actualSummary =
                await this.healthV2CoordinationService
                    .RetrieveRetryHealthV2Async(randomCancellationToken);

            // then
            actualSummary.TotalActiveEvents.Should().Be(6);
            actualSummary.DeadEvents.Should().Be(2);
            actualSummary.CriticalEvents.Should().Be(2);
            actualSummary.HealthyEvents.Should().Be(2);

            actualSummary.Distribution.Should().HaveCount(5);
            actualSummary.Distribution.First().RemainingRetries.Should().Be(0);
            actualSummary.Distribution.First().Count.Should().Be(2);
            actualSummary.Distribution.Single(b => b.RemainingRetries == 5).Count.Should().Be(1);

            actualSummary.ByAddress.Should().HaveCount(2);
            actualSummary.ByAddress.First().EventAddressV2Id.Should().Be(addressAId);

            RetryAddressDetailV2 detailA =
                actualSummary.ByAddress.Single(d => d.EventAddressV2Id == addressAId);

            detailA.EventAddressV2Name.Should().Be(addressAName);
            detailA.DeadEvents.Should().Be(2);
            detailA.CriticalEvents.Should().Be(1);
            detailA.TotalEvents.Should().Be(4);

            RetryAddressDetailV2 detailB =
                actualSummary.ByAddress.Single(d => d.EventAddressV2Id == addressBId);

            detailB.DeadEvents.Should().Be(0);
            detailB.CriticalEvents.Should().Be(1);
            detailB.TotalEvents.Should().Be(2);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken), Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnEmptyRetryHealthV2WhenNoActiveEventsExistAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid addressAId = Guid.NewGuid();
            DateTimeOffset anyDate = GetRandomDateTimeOffset();

            var addresses = new[]
            {
                CreateEventAddressV2(addressAId, GetRandomString(), GetRandomString())
            }.AsQueryable();

            var events = new[]
            {
                CreateEventV2ForAddress(addressAId, anyDate, "h1", 1, EventStatusV2.Quarantined)
            }.AsQueryable();

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken))
                    .ReturnsAsync(addresses);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(events);

            // when
            RetryHealthSummaryV2 actualSummary =
                await this.healthV2CoordinationService
                    .RetrieveRetryHealthV2Async(randomCancellationToken);

            // then
            actualSummary.TotalActiveEvents.Should().Be(0);
            actualSummary.DeadEvents.Should().Be(0);
            actualSummary.CriticalEvents.Should().Be(0);
            actualSummary.HealthyEvents.Should().Be(0);
            actualSummary.Distribution.Should().BeEmpty();
            actualSummary.ByAddress.Should().BeEmpty();

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken), Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
