// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V2
{
    public partial class EventArchiveV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveBatchOfEventArchiveV2sMatchingByEventAddressIdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid inputEventAddressId = GetRandomId();

            Guid matchingEventArchiveId = GetRandomId();
            Guid otherEventArchiveId = GetRandomId();

            var matchingEventArchiveV2 = new EventArchiveV2
            {
                Id = matchingEventArchiveId,
                CreatedDate = GetRandomDateTimeOffset()
            };

            var otherEventArchiveV2 = new EventArchiveV2
            {
                Id = otherEventArchiveId,
                CreatedDate = GetRandomDateTimeOffset()
            };

            var matchingListenerEventArchiveV2 = new ListenerEventArchiveV2
            {
                Id = GetRandomId(),
                EventArchiveV2Id = matchingEventArchiveId,
                EventAddressV2Id = inputEventAddressId,
                EventListenerV2Id = GetRandomId(),
                CreatedDate = GetRandomDateTimeOffset()
            };

            var otherListenerEventArchiveV2 = new ListenerEventArchiveV2
            {
                Id = GetRandomId(),
                EventArchiveV2Id = otherEventArchiveId,
                EventAddressV2Id = GetRandomId(),
                EventListenerV2Id = GetRandomId(),
                CreatedDate = GetRandomDateTimeOffset()
            };

            IQueryable<EventArchiveV2> storedEventArchiveV2s =
                new List<EventArchiveV2> { matchingEventArchiveV2, otherEventArchiveV2 }
                    .AsQueryable();

            IQueryable<ListenerEventArchiveV2> storedListenerEventArchiveV2s =
                new List<ListenerEventArchiveV2>
                {
                    matchingListenerEventArchiveV2,
                    otherListenerEventArchiveV2
                }
                    .AsQueryable();

            List<EventArchiveV2> expectedEventArchiveV2s =
                new List<EventArchiveV2> { matchingEventArchiveV2 };

            this.listenerEventArchiveV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(storedListenerEventArchiveV2s);

            this.eventArchiveV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(storedEventArchiveV2s);

            // when
            IEnumerable<EventArchiveV2> actualEventArchiveV2s =
                await this.eventArchiveV2OrchestrationService
                    .RetrieveBatchOfEventArchiveV2sMatchingAsync(
                        eventAddressId: inputEventAddressId,
                        eventListenerIds: null,
                        startDate: null,
                        endDate: null,
                        skip: 0,
                        take: 10,
                        randomCancellationToken);

            // then
            actualEventArchiveV2s.Should()
                .BeEquivalentTo(expectedEventArchiveV2s);

            this.listenerEventArchiveV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventArchiveV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveBatchOfEventArchiveV2sMatchingByStartDateAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset inputStartDate = GetRandomDateTimeOffset();
            DateTimeOffset onOrAfterStartDate = inputStartDate.AddDays(1);
            DateTimeOffset beforeStartDate = inputStartDate.AddDays(-1);

            Guid matchingEventArchiveId = GetRandomId();
            Guid otherEventArchiveId = GetRandomId();

            var matchingEventArchiveV2 = new EventArchiveV2
            {
                Id = matchingEventArchiveId,
                CreatedDate = GetRandomDateTimeOffset()
            };

            var otherEventArchiveV2 = new EventArchiveV2
            {
                Id = otherEventArchiveId,
                CreatedDate = GetRandomDateTimeOffset()
            };

            var matchingListenerEventArchiveV2 = new ListenerEventArchiveV2
            {
                Id = GetRandomId(),
                EventArchiveV2Id = matchingEventArchiveId,
                EventAddressV2Id = GetRandomId(),
                EventListenerV2Id = GetRandomId(),
                CreatedDate = onOrAfterStartDate
            };

            var otherListenerEventArchiveV2 = new ListenerEventArchiveV2
            {
                Id = GetRandomId(),
                EventArchiveV2Id = otherEventArchiveId,
                EventAddressV2Id = GetRandomId(),
                EventListenerV2Id = GetRandomId(),
                CreatedDate = beforeStartDate
            };

            IQueryable<EventArchiveV2> storedEventArchiveV2s =
                new List<EventArchiveV2> { matchingEventArchiveV2, otherEventArchiveV2 }
                    .AsQueryable();

            IQueryable<ListenerEventArchiveV2> storedListenerEventArchiveV2s =
                new List<ListenerEventArchiveV2>
                {
                    matchingListenerEventArchiveV2,
                    otherListenerEventArchiveV2
                }
                    .AsQueryable();

            List<EventArchiveV2> expectedEventArchiveV2s =
                new List<EventArchiveV2> { matchingEventArchiveV2 };

            this.listenerEventArchiveV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(storedListenerEventArchiveV2s);

            this.eventArchiveV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(storedEventArchiveV2s);

            // when
            IEnumerable<EventArchiveV2> actualEventArchiveV2s =
                await this.eventArchiveV2OrchestrationService
                    .RetrieveBatchOfEventArchiveV2sMatchingAsync(
                        eventAddressId: null,
                        eventListenerIds: null,
                        startDate: inputStartDate,
                        endDate: null,
                        skip: 0,
                        take: 10,
                        randomCancellationToken);

            // then
            actualEventArchiveV2s.Should()
                .BeEquivalentTo(expectedEventArchiveV2s);

            this.listenerEventArchiveV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventArchiveV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveBatchOfEventArchiveV2sMatchingByEventListenerIdsAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid inputEventListenerId = GetRandomId();

            Guid matchingEventArchiveId = GetRandomId();
            Guid otherEventArchiveId = GetRandomId();

            var matchingEventArchiveV2 = new EventArchiveV2
            {
                Id = matchingEventArchiveId,
                CreatedDate = GetRandomDateTimeOffset()
            };

            var otherEventArchiveV2 = new EventArchiveV2
            {
                Id = otherEventArchiveId,
                CreatedDate = GetRandomDateTimeOffset()
            };

            var matchingListenerEventArchiveV2 = new ListenerEventArchiveV2
            {
                Id = GetRandomId(),
                EventArchiveV2Id = matchingEventArchiveId,
                EventAddressV2Id = GetRandomId(),
                EventListenerV2Id = inputEventListenerId,
                CreatedDate = GetRandomDateTimeOffset()
            };

            var otherListenerEventArchiveV2 = new ListenerEventArchiveV2
            {
                Id = GetRandomId(),
                EventArchiveV2Id = otherEventArchiveId,
                EventAddressV2Id = GetRandomId(),
                EventListenerV2Id = GetRandomId(),
                CreatedDate = GetRandomDateTimeOffset()
            };

            IQueryable<EventArchiveV2> storedEventArchiveV2s =
                new List<EventArchiveV2> { matchingEventArchiveV2, otherEventArchiveV2 }
                    .AsQueryable();

            IQueryable<ListenerEventArchiveV2> storedListenerEventArchiveV2s =
                new List<ListenerEventArchiveV2>
                {
                    matchingListenerEventArchiveV2,
                    otherListenerEventArchiveV2
                }
                    .AsQueryable();

            List<Guid> inputEventListenerIds =
                new List<Guid> { inputEventListenerId };

            List<EventArchiveV2> expectedEventArchiveV2s =
                new List<EventArchiveV2> { matchingEventArchiveV2 };

            this.listenerEventArchiveV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(storedListenerEventArchiveV2s);

            this.eventArchiveV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(storedEventArchiveV2s);

            // when
            IEnumerable<EventArchiveV2> actualEventArchiveV2s =
                await this.eventArchiveV2OrchestrationService
                    .RetrieveBatchOfEventArchiveV2sMatchingAsync(
                        eventAddressId: null,
                        eventListenerIds: inputEventListenerIds,
                        startDate: null,
                        endDate: null,
                        skip: 0,
                        take: 10,
                        randomCancellationToken);

            // then
            actualEventArchiveV2s.Should()
                .BeEquivalentTo(expectedEventArchiveV2s);

            this.listenerEventArchiveV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventArchiveV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
