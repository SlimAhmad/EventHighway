// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.RestoringEvents.V2
{
    public partial class RestoringEventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRestoreEventV2sAndListenerEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<EventArchiveV2> inputEventArchiveV2s = CreateRandomEventArchiveV2s();

            List<ListenerEventArchiveV2> inputListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s();

            List<EventV2> expectedEventV2sToRestore =
                inputEventArchiveV2s.Select(MapToEventV2).ToList();

            List<ListenerEventV2> expectedListenerEventV2sToRestore =
                inputListenerEventArchiveV2s.Select(MapToListenerEventV2).ToList();

            var inputSequence = new MockSequence();

            this.eventV2ProcessingServiceMock.InSequence(inputSequence).Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(new List<EventV2>().AsQueryable());

            this.eventV2ProcessingServiceMock.InSequence(inputSequence).Setup(service =>
                service.BulkRestoreEventV2sAsync(
                    It.IsAny<IEnumerable<EventV2>>(),
                    randomCancellationToken))
                        .ReturnsAsync(expectedEventV2sToRestore);

            this.listenerEventV2ProcessingServiceMock.InSequence(inputSequence).Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(new List<ListenerEventV2>().AsQueryable());

            this.listenerEventV2ProcessingServiceMock.InSequence(inputSequence).Setup(service =>
                service.BulkRestoreListenerEventV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventV2>>(),
                    randomCancellationToken))
                        .ReturnsAsync(expectedListenerEventV2sToRestore);

            // when
            await this.restoringEventV2OrchestrationService.RestoreAsync(
                inputEventArchiveV2s,
                inputListenerEventArchiveV2s,
                randomCancellationToken);

            // then
            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.BulkRestoreEventV2sAsync(
                    It.Is<List<EventV2>>(actual =>
                        SameEventV2sAs(expectedEventV2sToRestore, actual)),
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.BulkRestoreListenerEventV2sAsync(
                    It.Is<List<ListenerEventV2>>(actual =>
                        SameListenerEventV2sAs(expectedListenerEventV2sToRestore, actual)),
                    randomCancellationToken),
                        Times.Once);

            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
