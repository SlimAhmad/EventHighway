// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventArchives.V1
{
    public partial class EventV1ArchiveServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventV1ArchivesAsync()
        {
            // given
            IQueryable<EventArchiveV1> randomEventV1Archives =
                CreateRandomEventV1Archives();

            IQueryable<EventArchiveV1> retrievedEventV1Archives =
                randomEventV1Archives;

            IQueryable<EventArchiveV1> expectedEventV1Archives =
                randomEventV1Archives.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventArchiveV1sAsync())
                    .ReturnsAsync(retrievedEventV1Archives);

            // when
            IQueryable<EventArchiveV1> actualEventV1Archives =
                await this.eventV1ArchiveService
                    .RetrieveAllEventArchiveV1sAsync();

            // then
            actualEventV1Archives.Should().BeEquivalentTo(
                expectedEventV1Archives);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventArchiveV1sAsync(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
