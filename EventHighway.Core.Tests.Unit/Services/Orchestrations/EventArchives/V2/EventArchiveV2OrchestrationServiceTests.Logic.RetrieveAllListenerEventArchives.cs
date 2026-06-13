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
            IQueryable<ListenerEventArchiveV2> randomListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s();

            IQueryable<ListenerEventArchiveV2> retrievedListenerEventArchiveV2s =
                randomListenerEventArchiveV2s;

            IQueryable<ListenerEventArchiveV2> expectedListenerEventArchiveV2s =
                randomListenerEventArchiveV2s.DeepClone();

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync())
                    .ReturnsAsync(retrievedListenerEventArchiveV2s);

            // when
            IQueryable<ListenerEventArchiveV2> actualListenerEventArchiveV2s =
                await this.eventArchiveV2OrchestrationService
                    .RetrieveAllListenerEventArchiveV2sAsync(
                        TestContext.Current.CancellationToken);

            // then
            actualListenerEventArchiveV2s.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV2s);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(),
                    Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
