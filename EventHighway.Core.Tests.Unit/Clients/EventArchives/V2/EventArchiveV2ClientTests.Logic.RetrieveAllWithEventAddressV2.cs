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

namespace EventHighway.Core.Tests.Unit.Clients.EventArchives.V2
{
    public partial class EventArchiveV2ClientTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventArchiveV2sWithEventAddressV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<EventArchiveV2> randomEventArchiveV2s =
                CreateRandomEventArchiveV2s();

            IQueryable<EventArchiveV2> retrievedEventArchiveV2s =
                randomEventArchiveV2s;

            IQueryable<EventArchiveV2> expectedEventArchiveV2s =
                retrievedEventArchiveV2s.DeepClone();

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sWithEventAddressV2Async(
                    randomCancellationToken))
                        .ReturnsAsync(retrievedEventArchiveV2s);

            // when
            IQueryable<EventArchiveV2> actualEventArchiveV2s =
                await this.eventArchiveV2Client
                    .RetrieveAllEventArchiveV2sWithEventAddressV2Async(
                        randomCancellationToken);

            // then
            actualEventArchiveV2s.Should().BeEquivalentTo(
                expectedEventArchiveV2s);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sWithEventAddressV2Async(
                    randomCancellationToken),
                        Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
