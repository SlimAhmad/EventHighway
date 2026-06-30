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

namespace EventHighway.Core.Tests.Unit.Clients.ListenerEventArchives.V2
{
    public partial class ListenerEventArchiveV2ClientTests
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
                retrievedListenerEventArchiveV2s.DeepClone();

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(retrievedListenerEventArchiveV2s);

            // when
            IQueryable<ListenerEventArchiveV2> actualListenerEventArchiveV2s =
                await this.listenerEventArchiveV2Client
                    .RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken);

            // then
            actualListenerEventArchiveV2s.Should().BeEquivalentTo(expectedListenerEventArchiveV2s);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
