// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Portal.Web.Models.Views.ListenerEvents;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.ListenerEvents
{
    public partial class ListenerEventsViewServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllListenerEventsMostRecentFirstAsync()
        {
            // given
            DateTimeOffset baseDate = GetRandomDateTimeOffset();

            ListenerEventV2 oldest = CreateRandomListenerEvent(baseDate.AddDays(-2));
            ListenerEventV2 middle = CreateRandomListenerEvent(baseDate.AddDays(-1));
            ListenerEventV2 newest = CreateRandomListenerEvent(baseDate);

            IQueryable<ListenerEventV2> storageListenerEvents =
                new[] { oldest, newest, middle }.AsQueryable();

            List<ListenerEventView> expectedViews =
                new[] { newest, middle, oldest }.Select(MapToView).ToList();

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(storageListenerEvents);

            // when
            List<ListenerEventView> actualViews =
                await this.listenerEventsViewService.RetrieveAllListenerEventsAsync(
                    TestContext.Current.CancellationToken);

            // then
            actualViews.Should().BeEquivalentTo(expectedViews, options => options.WithStrictOrdering());

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRemoveListenerEventByIdAsync()
        {
            // given
            ListenerEventV2 removedListenerEvent =
                CreateRandomListenerEvent(GetRandomDateTimeOffset());

            Guid listenerEventId = removedListenerEvent.Id;

            ListenerEventView expectedView = MapToView(removedListenerEvent);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RemoveListenerEventV2ByIdAsync(
                    listenerEventId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(removedListenerEvent);

            // when
            ListenerEventView actualView =
                await this.listenerEventsViewService.RemoveListenerEventByIdAsync(
                    listenerEventId, TestContext.Current.CancellationToken);

            // then
            actualView.Should().BeEquivalentTo(expectedView);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RemoveListenerEventV2ByIdAsync(
                    listenerEventId, It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldPurgeListenerEventsOlderThanAsync()
        {
            // given
            DateTimeOffset threshold = GetRandomDateTimeOffset();

            ListenerEventV2 staleOne = CreateRandomListenerEvent(threshold.AddDays(-3));
            ListenerEventV2 staleTwo = CreateRandomListenerEvent(threshold.AddDays(-1));
            ListenerEventV2 recent = CreateRandomListenerEvent(threshold.AddDays(1));

            IQueryable<ListenerEventV2> storageListenerEvents =
                new[] { staleOne, recent, staleTwo }.AsQueryable();

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(storageListenerEvents);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RemoveListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync((Guid id, CancellationToken _) =>
                            CreateRandomListenerEvent(threshold));

            // when
            int purgedCount =
                await this.listenerEventsViewService.PurgeListenerEventsOlderThanAsync(
                    threshold, TestContext.Current.CancellationToken);

            // then
            purgedCount.Should().Be(2);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RemoveListenerEventV2ByIdAsync(
                    staleOne.Id, It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RemoveListenerEventV2ByIdAsync(
                    staleTwo.Id, It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RemoveListenerEventV2ByIdAsync(
                    recent.Id, It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
