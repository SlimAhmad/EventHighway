// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
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
        public async Task ShouldBulkAddListenerEventArchiveV2sAsync()
        {
            // given
            IQueryable<ListenerEventArchiveV2> randomListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s();

            IEnumerable<ListenerEventArchiveV2> inputListenerEventArchiveV2s =
                randomListenerEventArchiveV2s;

            IEnumerable<ListenerEventArchiveV2> addedListenerEventArchiveV2s =
                randomListenerEventArchiveV2s;

            IEnumerable<ListenerEventArchiveV2> expectedListenerEventArchiveV2s =
                addedListenerEventArchiveV2s.DeepClone();

            this.listenerEventArchiveV2ProcessingServiceMock.Setup(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    inputListenerEventArchiveV2s,
                    TestContext.Current.CancellationToken))
                        .ReturnsAsync(addedListenerEventArchiveV2s);

            // when
            IEnumerable<ListenerEventArchiveV2> actualListenerEventArchiveV2s =
                await this.eventArchiveV2OrchestrationService.BulkAddListenerEventArchiveV2sAsync(
                    inputListenerEventArchiveV2s,
                    TestContext.Current.CancellationToken);

            // then
            actualListenerEventArchiveV2s.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV2s);

            this.listenerEventArchiveV2ProcessingServiceMock.Verify(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    inputListenerEventArchiveV2s,
                    TestContext.Current.CancellationToken),
                        Times.Once);

            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
