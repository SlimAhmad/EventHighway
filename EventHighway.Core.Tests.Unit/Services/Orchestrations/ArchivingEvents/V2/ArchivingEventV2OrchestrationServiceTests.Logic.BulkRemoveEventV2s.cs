// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ArchivingEvents.V2
{
    public partial class ArchivingEventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldBulkRemoveEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<EventV2> randomEventV2s = CreateRandomEventV2s();
            IEnumerable<EventV2> inputEventV2s = randomEventV2s;

            this.eventV2ProcessingServiceMock.Setup(service =>
                service.BulkRemoveEventV2sAsync(
                    inputEventV2s,
                    randomCancellationToken))
                        .Returns(ValueTask.CompletedTask);

            // when
            await this.archivingEventV2OrchestrationService
                .BulkRemoveEventV2sAsync(
                    inputEventV2s,
                    randomCancellationToken);

            // then
            this.eventV2ProcessingServiceMock.Verify(service =>
                service.BulkRemoveEventV2sAsync(
                    inputEventV2s,
                    randomCancellationToken),
                        Times.Once);

            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
