// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.HealthChecks.V2
{
    public partial class HealthV2CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveParticipantSummaryV2ForWindowAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var windowStart = new DateTimeOffset(2026, 6, 24, 0, 0, 0, TimeSpan.Zero);
            var expectedWindowEnd = new DateTimeOffset(2026, 6, 25, 0, 0, 0, TimeSpan.Zero);

            Guid p1Id = Guid.NewGuid();
            Guid p2Id = Guid.NewGuid();
            string p1Name = "Alpha";
            string p1Email = GetRandomString();
            string p1Phone = GetRandomString();

            EventParticipantV2 p1 = CreateEventParticipantV2(p1Id, p1Name, p1Email, p1Phone, isActive: true);
            EventParticipantV2 p2 = CreateEventParticipantV2(p2Id, "Beta", GetRandomString(), GetRandomString(), isActive: false);

            Guid addr1Id = Guid.NewGuid();
            Guid addr2Id = Guid.NewGuid();
            string addr1Name = "addr-one";
            string addr2Name = "addr-two";

            var addresses = new[]
            {
                CreateEventAddressV2(addr1Id, addr1Name, GetRandomString()),
                CreateEventAddressV2(addr2Id, addr2Name, GetRandomString())
            }.AsQueryable();

            Guid ev1 = Guid.NewGuid();
            Guid ev2 = Guid.NewGuid();
            Guid ev3 = Guid.NewGuid();
            Guid ev4 = Guid.NewGuid();
            Guid ev5 = Guid.NewGuid();

            var events = new[]
            {
                CreateEventV2ForParticipant(ev1, addr1Id, p1Id, p1, windowStart.AddHours(1), "h1", EventStatusV2.Active),
                CreateEventV2ForParticipant(ev2, addr1Id, p1Id, p1, windowStart.AddHours(2), "h1", EventStatusV2.Active),
                CreateEventV2ForParticipant(ev3, addr2Id, p1Id, p1, windowStart.AddHours(3), "h2", EventStatusV2.Quarantined),
                CreateEventV2ForParticipant(ev4, addr1Id, p2Id, p2, windowStart.AddHours(4), "h3", EventStatusV2.Active),
                CreateEventV2ForParticipant(ev5, addr1Id, null, null, windowStart.AddHours(5), "h4", EventStatusV2.Active),
                CreateEventV2ForParticipant(Guid.NewGuid(), addr1Id, p1Id, p1, windowStart.AddDays(-1), "h5", EventStatusV2.Active)
            }.AsQueryable();

            Guid li1 = Guid.NewGuid();
            Guid li2 = Guid.NewGuid();
            Guid li3 = Guid.NewGuid();
            Guid li4 = Guid.NewGuid();

            var listeners = new[]
            {
                CreateEventListenerV2ForParticipant(li1, addr1Id, p1Id, p1),
                CreateEventListenerV2ForParticipant(li2, addr2Id, p1Id, p1),
                CreateEventListenerV2ForParticipant(li3, addr1Id, p2Id, p2),
                CreateEventListenerV2ForParticipant(li4, addr1Id, null, null)
            }.AsQueryable();

            var listenerEvents = new[]
            {
                CreateListenerEventV2ForEventAndListener(ev1, li1, addr1Id, windowStart.AddHours(1), ListenerEventStatusV2.Success),
                CreateListenerEventV2ForEventAndListener(ev1, li3, addr1Id, windowStart.AddHours(2), ListenerEventStatusV2.Error),
                CreateListenerEventV2ForEventAndListener(ev4, li1, addr1Id, windowStart.AddHours(3), ListenerEventStatusV2.Error),
                CreateListenerEventV2ForEventAndListener(ev4, li3, addr1Id, windowStart.AddHours(4), ListenerEventStatusV2.Success)
            }.AsQueryable();

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken))
                    .ReturnsAsync(addresses);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(events);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventListenerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(listeners);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(listenerEvents);

            // when
            var actualSummaries =
                (await this.healthV2CoordinationService
                    .RetrieveParticipantSummaryV2Async(
                        TrafficPeriodV2.Day, windowStart, randomCancellationToken))
                    .ToList();

            // then
            actualSummaries.Should().HaveCount(3);
            actualSummaries.First().EventParticipantV2Id.Should().Be(p1Id);

            ParticipantSummaryV2 summaryP1 =
                actualSummaries.Single(s => s.EventParticipantV2Id == p1Id);

            summaryP1.Name.Should().Be(p1Name);
            summaryP1.ContactEmail.Should().Be(p1Email);
            summaryP1.ContactPhone.Should().Be(p1Phone);
            summaryP1.IsActive.Should().BeTrue();
            summaryP1.Period.Should().Be(TrafficPeriodV2.Day);
            summaryP1.WindowEnd.Should().Be(expectedWindowEnd);
            summaryP1.WindowLabel.Should().Be("24 Jun 2026");
            summaryP1.TotalEventsSubmitted.Should().Be(3);
            summaryP1.ActiveEventAddresses.Should().Be(2);
            summaryP1.ActiveEventAddressNames.Should().BeEquivalentTo(new[] { addr1Name, addr2Name });
            summaryP1.OwnedListeners.Should().Be(2);
            summaryP1.TotalListenerEvents.Should().Be(2);
            summaryP1.PublisherErrorRate.Should().Be(50m);
            summaryP1.ListenerErrorRate.Should().Be(50m);
            summaryP1.LoopsDetected.Should().Be(1);
            summaryP1.DuplicatesDetected.Should().Be(1);
            summaryP1.Status.Should().Be(HealthStatusV2.Red);
            summaryP1.LastActivity.Should().Be(windowStart.AddHours(3));

            ParticipantSummaryV2 summaryP2 =
                actualSummaries.Single(s => s.EventParticipantV2Id == p2Id);

            summaryP2.IsActive.Should().BeFalse();
            summaryP2.TotalEventsSubmitted.Should().Be(1);
            summaryP2.OwnedListeners.Should().Be(1);

            ParticipantSummaryV2 summaryUnknown =
                actualSummaries.Single(s => s.EventParticipantV2Id == Guid.Empty);

            summaryUnknown.Name.Should().Be("Unknown");
            summaryUnknown.ContactEmail.Should().BeNull();
            summaryUnknown.IsActive.Should().BeFalse();
            summaryUnknown.TotalEventsSubmitted.Should().Be(1);
            summaryUnknown.OwnedListeners.Should().Be(1);
            summaryUnknown.TotalListenerEvents.Should().Be(0);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken), Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventListenerV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken), Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetHealthConfiguration(), Times.Once);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReturnEmptyParticipantSummaryV2WhenNoEventsOrListenersExistAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var windowStart = new DateTimeOffset(2026, 6, 24, 0, 0, 0, TimeSpan.Zero);
            var emptyAddresses = Array.Empty<EventAddressV2>().AsQueryable();
            var emptyEvents = Array.Empty<EventV2>().AsQueryable();
            var emptyListeners = Array.Empty<EventListenerV2>().AsQueryable();
            var emptyListenerEvents = Array.Empty<ListenerEventV2>().AsQueryable();

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken))
                    .ReturnsAsync(emptyAddresses);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(emptyEvents);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventListenerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(emptyListeners);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(emptyListenerEvents);

            // when
            var actualSummaries =
                await this.healthV2CoordinationService
                    .RetrieveParticipantSummaryV2Async(
                        TrafficPeriodV2.Day, windowStart, randomCancellationToken);

            // then
            actualSummaries.Should().BeEmpty();

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken), Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventListenerV2sAsync(randomCancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken), Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetHealthConfiguration(), Times.Once);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
