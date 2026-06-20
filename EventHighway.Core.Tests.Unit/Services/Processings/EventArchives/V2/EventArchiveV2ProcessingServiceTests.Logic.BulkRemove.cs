// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventArchives.V2
{
    public partial class EventArchiveV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldBulkRemoveEventArchiveV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<EventArchiveV2> randomEventArchiveV2s =
                CreateRandomEventArchiveV2s().ToList();

            IEnumerable<EventArchiveV2> inputEventArchiveV2s = randomEventArchiveV2s;

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.BulkRemoveEventArchiveV2sAsync(
                    inputEventArchiveV2s,
                    randomCancellationToken))
                        .Returns(new ValueTask());

            // when
            await this.eventArchiveV2ProcessingService
                .BulkRemoveEventArchiveV2sAsync(
                    inputEventArchiveV2s,
                    randomCancellationToken);

            // then
            this.eventArchiveV2ServiceMock.Verify(service =>
                service.BulkRemoveEventArchiveV2sAsync(
                    inputEventArchiveV2s,
                    randomCancellationToken),
                        Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
