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

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V2
{
    public partial class EventArchiveV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllListenerEventArchiveV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken = TestContext.Current.CancellationToken;

            IQueryable<ListenerEventArchiveV2> randomListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s();

            IQueryable<ListenerEventArchiveV2> retrievedListenerEventArchiveV2s =
                randomListenerEventArchiveV2s;

            IQueryable<ListenerEventArchiveV2> expectedListenerEventArchiveV2s =
                randomListenerEventArchiveV2s.DeepClone();

            this.listenerEventArchiveV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(retrievedListenerEventArchiveV2s);

            // when
            IQueryable<ListenerEventArchiveV2> actualListenerEventArchiveV2s =
                await this.eventArchiveV2OrchestrationService
                    .RetrieveAllListenerEventArchiveV2sAsync(
                        randomCancellationToken);

            // then
            actualListenerEventArchiveV2s.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV2s);

            this.listenerEventArchiveV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
