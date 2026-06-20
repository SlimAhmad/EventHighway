// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventArchives.V2
{
    public partial class EventArchiveV2ServiceTests
    {
        [Fact]
        public async Task ShouldBulkRemoveEventArchiveV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<EventArchiveV2> randomEventArchiveV2s =
                CreateRandomEventArchiveV2s();

            IEnumerable<EventArchiveV2> inputEventArchiveV2s =
                randomEventArchiveV2s;

            this.storageBrokerMock.Setup(broker =>
                broker.BulkDeleteEventArchiveV2sAsync(
                    inputEventArchiveV2s,
                        randomCancellationToken))
                            .Returns(ValueTask.CompletedTask);

            // when
            await this.eventArchiveV2Service.BulkRemoveEventArchiveV2sAsync(
                inputEventArchiveV2s,
                    randomCancellationToken);

            // then
            this.storageBrokerMock.Verify(broker =>
                broker.BulkDeleteEventArchiveV2sAsync(
                    inputEventArchiveV2s,
                        randomCancellationToken),
                            Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
