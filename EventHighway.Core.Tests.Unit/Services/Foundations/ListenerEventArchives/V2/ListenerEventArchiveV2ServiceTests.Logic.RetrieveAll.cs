// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEventArchives.V2
{
    public partial class ListenerEventArchiveV2ServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllListenerEventArchiveV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<ListenerEventArchiveV2> randomListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s();

            IQueryable<ListenerEventArchiveV2> retrievedListenerEventArchiveV2s =
                randomListenerEventArchiveV2s;

            IQueryable<ListenerEventArchiveV2> expectedListenerEventArchiveV2s =
                randomListenerEventArchiveV2s.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(retrievedListenerEventArchiveV2s);

            // when
            IQueryable<ListenerEventArchiveV2> actualListenerEventArchiveV2s =
                await this.listenerEventArchiveV2Service
                    .RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken);

            // then
            actualListenerEventArchiveV2s.Should().BeEquivalentTo(
                expectedListenerEventArchiveV2s);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllListenerEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
