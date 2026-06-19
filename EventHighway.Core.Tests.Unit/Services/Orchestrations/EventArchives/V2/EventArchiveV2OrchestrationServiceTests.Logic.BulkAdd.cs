// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V2
{
    public partial class EventArchiveV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldBulkAddEventArchiveV2sAsync()
        {
            // given
            IQueryable<EventArchiveV2> randomEventArchiveV2s = CreateRandomEventArchiveV2s();
            IEnumerable<EventArchiveV2> inputEventArchiveV2s = randomEventArchiveV2s;
            IEnumerable<EventArchiveV2> addedEventArchiveV2s = inputEventArchiveV2s;

            this.eventArchiveV2ProcessingServiceMock.Setup(service =>
                service.BulkAddEventArchiveV2sAsync(
                    inputEventArchiveV2s,
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(addedEventArchiveV2s);

            // when
            IEnumerable<EventArchiveV2> actualAddedEventArchiveV2s =
                await this.eventArchiveV2OrchestrationService.BulkAddEventArchiveV2sAsync(
                    inputEventArchiveV2s,
                    TestContext.Current.CancellationToken);

            // then
            actualAddedEventArchiveV2s.Should().BeEquivalentTo(addedEventArchiveV2s);

            this.eventArchiveV2ProcessingServiceMock.Verify(service =>
                service.BulkAddEventArchiveV2sAsync(
                    inputEventArchiveV2s,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
