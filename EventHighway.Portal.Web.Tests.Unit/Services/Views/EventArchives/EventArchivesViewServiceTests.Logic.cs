// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Portal.Web.Models.Views.EventArchives;
using FluentAssertions;
using Moq;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.EventArchives
{
    public partial class EventArchivesViewServiceTests
    {
        [Fact]
        public async Task ShouldArchiveProcessedEventsAsync()
        {
            // given . when
            await this.eventArchivesViewService.ArchiveProcessedEventsAsync(
                TestContext.Current.CancellationToken);

            // then
            this.eventHighwayBrokerMock.Verify(broker =>
                broker.ArchiveEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldPurgeArchivesOlderThanAsync()
        {
            // given
            DateTimeOffset olderThan = GetRandomDateTimeOffset();

            // when
            await this.eventArchivesViewService.PurgeArchivesOlderThanAsync(
                olderThan, TestContext.Current.CancellationToken);

            // then
            this.eventHighwayBrokerMock.Verify(broker =>
                broker.PurgeEventArchiveV2sAsync(
                    olderThan, It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveAllEventArchivesMostRecentFirstAsync()
        {
            // given
            DateTimeOffset baseDate = GetRandomDateTimeOffset();

            EventArchiveV2 oldest = CreateRandomEventArchive(baseDate.AddDays(-2));
            EventArchiveV2 middle = CreateRandomEventArchive(baseDate.AddDays(-1));
            EventArchiveV2 newest = CreateRandomEventArchive(baseDate);

            IQueryable<EventArchiveV2> storageEventArchives =
                new[] { oldest, newest, middle }.AsQueryable();

            List<EventArchiveView> expectedViews =
                new[] { newest, middle, oldest }.Select(MapToView).ToList();

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllEventArchiveV2sWithEventAddressV2Async(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(storageEventArchives);

            // when
            List<EventArchiveView> actualViews =
                await this.eventArchivesViewService.RetrieveAllEventArchivesAsync(
                    TestContext.Current.CancellationToken);

            // then
            actualViews.Should().BeEquivalentTo(
                expectedViews, options => options.WithStrictOrdering());

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllEventArchiveV2sWithEventAddressV2Async(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveEventArchiveByIdAsync()
        {
            // given
            DateTimeOffset baseDate = GetRandomDateTimeOffset();

            EventArchiveV2 targetEventArchive = CreateRandomEventArchive(baseDate);
            Guid eventArchiveId = targetEventArchive.Id;

            IQueryable<EventArchiveV2> storageEventArchives = new[]
            {
                CreateRandomEventArchive(baseDate.AddDays(-1)),
                targetEventArchive,
                CreateRandomEventArchive(baseDate.AddDays(-2))
            }.AsQueryable();

            EventArchiveView expectedView = MapToView(targetEventArchive);

            this.eventHighwayBrokerMock.Setup(broker =>
                broker.RetrieveAllEventArchiveV2sWithEventAddressV2Async(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(storageEventArchives);

            // when
            EventArchiveView actualView =
                await this.eventArchivesViewService.RetrieveEventArchiveByIdAsync(
                    eventArchiveId, TestContext.Current.CancellationToken);

            // then
            actualView.Should().BeEquivalentTo(expectedView);

            this.eventHighwayBrokerMock.Verify(broker =>
                broker.RetrieveAllEventArchiveV2sWithEventAddressV2Async(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventHighwayBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
