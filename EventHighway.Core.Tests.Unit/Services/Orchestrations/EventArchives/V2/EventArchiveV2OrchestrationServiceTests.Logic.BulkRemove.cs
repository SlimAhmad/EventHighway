// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V2
{
    public partial class EventArchiveV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldBulkRemoveEventArchiveV2sAsync()
        {
            // given
            IQueryable<EventArchiveV2> randomEventArchiveV2s = CreateRandomEventArchiveV2s();
            IEnumerable<EventArchiveV2> inputEventArchiveV2s = randomEventArchiveV2s;

            this.eventArchiveV2ProcessingServiceMock.Setup(service =>
                service.BulkRemoveEventArchiveV2sAsync(
                    inputEventArchiveV2s,
                    TestContext.Current.CancellationToken))
                        .Returns(new ValueTask());

            // when
            await this.eventArchiveV2OrchestrationService
                .BulkRemoveEventArchiveV2sAsync(
                    inputEventArchiveV2s,
                    TestContext.Current.CancellationToken);

            // then
            this.eventArchiveV2ProcessingServiceMock.Verify(service =>
                service.BulkRemoveEventArchiveV2sAsync(
                    inputEventArchiveV2s,
                    TestContext.Current.CancellationToken),
                        Times.Once);

            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
