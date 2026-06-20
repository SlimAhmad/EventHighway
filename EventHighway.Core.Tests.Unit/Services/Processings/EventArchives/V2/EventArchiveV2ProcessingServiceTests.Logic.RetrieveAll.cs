// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventArchives.V2
{
    public partial class EventArchiveV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventArchiveV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<EventArchiveV2> randomEventArchiveV2s = CreateRandomEventArchiveV2s();
            IQueryable<EventArchiveV2> retrievedEventArchiveV2s = randomEventArchiveV2s;
            IQueryable<EventArchiveV2> expectedEventArchiveV2s = randomEventArchiveV2s.DeepClone();

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(retrievedEventArchiveV2s);

            // when
            IQueryable<EventArchiveV2> actualEventArchiveV2s =
                await this.eventArchiveV2ProcessingService.RetrieveAllEventArchiveV2sAsync(randomCancellationToken);

            // then
            actualEventArchiveV2s.Should().BeEquivalentTo(expectedEventArchiveV2s);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
