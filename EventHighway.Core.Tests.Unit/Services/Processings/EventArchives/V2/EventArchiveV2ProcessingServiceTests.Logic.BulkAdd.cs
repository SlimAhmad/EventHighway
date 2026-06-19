// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventArchives.V2
{
    public partial class EventArchiveV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldBulkAddEventArchiveV2sAsync()
        {
            // given
            List<EventArchiveV2> randomEventArchiveV2s =
                CreateRandomEventArchiveV2s().ToList();

            IEnumerable<EventArchiveV2> inputEventArchiveV2s = randomEventArchiveV2s;
            IEnumerable<EventArchiveV2> returnedEventArchiveV2s = randomEventArchiveV2s;
            IEnumerable<EventArchiveV2> expectedEventArchiveV2s = returnedEventArchiveV2s;

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.BulkAddEventArchiveV2sAsync(
                    inputEventArchiveV2s,
                    TestContext.Current.CancellationToken))
                        .ReturnsAsync(returnedEventArchiveV2s);

            // when
            IEnumerable<EventArchiveV2> actualEventArchiveV2s =
                await this.eventArchiveV2ProcessingService
                    .BulkAddEventArchiveV2sAsync(
                        inputEventArchiveV2s,
                        TestContext.Current.CancellationToken);

            // then
            actualEventArchiveV2s.Should().BeEquivalentTo(expectedEventArchiveV2s);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.BulkAddEventArchiveV2sAsync(
                    inputEventArchiveV2s,
                    TestContext.Current.CancellationToken),
                        Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
