// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ArchivingEvents.V2
{
    public partial class ArchivingEventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRemoveEventV2AndListenerEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 randomEventV2 = CreateRandomEventV2();

            var randomListenerEventV2s =
                new List<ListenerEventV2>(randomEventV2.ListenerEventV2s);

            EventV2 inputEventV2 = randomEventV2;

            // when
            await this.archivingEventV2OrchestrationService
                .RemoveEventV2AndListenerEventV2sAsync(
                    inputEventV2,
                    randomCancellationToken);

            // then
            foreach (ListenerEventV2 listenerEventV2 in randomListenerEventV2s)
            {
                this.listenerEventV2ProcessingServiceMock.Verify(service =>
                    service.RemoveListenerEventV2ByIdAsync(
                        listenerEventV2.Id,
                        randomCancellationToken),
                            Times.Once);
            }

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RemoveEventV2ByIdAsync(
                    inputEventV2.Id,
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
