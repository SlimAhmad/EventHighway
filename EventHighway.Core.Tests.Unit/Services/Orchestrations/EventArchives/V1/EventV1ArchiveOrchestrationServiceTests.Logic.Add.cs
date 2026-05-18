// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V1
{
    public partial class EventV1ArchiveOrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldAddEventV1ArchiveWithListenerEventV1ArchivesAsync()
        {
            // given
            EventArchiveV1 randomEventV1Archive = CreateRandomEventV1Archive();
            EventArchiveV1 inputEventV1Archive = randomEventV1Archive;

            List<ListenerEventArchiveV1> randomListenerEventV1Archives =
                randomEventV1Archive.ListenerEventArchiveV1s.ToList();

            List<ListenerEventArchiveV1> inputListenerEventV1Archives =
                randomListenerEventV1Archives;

            // when
            await this.eventV1ArchiveOrchestrationService
                .AddEventV1ArchiveWithListenerEventV1ArchivesAsync(inputEventV1Archive);

            // then
            foreach (ListenerEventArchiveV1 listenerEventV1Archive in inputListenerEventV1Archives)
            {
                this.listenerEventV1ArchiveServiceMock.Verify(service =>
                    service.AddListenerEventV1ArchiveAsync(listenerEventV1Archive),
                        Times.Once);
            }

            this.eventV1ArchiveServiceMock.Verify(service =>
                service.AddEventArchiveV1Async(inputEventV1Archive),
                    Times.Once);

            this.listenerEventV1ArchiveServiceMock.VerifyNoOtherCalls();
            this.eventV1ArchiveServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
