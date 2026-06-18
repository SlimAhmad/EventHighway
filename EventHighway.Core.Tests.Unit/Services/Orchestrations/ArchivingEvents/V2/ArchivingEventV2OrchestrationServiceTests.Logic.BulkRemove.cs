// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
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
        public async Task ShouldBulkRemoveEventV2AndListenerEventV2sAsync()
        {
            // given
            var mockSequence = new MockSequence();
            IQueryable<EventV2> randomEventV2s = CreateRandomEventV2s();
            IEnumerable<EventV2> inputEventV2s = randomEventV2s;

            IEnumerable<ListenerEventV2> inputListenerEventV2s =
                inputEventV2s.SelectMany(eventV2 => eventV2.ListenerEventV2s);

            this.listenerEventV2ProcessingServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.BulkRemoveListenerEventV2sAsync(
                        inputListenerEventV2s,
                        It.IsAny<CancellationToken>()))
                            .Returns(ValueTask.CompletedTask);

            this.eventV2ProcessingServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.BulkRemoveEventV2sAsync(
                        inputEventV2s,
                        It.IsAny<CancellationToken>()))
                            .Returns(ValueTask.CompletedTask);

            // when
            await this.archivingEventV2OrchestrationService
                .BulkRemoveEventV2AndListenerEventV2sAsync(
                    inputEventV2s,
                    TestContext.Current.CancellationToken);

            // then
            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.BulkRemoveListenerEventV2sAsync(
                    inputListenerEventV2s,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.BulkRemoveEventV2sAsync(
                    inputEventV2s,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
