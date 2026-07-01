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
        public async Task ShouldBulkRemoveEventV2sAndTheirListenerEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<EventV2> randomEventV2s = CreateRandomEventV2s();
            IEnumerable<EventV2> inputEventV2s = randomEventV2s;

            IEnumerable<ListenerEventV2> expectedListenerEventV2s =
                inputEventV2s.SelectMany(eventV2 => eventV2.ListenerEventV2s).ToList();

            var sequence = new MockSequence();

            this.listenerEventV2ProcessingServiceMock.InSequence(sequence).Setup(service =>
                service.BulkRemoveListenerEventV2sAsync(
                    It.Is<IEnumerable<ListenerEventV2>>(actualListenerEventV2s =>
                        actualListenerEventV2s.SequenceEqual(expectedListenerEventV2s)),
                    randomCancellationToken))
                        .Returns(ValueTask.CompletedTask);

            this.eventV2ProcessingServiceMock.InSequence(sequence).Setup(service =>
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
            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.BulkRemoveListenerEventV2sAsync(
                    It.Is<IEnumerable<ListenerEventV2>>(actualListenerEventV2s =>
                        actualListenerEventV2s.SequenceEqual(expectedListenerEventV2s)),
                    randomCancellationToken),
                        Times.Once);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.BulkRemoveEventV2sAsync(
                    inputEventV2s,
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
