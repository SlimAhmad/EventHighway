// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.ReplayingEvents.V2
{
    public partial class ReplayingEventV2CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldReplayEventArchiveV2sForSpecifiedListenersAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid? eventAddressId = GetRandomId();

            List<Guid> eventListenerIds =
                new List<Guid> { GetRandomId(), GetRandomId() };

            DateTimeOffset? startDate = GetRandomDateTimeOffset();
            DateTimeOffset? endDate = startDate.Value.AddDays(GetRandomNumber());

            int take = GetRandomNumber();
            BatchConfiguration batchConfiguration = CreateBatchConfiguration(take);

            List<ListenerEventArchiveV2> listenerArchiveBatch =
                CreateRandomListenerEventArchiveV2s();

            List<EventArchiveV2> eventArchives = CreateRandomEventArchiveV2s();

            IEnumerable<Guid> expectedEventArchiveIds =
                listenerArchiveBatch
                    .Select(listenerEventArchiveV2 => listenerEventArchiveV2.EventArchiveV2Id)
                    .Distinct()
                    .ToList();

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfListenerEventArchiveV2sAsync(
                    eventAddressId, eventListenerIds, startDate, endDate, 0, take, randomCancellationToken))
                        .ReturnsAsync(listenerArchiveBatch);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfListenerEventArchiveV2sAsync(
                    eventAddressId, eventListenerIds, startDate, endDate, take, take, randomCancellationToken))
                        .ReturnsAsync(new List<ListenerEventArchiveV2>());

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveEventArchiveV2sByIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(), randomCancellationToken))
                        .ReturnsAsync(eventArchives);

            this.restoringEventV2OrchestrationServiceMock.Setup(service =>
                service.RestoreAsync(
                    It.IsAny<IEnumerable<EventArchiveV2>>(),
                    It.IsAny<IEnumerable<ListenerEventArchiveV2>>(),
                    randomCancellationToken))
                        .Returns(ValueTask.CompletedTask);

            // when
            await this.replayingEventV2CoordinationService.ReplayEventArchiveV2sAsync(
                eventAddressId, eventListenerIds, startDate, endDate, randomCancellationToken);

            // then
            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(), Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfListenerEventArchiveV2sAsync(
                    eventAddressId, eventListenerIds, startDate, endDate, 0, take, randomCancellationToken),
                        Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfListenerEventArchiveV2sAsync(
                    eventAddressId, eventListenerIds, startDate, endDate, take, take, randomCancellationToken),
                        Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventArchiveV2sByIdsAsync(
                    It.Is<IEnumerable<Guid>>(ids => SameGuidSetAs(expectedEventArchiveIds, ids)),
                    randomCancellationToken),
                        Times.Once);

            this.restoringEventV2OrchestrationServiceMock.Verify(service =>
                service.RestoreAsync(eventArchives, listenerArchiveBatch, randomCancellationToken),
                    Times.Once);

            this.restoringEventV2OrchestrationServiceMock.Verify(service =>
                service.GenerateReplayForNewListenersAsync(
                    It.IsAny<IEnumerable<EventArchiveV2>>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.restoringEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
