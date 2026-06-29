// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
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
    }
}
