// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V2
{
    public partial class EventArchiveV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldBulkAddEventArchiveV2sAsync()
        {
            // given
            IQueryable<EventArchiveV2> randomEventArchiveV2s = CreateRandomEventArchiveV2s();
            IEnumerable<EventArchiveV2> inputEventArchiveV2s = randomEventArchiveV2s;

            IEnumerable<ListenerEventArchiveV2> inputListenerEventArchiveV2s =
                inputEventArchiveV2s.SelectMany(eventArchiveV2 =>
                    eventArchiveV2.ListenerEventArchiveV2s);

            var mockSequence = new MockSequence();

            this.eventArchiveV2ServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.BulkAddEventArchiveV2sAsync(
                        inputEventArchiveV2s,
                        It.IsAny<CancellationToken>()))
                            .ReturnsAsync(inputEventArchiveV2s);

            this.listenerEventArchiveV2ServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.BulkAddListenerEventArchiveV2sAsync(
                        inputListenerEventArchiveV2s,
                        It.IsAny<CancellationToken>()))
                            .ReturnsAsync(inputListenerEventArchiveV2s);

            // when
            await this.eventArchiveV2OrchestrationService.BulkAddEventArchiveV2sAsync(
                inputEventArchiveV2s,
                TestContext.Current.CancellationToken);

            // then
            this.eventArchiveV2ServiceMock.Verify(service =>
                service.BulkAddEventArchiveV2sAsync(
                    inputEventArchiveV2s,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    inputListenerEventArchiveV2s,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
