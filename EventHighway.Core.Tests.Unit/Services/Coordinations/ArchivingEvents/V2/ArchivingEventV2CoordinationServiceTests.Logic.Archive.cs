// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
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
            var mockSequence = new MockSequence();

            DateTimeOffset randomDateTimeOffset =
                GetRandomDateTimeOffset();

            DateTimeOffset retrievedDateTimeOffset =
                randomDateTimeOffset;

            List<dynamic> randomListenerEventV2sProperties =
                CreateRandomListenerEventV2sProperties();

            List<dynamic> randomEventV2sProperties =
                CreateRandomEventV2sProperties();

            ICollection<ListenerEventV2> retrievedListenerEventV2s =
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

            IQueryable<EventV2> retrievedEventV2s =
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
                        EventAddressId = item.EventAddressId,
                        ListenerEventV2s = retrievedListenerEventV2s
                    }).AsQueryable();

            List<ListenerEventArchiveV1> mappedListenerEventArchiveV1s =
                randomListenerEventV2sProperties.Select(item =>
                    new ListenerEventArchiveV1
                    {
                        Id = item.Id,
                        Status = (ListenerEventArchiveStatusV1)item.Status,
                        Response = item.Response,
                        ResponseReasonPhrase = item.ResponseMessage,
                        CreatedDate = item.CreatedDate,
                        UpdatedDate = item.UpdatedDate,
                        ArchivedDate = retrievedDateTimeOffset,
                        EventId = item.EventId,
                        EventAddressId = item.EventAddressId,
                        EventListenerId = item.EventListenerId
                    }).ToList();

            List<EventArchiveV1> mappedEventArchiveV1s =
                randomEventV2sProperties.Select(item =>
                    new EventArchiveV1
                    {
                        Id = item.Id,
                        Content = item.Content,
                        Type = (EventArchiveTypeV1)item.Type,
                        CreatedDate = item.CreatedDate,
                        UpdatedDate = item.UpdatedDate,
                        ScheduledDate = item.ScheduledDate,
                        EventAddressId = item.EventAddressId,
                        ArchivedDate = retrievedDateTimeOffset,
                        ListenerEventArchiveV1s = mappedListenerEventArchiveV1s
                    }).ToList();

            this.archivingEvent2OrchestrationServiceMock
                .InSequence(mockSequence).Setup(service =>
                    service.RetrieveAllDeadEventV2sWithListenersAsync(
                        It.IsAny<CancellationToken>()))
                            .ReturnsAsync(retrievedEventV2s);

            foreach ((EventArchiveV1 mappedEventArchiveV1, EventV2 retrievedEventV2)
                in mappedEventArchiveV1s.Zip(retrievedEventV2s))
            {
                this.dateTimeBrokerMock
                    .InSequence(mockSequence).Setup(broker =>
                        broker.GetDateTimeOffsetAsync())
                            .ReturnsAsync(retrievedDateTimeOffset);

                this.eventArchiveV1OrchestrationServiceMock
                    .InSequence(mockSequence).Setup(service =>
                        service.AddEventArchiveV1WithListenerEventArchiveV1sAsync(
                            It.Is(SameEventArchiveV1As(mappedEventArchiveV1))))
                                .Returns(ValueTask.CompletedTask);

                this.archivingEvent2OrchestrationServiceMock
                    .InSequence(mockSequence).Setup(service =>
                        service.RemoveEventV2AndListenerEventV2sAsync(
                            It.Is(SameEventV2As(retrievedEventV2)),
                            It.IsAny<CancellationToken>()))
                                .Returns(ValueTask.CompletedTask);
            }

            // when
            await this.archivingEventV1CoordinationService
                .ArchiveDeadEventV2sAsync(TestContext.Current.CancellationToken);

            // then
            this.archivingEvent2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllDeadEventV2sWithListenersAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Exactly(mappedEventArchiveV1s.Count));

            foreach ((EventArchiveV1 mappedEventArchiveV1, EventV2 retrievedEventV2)
                in mappedEventArchiveV1s.Zip(retrievedEventV2s))
            {
                this.eventArchiveV1OrchestrationServiceMock.Verify(service =>
                    service.AddEventArchiveV1WithListenerEventArchiveV1sAsync(
                        It.Is(SameEventArchiveV1As(mappedEventArchiveV1))),
                            Times.Once);

                this.archivingEvent2OrchestrationServiceMock.Verify(service =>
                    service.RemoveEventV2AndListenerEventV2sAsync(
                        It.Is(SameEventV2As(retrievedEventV2)),
                        It.IsAny<CancellationToken>()),
                            Times.Once);
            }

            this.archivingEvent2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV1OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
