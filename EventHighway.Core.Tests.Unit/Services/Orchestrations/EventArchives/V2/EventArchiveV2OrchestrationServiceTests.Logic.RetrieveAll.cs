// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V2
{
    public partial class EventArchiveV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventArchiveV2sAsync()
        {
            // given
            IQueryable<EventArchiveV2> randomEventArchiveV2s = CreateRandomEventArchiveV2s();
            IQueryable<EventArchiveV2> retrievedEventArchiveV2s = randomEventArchiveV2s;
            IQueryable<EventArchiveV2> expectedEventArchiveV2s = randomEventArchiveV2s.DeepClone();

            this.eventArchiveV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync())
                    .ReturnsAsync(retrievedEventArchiveV2s);

            // when
            IQueryable<EventArchiveV2> actualEventArchiveV2s =
                await this.eventArchiveV2OrchestrationService
                    .RetrieveAllEventArchiveV2sAsync(
                        TestContext.Current.CancellationToken);

            // then
            actualEventArchiveV2s.Should().BeEquivalentTo(expectedEventArchiveV2s);

            this.eventArchiveV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(),
                    Times.Once);

            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
