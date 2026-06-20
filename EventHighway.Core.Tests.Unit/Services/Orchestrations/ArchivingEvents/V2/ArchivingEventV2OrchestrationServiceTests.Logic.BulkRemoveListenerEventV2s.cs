// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ArchivingEvents.V2
{
    public partial class ArchivingEventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldBulkRemoveListenerEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEnumerable<ListenerEventV2> randomListenerEventV2s = CreateRandomListenerEventV2s();
            IEnumerable<ListenerEventV2> inputListenerEventV2s = randomListenerEventV2s;

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.BulkRemoveListenerEventV2sAsync(
                    inputListenerEventV2s,
                    randomCancellationToken))
                        .Returns(ValueTask.CompletedTask);

            // when
            await this.archivingEventV2OrchestrationService
                .BulkRemoveListenerEventV2sAsync(
                    inputListenerEventV2s,
                    randomCancellationToken);

            // then
            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.BulkRemoveListenerEventV2sAsync(
                    inputListenerEventV2s,
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
