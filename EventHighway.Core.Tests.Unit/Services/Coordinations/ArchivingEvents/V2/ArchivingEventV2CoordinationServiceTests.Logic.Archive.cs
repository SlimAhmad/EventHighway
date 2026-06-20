// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.ArchivingEvents.V2
{
    public partial class ArchivingEventV2CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldArchiveDeadEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var mockSequence = new MockSequence();

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            List<dynamic> randomEventV2sProperties = CreateRandomEventV2sProperties();

            List<EventV2> retrievedEventV2s =
                randomEventV2sProperties.Select(item =>
                    new EventV2
                    {
                        Id = item.Id,
                        Content = item.Content,
                        EventName = item.EventName,
                        Type = (EventTypeV2)item.Type,
                        CreatedDate = item.CreatedDate,
                        UpdatedDate = item.UpdatedDate,
                        ScheduledDate = item.ScheduledDate,
                        RemainingRetryAttempts = item.RemainingRetryAttempts,
                        EventAddressId = item.EventAddressId
                    }).ToList();

            List<dynamic> randomListenerEventV2sProperties = CreateRandomListenerEventV2sProperties();

            List<ListenerEventV2> retrievedListenerEventV2s =
                randomListenerEventV2sProperties.Select(item =>
                    new ListenerEventV2
                    {
                        Id = item.Id,
                        Status = (ListenerEventStatusV2)item.Status,
                        Response = item.Response,
                        ResponseCode = item.ResponseCode,
                        ResponseMessage = item.ResponseMessage,
                        CreatedDate = item.CreatedDate,
                        UpdatedDate = item.UpdatedDate,
                        EventId = item.EventId,
                        EventAddressId = item.EventAddressId,
                        EventListenerId = item.EventListenerId
                    }).ToList();

            IEnumerable<Guid> inputEventV2Ids =
                retrievedEventV2s.Select(eventV2 => eventV2.Id).ToList();

            List<EventArchiveV2> expectedEventArchiveV2s =
                randomEventV2sProperties.Select(item =>
                    new EventArchiveV2
                    {
                        Id = item.Id,
                        Content = item.Content,
                        EventName = item.EventName,
                        Type = (EventArchiveTypeV2)item.Type,
                        CreatedDate = item.CreatedDate,
                        UpdatedDate = item.UpdatedDate,
                        ScheduledDate = item.ScheduledDate,
                        RemainingRetryAttempts = item.RemainingRetryAttempts,
                        EventAddressId = item.EventAddressId
                    }).ToList();

            List<ListenerEventArchiveV2> expectedListenerEventArchiveV2s =
                randomListenerEventV2sProperties.Select(item =>
                    new ListenerEventArchiveV2
                    {
                        Id = item.Id,
                        Status = (ListenerEventArchiveStatusV2)item.Status,
                        Response = item.Response,
                        ResponseCode = item.ResponseCode,
                        ResponseMessage = item.ResponseMessage,
                        CreatedDate = item.CreatedDate,
                        UpdatedDate = item.UpdatedDate,
                        EventId = item.EventId,
                        EventAddressId = item.EventAddressId,
                        EventListenerId = item.EventListenerId,
                        EventArchiveV2Id = item.EventId
                    }).ToList();

            // Step 2
            this.archivingEventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.RetrieveBatchOfDeadEventV2sAsync(randomCancellationToken))
                        .ReturnsAsync((IEnumerable<EventV2>)retrievedEventV2s);

            // Step 3
            this.eventArchiveV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.BulkAddEventArchiveV2sAsync(
                        It.Is(SameEventArchiveV2sAs(expectedEventArchiveV2s)),
                        randomCancellationToken))
                            .ReturnsAsync(expectedEventArchiveV2s);

            IEnumerable<Guid> archivedEventV2Ids =
                expectedEventArchiveV2s.Select(a => a.Id).ToList();

            // Step 4
            this.archivingEventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.RetrieveBatchOfListenerEventV2sAsync(
                        It.Is(SameEventV2IdsAs(archivedEventV2Ids)),
                        randomCancellationToken))
                            .ReturnsAsync((IEnumerable<ListenerEventV2>)retrievedListenerEventV2s);

            // Step 5
            this.eventArchiveV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.BulkAddListenerEventArchiveV2sAsync(
                        It.Is(SameListenerEventArchiveV2sAs(expectedListenerEventArchiveV2s)),
                        randomCancellationToken))
                            .ReturnsAsync(expectedListenerEventArchiveV2s);

            // Step 6
            this.archivingEventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.BulkRemoveListenerEventV2sAsync(
                        It.Is(SameListenerEventV2sAs(retrievedListenerEventV2s)),
                        randomCancellationToken))
                            .Returns(ValueTask.CompletedTask);

            List<EventV2> archivedDeadEventV2s =
                retrievedEventV2s
                    .Where(e => archivedEventV2Ids.Contains(e.Id)).ToList();

            // Step 7
            this.archivingEventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.RetrieveBatchOfListenerEventV2sAsync(
                        It.Is(SameEventV2IdsAs(archivedEventV2Ids)),
                        randomCancellationToken))
                            .ReturnsAsync(Enumerable.Empty<ListenerEventV2>());

            // Step 8
            this.archivingEventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.BulkRemoveEventV2sAsync(
                        It.Is(SameEventV2sAs(archivedDeadEventV2s)),
                        randomCancellationToken))
                            .Returns(ValueTask.CompletedTask);

            // Step 9
            this.archivingEventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.RetrieveBatchOfDeadEventV2sAsync(randomCancellationToken))
                        .ReturnsAsync(Enumerable.Empty<EventV2>());

            // when
            await this.archivingEventV2CoordinationService
                .ArchiveDeadEventV2sAsync(randomCancellationToken);

            // then
            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfDeadEventV2sAsync(randomCancellationToken),
                    Times.Exactly(2));

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.BulkAddEventArchiveV2sAsync(
                    It.Is(SameEventArchiveV2sAs(expectedEventArchiveV2s)),
                    randomCancellationToken),
                        Times.Once);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfListenerEventV2sAsync(
                    It.Is(SameEventV2IdsAs(archivedEventV2Ids)),
                    randomCancellationToken),
                        Times.Exactly(2));

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    It.Is(SameListenerEventArchiveV2sAs(expectedListenerEventArchiveV2s)),
                    randomCancellationToken),
                        Times.Once);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.BulkRemoveListenerEventV2sAsync(
                    It.Is(SameListenerEventV2sAs(retrievedListenerEventV2s)),
                    randomCancellationToken),
                        Times.Once);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.BulkRemoveEventV2sAsync(
                    It.Is(SameEventV2sAs(archivedDeadEventV2s)),
                    randomCancellationToken),
                        Times.Once);

            this.archivingEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldArchiveCleanEventV2sAndRetainEventV2sWithUnarchivableListenerEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var mockSequence = new MockSequence();

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            Guid cleanEventV2Id = GetRandomId();
            Guid faultedEventV2Id = GetRandomId();

            var cleanEventV2 = new EventV2
            {
                Id = cleanEventV2Id,
                Content = GetRandomString(),
                EventName = GetRandomString(),
                Type = GetRandomEnum<EventTypeV2>(),
                CreatedDate = randomDateTimeOffset,
                UpdatedDate = randomDateTimeOffset,
                ScheduledDate = randomDateTimeOffset,
                RemainingRetryAttempts = GetRandomNumber(),
                EventAddressId = GetRandomId()
            };

            var faultedEventV2 = new EventV2
            {
                Id = faultedEventV2Id,
                Content = GetRandomString(),
                EventName = GetRandomString(),
                Type = GetRandomEnum<EventTypeV2>(),
                CreatedDate = randomDateTimeOffset,
                UpdatedDate = randomDateTimeOffset,
                ScheduledDate = randomDateTimeOffset,
                RemainingRetryAttempts = GetRandomNumber(),
                EventAddressId = GetRandomId()
            };

            List<EventV2> retrievedEventV2s =
                new List<EventV2> { cleanEventV2, faultedEventV2 };

            var cleanListenerEventV2 = new ListenerEventV2
            {
                Id = GetRandomId(),
                Status = GetRandomEnum<ListenerEventStatusV2>(),
                Response = GetRandomString(),
                ResponseCode = GetRandomString(),
                ResponseMessage = GetRandomString(),
                CreatedDate = randomDateTimeOffset,
                UpdatedDate = randomDateTimeOffset,
                EventId = cleanEventV2Id,
                EventAddressId = GetRandomId(),
                EventListenerId = GetRandomId()
            };

            var faultedListenerEventV2 = new ListenerEventV2
            {
                Id = GetRandomId(),
                Status = GetRandomEnum<ListenerEventStatusV2>(),
                Response = GetRandomString(),
                ResponseCode = GetRandomString(),
                ResponseMessage = GetRandomString(),
                CreatedDate = randomDateTimeOffset,
                UpdatedDate = randomDateTimeOffset,
                EventId = faultedEventV2Id,
                EventAddressId = GetRandomId(),
                EventListenerId = GetRandomId()
            };

            List<ListenerEventV2> retrievedListenerEventV2s =
                new List<ListenerEventV2> { cleanListenerEventV2, faultedListenerEventV2 };

            IEnumerable<Guid> inputEventV2Ids =
                new List<Guid> { cleanEventV2Id, faultedEventV2Id };

            IEnumerable<Guid> remainingEventV2Ids =
                new List<Guid> { cleanEventV2Id };

            List<EventV2> faultedDeadEventV2s =
                new List<EventV2> { faultedEventV2 };

            List<EventV2> expectedRemovableEventV2s =
                new List<EventV2> { cleanEventV2 };

            List<ListenerEventV2> expectedRemovableListenerEventV2s =
                new List<ListenerEventV2> { cleanListenerEventV2 };

            List<EventArchiveV2> expectedEventArchiveV2s =
                retrievedEventV2s.Select(eventV2 =>
                    new EventArchiveV2
                    {
                        Id = eventV2.Id,
                        Content = eventV2.Content,
                        EventName = eventV2.EventName,
                        Type = (EventArchiveTypeV2)eventV2.Type,
                        CreatedDate = eventV2.CreatedDate,
                        UpdatedDate = eventV2.UpdatedDate,
                        ScheduledDate = eventV2.ScheduledDate,
                        RemainingRetryAttempts = eventV2.RemainingRetryAttempts,
                        EventAddressId = eventV2.EventAddressId
                    }).ToList();

            List<ListenerEventArchiveV2> expectedListenerEventArchiveV2s =
                retrievedListenerEventV2s.Select(listenerEventV2 =>
                    new ListenerEventArchiveV2
                    {
                        Id = listenerEventV2.Id,
                        Status = (ListenerEventArchiveStatusV2)listenerEventV2.Status,
                        Response = listenerEventV2.Response,
                        ResponseCode = listenerEventV2.ResponseCode,
                        ResponseMessage = listenerEventV2.ResponseMessage,
                        CreatedDate = listenerEventV2.CreatedDate,
                        UpdatedDate = listenerEventV2.UpdatedDate,
                        EventId = listenerEventV2.EventId,
                        EventAddressId = listenerEventV2.EventAddressId,
                        EventListenerId = listenerEventV2.EventListenerId,
                        EventArchiveV2Id = listenerEventV2.EventId
                    }).ToList();

            IEnumerable<EventArchiveV2> addedEventArchiveV2s =
                expectedEventArchiveV2s;

            IEnumerable<ListenerEventArchiveV2> addedListenerEventArchiveV2s =
                expectedListenerEventArchiveV2s
                    .Where(listenerEventArchiveV2 =>
                        listenerEventArchiveV2.Id == cleanListenerEventV2.Id).ToList();

            var expectedFailedArchivingEventV2CoordinationException =
                new FailedArchivingEventV2CoordinationException(
                    message: "Some dead events could not be fully archived " +
                        "and were retained for the next run.");

            expectedFailedArchivingEventV2CoordinationException.AddData(
                key: "failedListenerEventV2Ids",
                values: faultedListenerEventV2.Id.ToString());

            // Step 2
            this.archivingEventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.RetrieveBatchOfDeadEventV2sAsync(randomCancellationToken))
                        .ReturnsAsync((IEnumerable<EventV2>)retrievedEventV2s);

            // Step 3
            this.eventArchiveV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.BulkAddEventArchiveV2sAsync(
                        It.Is(SameEventArchiveV2sAs(expectedEventArchiveV2s)),
                        randomCancellationToken))
                            .ReturnsAsync(addedEventArchiveV2s);

            // Step 4
            this.archivingEventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.RetrieveBatchOfListenerEventV2sAsync(
                        It.Is(SameEventV2IdsAs(inputEventV2Ids)),
                        randomCancellationToken))
                            .ReturnsAsync((IEnumerable<ListenerEventV2>)retrievedListenerEventV2s);

            // Step 5 (one listener archive fails -> only clean one added)
            this.eventArchiveV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.BulkAddListenerEventArchiveV2sAsync(
                        It.Is(SameListenerEventArchiveV2sAs(expectedListenerEventArchiveV2s)),
                        randomCancellationToken))
                            .ReturnsAsync(addedListenerEventArchiveV2s);

            // Step 6 (only the clean listener is removed)
            this.archivingEventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.BulkRemoveListenerEventV2sAsync(
                        It.Is(SameListenerEventV2sAs(expectedRemovableListenerEventV2s)),
                        randomCancellationToken))
                            .Returns(ValueTask.CompletedTask);

            // Step 7 (faulted event pruned -> only clean event id remains, no listeners left)
            this.archivingEventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.RetrieveBatchOfListenerEventV2sAsync(
                        It.Is(SameEventV2IdsAs(remainingEventV2Ids)),
                        randomCancellationToken))
                            .ReturnsAsync(Enumerable.Empty<ListenerEventV2>());

            // Step 8 (only the clean event is removed)
            this.archivingEventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.BulkRemoveEventV2sAsync(
                        It.Is(SameEventV2sAs(expectedRemovableEventV2s)),
                        randomCancellationToken))
                            .Returns(ValueTask.CompletedTask);

            // Step 9 (faulted event still dead -> returned again, now faulted -> outer breaks)
            this.archivingEventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.RetrieveBatchOfDeadEventV2sAsync(randomCancellationToken))
                        .ReturnsAsync((IEnumerable<EventV2>)faultedDeadEventV2s);

            // when
            await this.archivingEventV2CoordinationService
                .ArchiveDeadEventV2sAsync(randomCancellationToken);

            // then
            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfDeadEventV2sAsync(randomCancellationToken),
                    Times.Exactly(2));

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.BulkAddEventArchiveV2sAsync(
                    It.Is(SameEventArchiveV2sAs(expectedEventArchiveV2s)),
                    randomCancellationToken),
                        Times.Once);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfListenerEventV2sAsync(
                    It.Is(SameEventV2IdsAs(inputEventV2Ids)),
                    randomCancellationToken),
                        Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    It.Is(SameListenerEventArchiveV2sAs(expectedListenerEventArchiveV2s)),
                    randomCancellationToken),
                        Times.Once);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.BulkRemoveListenerEventV2sAsync(
                    It.Is(SameListenerEventV2sAs(expectedRemovableListenerEventV2s)),
                    randomCancellationToken),
                        Times.Once);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfListenerEventV2sAsync(
                    It.Is(SameEventV2IdsAs(remainingEventV2Ids)),
                    randomCancellationToken),
                        Times.Once);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.BulkRemoveEventV2sAsync(
                    It.Is(SameEventV2sAs(expectedRemovableEventV2s)),
                    randomCancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedFailedArchivingEventV2CoordinationException))),
                        Times.Once);

            this.archivingEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldRetainEventV2sWithUnarchivableEventV2sAndLogAggregateExceptionAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var mockSequence = new MockSequence();

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            Guid cleanEventV2Id = GetRandomId();
            Guid faultedEventV2Id = GetRandomId();

            var cleanEventV2 = new EventV2
            {
                Id = cleanEventV2Id,
                Content = GetRandomString(),
                EventName = GetRandomString(),
                Type = GetRandomEnum<EventTypeV2>(),
                CreatedDate = randomDateTimeOffset,
                UpdatedDate = randomDateTimeOffset,
                ScheduledDate = randomDateTimeOffset,
                RemainingRetryAttempts = GetRandomNumber(),
                EventAddressId = GetRandomId()
            };

            var faultedEventV2 = new EventV2
            {
                Id = faultedEventV2Id,
                Content = GetRandomString(),
                EventName = GetRandomString(),
                Type = GetRandomEnum<EventTypeV2>(),
                CreatedDate = randomDateTimeOffset,
                UpdatedDate = randomDateTimeOffset,
                ScheduledDate = randomDateTimeOffset,
                RemainingRetryAttempts = GetRandomNumber(),
                EventAddressId = GetRandomId()
            };

            List<EventV2> retrievedEventV2s =
                new List<EventV2> { cleanEventV2, faultedEventV2 };

            var cleanListenerEventV2 = new ListenerEventV2
            {
                Id = GetRandomId(),
                Status = GetRandomEnum<ListenerEventStatusV2>(),
                Response = GetRandomString(),
                ResponseCode = GetRandomString(),
                ResponseMessage = GetRandomString(),
                CreatedDate = randomDateTimeOffset,
                UpdatedDate = randomDateTimeOffset,
                EventId = cleanEventV2Id,
                EventAddressId = GetRandomId(),
                EventListenerId = GetRandomId()
            };

            List<ListenerEventV2> retrievedListenerEventV2s =
                new List<ListenerEventV2> { cleanListenerEventV2 };

            IEnumerable<Guid> archivedEventV2Ids =
                new List<Guid> { cleanEventV2Id };

            List<EventV2> expectedRemovableEventV2s =
                new List<EventV2> { cleanEventV2 };

            List<ListenerEventV2> expectedRemovableListenerEventV2s =
                new List<ListenerEventV2> { cleanListenerEventV2 };

            List<EventArchiveV2> expectedEventArchiveV2s =
                retrievedEventV2s.Select(eventV2 =>
                    new EventArchiveV2
                    {
                        Id = eventV2.Id,
                        Content = eventV2.Content,
                        EventName = eventV2.EventName,
                        Type = (EventArchiveTypeV2)eventV2.Type,
                        CreatedDate = eventV2.CreatedDate,
                        UpdatedDate = eventV2.UpdatedDate,
                        ScheduledDate = eventV2.ScheduledDate,
                        RemainingRetryAttempts = eventV2.RemainingRetryAttempts,
                        EventAddressId = eventV2.EventAddressId
                    }).ToList();

            IEnumerable<EventArchiveV2> addedEventArchiveV2s =
                expectedEventArchiveV2s
                    .Where(eventArchiveV2 => eventArchiveV2.Id == cleanEventV2Id).ToList();

            List<ListenerEventArchiveV2> expectedListenerEventArchiveV2s =
                retrievedListenerEventV2s.Select(listenerEventV2 =>
                    new ListenerEventArchiveV2
                    {
                        Id = listenerEventV2.Id,
                        Status = (ListenerEventArchiveStatusV2)listenerEventV2.Status,
                        Response = listenerEventV2.Response,
                        ResponseCode = listenerEventV2.ResponseCode,
                        ResponseMessage = listenerEventV2.ResponseMessage,
                        CreatedDate = listenerEventV2.CreatedDate,
                        UpdatedDate = listenerEventV2.UpdatedDate,
                        EventId = listenerEventV2.EventId,
                        EventAddressId = listenerEventV2.EventAddressId,
                        EventListenerId = listenerEventV2.EventListenerId,
                        EventArchiveV2Id = listenerEventV2.EventId
                    }).ToList();

            IEnumerable<ListenerEventArchiveV2> addedListenerEventArchiveV2s =
                expectedListenerEventArchiveV2s;

            var expectedFailedArchivingEventV2CoordinationException =
                new FailedArchivingEventV2CoordinationException(
                    message: "Some dead events could not be fully archived " +
                        "and were retained for the next run.");

            expectedFailedArchivingEventV2CoordinationException.AddData(
                key: "failedEventV2Ids",
                values: faultedEventV2Id.ToString());

            // Step 2
            this.archivingEventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.RetrieveBatchOfDeadEventV2sAsync(randomCancellationToken))
                        .ReturnsAsync((IEnumerable<EventV2>)retrievedEventV2s);

            // Step 3 (one event archive fails -> only clean one added)
            this.eventArchiveV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.BulkAddEventArchiveV2sAsync(
                        It.Is(SameEventArchiveV2sAs(expectedEventArchiveV2s)),
                        randomCancellationToken))
                            .ReturnsAsync(addedEventArchiveV2s);

            // Step 4
            this.archivingEventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.RetrieveBatchOfListenerEventV2sAsync(
                        It.Is(SameEventV2IdsAs(archivedEventV2Ids)),
                        randomCancellationToken))
                            .ReturnsAsync((IEnumerable<ListenerEventV2>)retrievedListenerEventV2s);

            // Step 5
            this.eventArchiveV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.BulkAddListenerEventArchiveV2sAsync(
                        It.Is(SameListenerEventArchiveV2sAs(expectedListenerEventArchiveV2s)),
                        randomCancellationToken))
                            .ReturnsAsync(addedListenerEventArchiveV2s);

            // Step 6
            this.archivingEventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.BulkRemoveListenerEventV2sAsync(
                        It.Is(SameListenerEventV2sAs(expectedRemovableListenerEventV2s)),
                        randomCancellationToken))
                            .Returns(ValueTask.CompletedTask);

            // Step 7 (no more listeners for the clean event)
            this.archivingEventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.RetrieveBatchOfListenerEventV2sAsync(
                        It.Is(SameEventV2IdsAs(archivedEventV2Ids)),
                        randomCancellationToken))
                            .ReturnsAsync(Enumerable.Empty<ListenerEventV2>());

            // Step 8 (only the clean event is removed)
            this.archivingEventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.BulkRemoveEventV2sAsync(
                        It.Is(SameEventV2sAs(expectedRemovableEventV2s)),
                        randomCancellationToken))
                            .Returns(ValueTask.CompletedTask);

            // Step 9 (faulted event still dead -> returned again, now faulted -> outer breaks)
            this.archivingEventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.RetrieveBatchOfDeadEventV2sAsync(randomCancellationToken))
                        .ReturnsAsync((IEnumerable<EventV2>)new List<EventV2> { faultedEventV2 });

            // when
            await this.archivingEventV2CoordinationService
                .ArchiveDeadEventV2sAsync(randomCancellationToken);

            // then
            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfDeadEventV2sAsync(randomCancellationToken),
                    Times.Exactly(2));

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.BulkAddEventArchiveV2sAsync(
                    It.Is(SameEventArchiveV2sAs(expectedEventArchiveV2s)),
                    randomCancellationToken),
                        Times.Once);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfListenerEventV2sAsync(
                    It.Is(SameEventV2IdsAs(archivedEventV2Ids)),
                    randomCancellationToken),
                        Times.Exactly(2));

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    It.Is(SameListenerEventArchiveV2sAs(expectedListenerEventArchiveV2s)),
                    randomCancellationToken),
                        Times.Once);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.BulkRemoveListenerEventV2sAsync(
                    It.Is(SameListenerEventV2sAs(expectedRemovableListenerEventV2s)),
                    randomCancellationToken),
                        Times.Once);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.BulkRemoveEventV2sAsync(
                    It.Is(SameEventV2sAs(expectedRemovableEventV2s)),
                    randomCancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedFailedArchivingEventV2CoordinationException))),
                        Times.Once);

            this.archivingEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldRetainEventV2sWithUnarchivableListenerEventV2sAndLogAggregateExceptionAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var mockSequence = new MockSequence();

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            Guid eventV2Id = GetRandomId();

            var deadEventV2 = new EventV2
            {
                Id = eventV2Id,
                Content = GetRandomString(),
                EventName = GetRandomString(),
                Type = GetRandomEnum<EventTypeV2>(),
                CreatedDate = randomDateTimeOffset,
                UpdatedDate = randomDateTimeOffset,
                ScheduledDate = randomDateTimeOffset,
                RemainingRetryAttempts = GetRandomNumber(),
                EventAddressId = GetRandomId()
            };

            List<EventV2> retrievedEventV2s =
                new List<EventV2> { deadEventV2 };

            var listenerEventV2 = new ListenerEventV2
            {
                Id = GetRandomId(),
                Status = GetRandomEnum<ListenerEventStatusV2>(),
                Response = GetRandomString(),
                ResponseCode = GetRandomString(),
                ResponseMessage = GetRandomString(),
                CreatedDate = randomDateTimeOffset,
                UpdatedDate = randomDateTimeOffset,
                EventId = eventV2Id,
                EventAddressId = GetRandomId(),
                EventListenerId = GetRandomId()
            };

            List<ListenerEventV2> retrievedListenerEventV2s =
                new List<ListenerEventV2> { listenerEventV2 };

            IEnumerable<Guid> inputEventV2Ids =
                new List<Guid> { eventV2Id };

            List<EventArchiveV2> expectedEventArchiveV2s =
                new List<EventArchiveV2>
                {
                    new EventArchiveV2
                    {
                        Id = deadEventV2.Id,
                        Content = deadEventV2.Content,
                        EventName = deadEventV2.EventName,
                        Type = (EventArchiveTypeV2)deadEventV2.Type,
                        CreatedDate = deadEventV2.CreatedDate,
                        UpdatedDate = deadEventV2.UpdatedDate,
                        ScheduledDate = deadEventV2.ScheduledDate,
                        RemainingRetryAttempts = deadEventV2.RemainingRetryAttempts,
                        EventAddressId = deadEventV2.EventAddressId
                    }
                };

            List<ListenerEventArchiveV2> expectedListenerEventArchiveV2s =
                new List<ListenerEventArchiveV2>
                {
                    new ListenerEventArchiveV2
                    {
                        Id = listenerEventV2.Id,
                        Status = (ListenerEventArchiveStatusV2)listenerEventV2.Status,
                        Response = listenerEventV2.Response,
                        ResponseCode = listenerEventV2.ResponseCode,
                        ResponseMessage = listenerEventV2.ResponseMessage,
                        CreatedDate = listenerEventV2.CreatedDate,
                        UpdatedDate = listenerEventV2.UpdatedDate,
                        EventId = listenerEventV2.EventId,
                        EventAddressId = listenerEventV2.EventAddressId,
                        EventListenerId = listenerEventV2.EventListenerId,
                        EventArchiveV2Id = listenerEventV2.EventId
                    }
                };

            IEnumerable<EventArchiveV2> addedEventArchiveV2s =
                expectedEventArchiveV2s;

            IEnumerable<ListenerEventArchiveV2> addedListenerEventArchiveV2s =
                new List<ListenerEventArchiveV2>();

            var expectedFailedArchivingEventV2CoordinationException =
                new FailedArchivingEventV2CoordinationException(
                    message: "Some dead events could not be fully archived " +
                        "and were retained for the next run.");

            expectedFailedArchivingEventV2CoordinationException.AddData(
                key: "failedListenerEventV2Ids",
                values: listenerEventV2.Id.ToString());

            // Step 2
            this.archivingEventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.RetrieveBatchOfDeadEventV2sAsync(randomCancellationToken))
                        .ReturnsAsync((IEnumerable<EventV2>)retrievedEventV2s);

            // Step 3
            this.eventArchiveV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.BulkAddEventArchiveV2sAsync(
                        It.Is(SameEventArchiveV2sAs(expectedEventArchiveV2s)),
                        randomCancellationToken))
                            .ReturnsAsync(addedEventArchiveV2s);

            // Step 4
            this.archivingEventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.RetrieveBatchOfListenerEventV2sAsync(
                        It.Is(SameEventV2IdsAs(inputEventV2Ids)),
                        randomCancellationToken))
                            .ReturnsAsync((IEnumerable<ListenerEventV2>)retrievedListenerEventV2s);

            // Step 5 (listener archive fails -> none added)
            this.eventArchiveV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.BulkAddListenerEventArchiveV2sAsync(
                        It.Is(SameListenerEventArchiveV2sAs(expectedListenerEventArchiveV2s)),
                        randomCancellationToken))
                            .ReturnsAsync(addedListenerEventArchiveV2s);

            // Step 6 (event still dead -> returned again, now faulted -> outer breaks)
            this.archivingEventV2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.RetrieveBatchOfDeadEventV2sAsync(randomCancellationToken))
                        .ReturnsAsync((IEnumerable<EventV2>)retrievedEventV2s);

            // when
            await this.archivingEventV2CoordinationService
                .ArchiveDeadEventV2sAsync(randomCancellationToken);

            // then
            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfDeadEventV2sAsync(randomCancellationToken),
                    Times.Exactly(2));

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.BulkAddEventArchiveV2sAsync(
                    It.Is(SameEventArchiveV2sAs(expectedEventArchiveV2s)),
                    randomCancellationToken),
                        Times.Once);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfListenerEventV2sAsync(
                    It.Is(SameEventV2IdsAs(inputEventV2Ids)),
                    randomCancellationToken),
                        Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    It.Is(SameListenerEventArchiveV2sAs(expectedListenerEventArchiveV2s)),
                    randomCancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedFailedArchivingEventV2CoordinationException))),
                        Times.Once);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.BulkRemoveListenerEventV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventV2>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.BulkRemoveEventV2sAsync(
                    It.IsAny<IEnumerable<EventV2>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.archivingEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
