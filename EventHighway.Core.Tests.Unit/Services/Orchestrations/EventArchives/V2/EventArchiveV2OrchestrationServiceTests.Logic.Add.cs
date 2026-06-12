// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V2
{
    public partial class EventArchiveV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldAddEventArchiveV2WithListenerEventArchiveV2sAsync()
        {
            // given
            EventArchiveV2 randomEventArchiveV2 = CreateRandomEventArchiveV2();
            EventArchiveV2 inputEventArchiveV2 = randomEventArchiveV2;

            List<ListenerEventArchiveV2> randomListenerEventArchiveV2s =
                randomEventArchiveV2.ListenerEventArchiveV2s.ToList();

            List<ListenerEventArchiveV2> inputListenerEventArchiveV2s =
                randomListenerEventArchiveV2s;

            // when
            await this.eventArchiveV2OrchestrationService
                .AddEventArchiveV2WithListenerEventArchiveV2sAsync(
                    inputEventArchiveV2,
                    TestContext.Current.CancellationToken);

            // then
            foreach (ListenerEventArchiveV2 listenerEventArchiveV2 in inputListenerEventArchiveV2s)
            {
                this.listenerEventArchiveV2ServiceMock.Verify(service =>
                    service.AddListenerEventArchiveV2Async(
                        listenerEventArchiveV2,
                        TestContext.Current.CancellationToken),
                            Times.Once);
            }

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.AddEventArchiveV2Async(
                    inputEventArchiveV2,
                    TestContext.Current.CancellationToken),
                        Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
