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

            Guid eventArchiveId = GetRandomId();
            var eventArchiveV2 = new EventArchiveV2 { Id = eventArchiveId };

            List<EventArchiveV2> eventArchiveV2Batch =
                new List<EventArchiveV2> { eventArchiveV2 };

            List<ListenerEventArchiveV2> listenerArchivePage =
                new List<ListenerEventArchiveV2>
                {
                    new ListenerEventArchiveV2
                    {
                        Id = GetRandomId(),
                        EventArchiveV2Id = eventArchiveId
                    }
                };

            var eventArchiveV2WithListenerArchivePage = new EventArchiveV2
            {
                Id = eventArchiveId,
                ListenerEventArchiveV2s = listenerArchivePage
            };

            var eventArchiveV2WithEmptyListenerArchivePage = new EventArchiveV2
            {
                Id = eventArchiveId,
                ListenerEventArchiveV2s = new List<ListenerEventArchiveV2>()
            };

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfEventArchiveV2sMatchingAsync(
                    eventAddressId, eventListenerIds, startDate, endDate, 0, take, randomCancellationToken))
                        .ReturnsAsync(eventArchiveV2Batch);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfEventArchiveV2sMatchingAsync(
                    eventAddressId, eventListenerIds, startDate, endDate, take, take, randomCancellationToken))
                        .ReturnsAsync(new List<EventArchiveV2>());

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveEventArchiveV2WithListenerEventArchiveV2sByIdAsync(
                    eventArchiveId, eventListenerIds, startDate, endDate, 0, take, randomCancellationToken))
                        .ReturnsAsync(eventArchiveV2WithListenerArchivePage);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveEventArchiveV2WithListenerEventArchiveV2sByIdAsync(
                    eventArchiveId, eventListenerIds, startDate, endDate, take, take, randomCancellationToken))
                        .ReturnsAsync(eventArchiveV2WithEmptyListenerArchivePage);

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
                service.RetrieveBatchOfEventArchiveV2sMatchingAsync(
                    eventAddressId, eventListenerIds, startDate, endDate, 0, take, randomCancellationToken),
                        Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfEventArchiveV2sMatchingAsync(
                    eventAddressId, eventListenerIds, startDate, endDate, take, take, randomCancellationToken),
                        Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventArchiveV2WithListenerEventArchiveV2sByIdAsync(
                    eventArchiveId, eventListenerIds, startDate, endDate, 0, take, randomCancellationToken),
                        Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventArchiveV2WithListenerEventArchiveV2sByIdAsync(
                    eventArchiveId, eventListenerIds, startDate, endDate, take, take, randomCancellationToken),
                        Times.Once);

            this.restoringEventV2OrchestrationServiceMock.Verify(service =>
                service.RestoreAsync(
                    It.Is<IEnumerable<EventArchiveV2>>(actualEventArchiveV2s =>
                        actualEventArchiveV2s.SequenceEqual(new List<EventArchiveV2> { eventArchiveV2 })),
                    It.Is<IEnumerable<ListenerEventArchiveV2>>(actualListenerArchivePage =>
                        actualListenerArchivePage.SequenceEqual(listenerArchivePage)),
                    randomCancellationToken),
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

        [Fact]
        public async Task ShouldReplayEventArchiveV2sAndGenerateForNewListenersWhenListenerIdsNullAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid? eventAddressId = GetRandomId();
            IEnumerable<Guid> nullEventListenerIds = null;
            DateTimeOffset? startDate = GetRandomDateTimeOffset();
            DateTimeOffset? endDate = startDate.Value.AddDays(GetRandomNumber());

            int take = GetRandomNumber();
            BatchConfiguration batchConfiguration = CreateBatchConfiguration(take);

            Guid eventArchiveId = GetRandomId();
            var eventArchiveV2 = new EventArchiveV2 { Id = eventArchiveId };

            List<EventArchiveV2> eventArchiveV2Batch =
                new List<EventArchiveV2> { eventArchiveV2 };

            List<ListenerEventArchiveV2> listenerArchivePage =
                new List<ListenerEventArchiveV2>
                {
                    new ListenerEventArchiveV2
                    {
                        Id = GetRandomId(),
                        EventArchiveV2Id = eventArchiveId
                    }
                };

            var eventArchiveV2WithListenerArchivePage = new EventArchiveV2
            {
                Id = eventArchiveId,
                ListenerEventArchiveV2s = listenerArchivePage
            };

            var eventArchiveV2WithEmptyListenerArchivePage = new EventArchiveV2
            {
                Id = eventArchiveId,
                ListenerEventArchiveV2s = new List<ListenerEventArchiveV2>()
            };

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfEventArchiveV2sMatchingAsync(
                    eventAddressId, nullEventListenerIds, startDate, endDate, 0, take, randomCancellationToken))
                        .ReturnsAsync(eventArchiveV2Batch);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfEventArchiveV2sMatchingAsync(
                    eventAddressId, nullEventListenerIds, startDate, endDate, take, take, randomCancellationToken))
                        .ReturnsAsync(new List<EventArchiveV2>());

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveEventArchiveV2WithListenerEventArchiveV2sByIdAsync(
                    eventArchiveId, nullEventListenerIds, startDate, endDate, 0, take, randomCancellationToken))
                        .ReturnsAsync(eventArchiveV2WithListenerArchivePage);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveEventArchiveV2WithListenerEventArchiveV2sByIdAsync(
                    eventArchiveId, nullEventListenerIds, startDate, endDate, take, take, randomCancellationToken))
                        .ReturnsAsync(eventArchiveV2WithEmptyListenerArchivePage);

            this.restoringEventV2OrchestrationServiceMock.Setup(service =>
                service.RestoreAsync(
                    It.IsAny<IEnumerable<EventArchiveV2>>(),
                    It.IsAny<IEnumerable<ListenerEventArchiveV2>>(),
                    randomCancellationToken))
                        .Returns(ValueTask.CompletedTask);

            this.restoringEventV2OrchestrationServiceMock.Setup(service =>
                service.GenerateReplayForNewListenersAsync(
                    It.IsAny<IEnumerable<EventArchiveV2>>(), randomCancellationToken))
                        .Returns(ValueTask.CompletedTask);

            // when
            await this.replayingEventV2CoordinationService.ReplayEventArchiveV2sAsync(
                eventAddressId, nullEventListenerIds, startDate, endDate, randomCancellationToken);

            // then
            this.restoringEventV2OrchestrationServiceMock.Verify(service =>
                service.RestoreAsync(
                    It.Is<IEnumerable<EventArchiveV2>>(actualEventArchiveV2s =>
                        actualEventArchiveV2s.SequenceEqual(new List<EventArchiveV2> { eventArchiveV2 })),
                    It.Is<IEnumerable<ListenerEventArchiveV2>>(actualListenerArchivePage =>
                        actualListenerArchivePage.SequenceEqual(listenerArchivePage)),
                    randomCancellationToken),
                        Times.Once);

            this.restoringEventV2OrchestrationServiceMock.Verify(service =>
                service.GenerateReplayForNewListenersAsync(
                    It.Is<IEnumerable<EventArchiveV2>>(actualEventArchiveV2s =>
                        actualEventArchiveV2s.SequenceEqual(eventArchiveV2Batch)),
                    randomCancellationToken),
                        Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfEventArchiveV2sMatchingAsync(
                    eventAddressId, nullEventListenerIds, startDate, endDate,
                    It.IsAny<int>(), take, randomCancellationToken),
                        Times.Exactly(2));

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventArchiveV2WithListenerEventArchiveV2sByIdAsync(
                    eventArchiveId, nullEventListenerIds, startDate, endDate,
                    It.IsAny<int>(), take, randomCancellationToken),
                        Times.Exactly(2));

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(), Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.restoringEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldReplayInSinglePassWhenBatchSizeIsZeroAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid? eventAddressId = GetRandomId();
            List<Guid> eventListenerIds = new List<Guid> { GetRandomId() };
            DateTimeOffset? startDate = GetRandomDateTimeOffset();
            DateTimeOffset? endDate = startDate.Value.AddDays(GetRandomNumber());

            int take = 0;
            BatchConfiguration batchConfiguration = CreateBatchConfiguration(take);

            Guid eventArchiveId = GetRandomId();
            var eventArchiveV2 = new EventArchiveV2 { Id = eventArchiveId };

            List<EventArchiveV2> eventArchiveV2Batch =
                new List<EventArchiveV2> { eventArchiveV2 };

            List<ListenerEventArchiveV2> listenerArchivePage =
                new List<ListenerEventArchiveV2>
                {
                    new ListenerEventArchiveV2
                    {
                        Id = GetRandomId(),
                        EventArchiveV2Id = eventArchiveId
                    }
                };

            var eventArchiveV2WithListenerArchivePage = new EventArchiveV2
            {
                Id = eventArchiveId,
                ListenerEventArchiveV2s = listenerArchivePage
            };

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfEventArchiveV2sMatchingAsync(
                    eventAddressId, eventListenerIds, startDate, endDate, 0, take, randomCancellationToken))
                        .ReturnsAsync(eventArchiveV2Batch);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveEventArchiveV2WithListenerEventArchiveV2sByIdAsync(
                    eventArchiveId, eventListenerIds, startDate, endDate, 0, take, randomCancellationToken))
                        .ReturnsAsync(eventArchiveV2WithListenerArchivePage);

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
            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfEventArchiveV2sMatchingAsync(
                    eventAddressId, eventListenerIds, startDate, endDate, 0, take, randomCancellationToken),
                        Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventArchiveV2WithListenerEventArchiveV2sByIdAsync(
                    eventArchiveId, eventListenerIds, startDate, endDate, 0, take, randomCancellationToken),
                        Times.Once);

            this.restoringEventV2OrchestrationServiceMock.Verify(service =>
                service.RestoreAsync(
                    It.Is<IEnumerable<EventArchiveV2>>(actualEventArchiveV2s =>
                        actualEventArchiveV2s.SequenceEqual(new List<EventArchiveV2> { eventArchiveV2 })),
                    It.Is<IEnumerable<ListenerEventArchiveV2>>(actualListenerArchivePage =>
                        actualListenerArchivePage.SequenceEqual(listenerArchivePage)),
                    randomCancellationToken),
                        Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(), Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.restoringEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldNotRestoreWhenNoArchivesMatchAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid? eventAddressId = GetRandomId();
            List<Guid> eventListenerIds = new List<Guid> { GetRandomId() };
            DateTimeOffset? startDate = GetRandomDateTimeOffset();
            DateTimeOffset? endDate = startDate.Value.AddDays(GetRandomNumber());

            int take = GetRandomNumber();
            BatchConfiguration batchConfiguration = CreateBatchConfiguration(take);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfEventArchiveV2sMatchingAsync(
                    eventAddressId, eventListenerIds, startDate, endDate, 0, take, randomCancellationToken))
                        .ReturnsAsync(new List<EventArchiveV2>());

            // when
            await this.replayingEventV2CoordinationService.ReplayEventArchiveV2sAsync(
                eventAddressId, eventListenerIds, startDate, endDate, randomCancellationToken);

            // then
            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfEventArchiveV2sMatchingAsync(
                    eventAddressId, eventListenerIds, startDate, endDate, 0, take, randomCancellationToken),
                        Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventArchiveV2WithListenerEventArchiveV2sByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.restoringEventV2OrchestrationServiceMock.Verify(service =>
                service.RestoreAsync(
                    It.IsAny<IEnumerable<EventArchiveV2>>(),
                    It.IsAny<IEnumerable<ListenerEventArchiveV2>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(), Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.restoringEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
