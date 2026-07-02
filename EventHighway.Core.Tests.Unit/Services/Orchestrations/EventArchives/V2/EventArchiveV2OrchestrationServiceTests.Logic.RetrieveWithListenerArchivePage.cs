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
        public async Task ShouldRetrieveEventArchiveV2WithListenerEventArchiveV2sByIdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid inputEventArchiveId = GetRandomId();
            Guid otherEventArchiveId = GetRandomId();

            var matchingListenerEventArchiveV2 = new ListenerEventArchiveV2
            {
                Id = GetRandomId(),
                EventArchiveV2Id = inputEventArchiveId,
                EventAddressV2Id = GetRandomId(),
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

            IQueryable<ListenerEventArchiveV2> storedListenerEventArchiveV2s =
                new List<ListenerEventArchiveV2>
                {
                    matchingListenerEventArchiveV2,
                    otherListenerEventArchiveV2
                }
                    .AsQueryable();

            var expectedEventArchiveV2 = new EventArchiveV2
            {
                Id = inputEventArchiveId,
                ListenerEventArchiveV2s =
                    new List<ListenerEventArchiveV2> { matchingListenerEventArchiveV2 }
            };

            this.listenerEventArchiveV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(storedListenerEventArchiveV2s);

            // when
            EventArchiveV2 actualEventArchiveV2 =
                await this.eventArchiveV2OrchestrationService
                    .RetrieveEventArchiveV2WithListenerEventArchiveV2sByIdAsync(
                        eventArchiveId: inputEventArchiveId,
                        eventListenerIds: null,
                        startDate: null,
                        endDate: null,
                        skip: 0,
                        take: 10,
                        randomCancellationToken);

            // then
            actualEventArchiveV2.Should()
                .BeEquivalentTo(expectedEventArchiveV2);

            this.listenerEventArchiveV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldRetrieveEventArchiveV2WithListenerEventArchiveV2sByIdWithPagingAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid inputEventArchiveId = GetRandomId();
            DateTimeOffset baseCreatedDate = GetRandomDateTimeOffset();

            var firstListenerEventArchiveV2 = new ListenerEventArchiveV2
            {
                Id = GetRandomId(),
                EventArchiveV2Id = inputEventArchiveId,
                EventAddressV2Id = GetRandomId(),
                EventListenerV2Id = GetRandomId(),
                CreatedDate = baseCreatedDate
            };

            var secondListenerEventArchiveV2 = new ListenerEventArchiveV2
            {
                Id = GetRandomId(),
                EventArchiveV2Id = inputEventArchiveId,
                EventAddressV2Id = GetRandomId(),
                EventListenerV2Id = GetRandomId(),
                CreatedDate = baseCreatedDate.AddDays(1)
            };

            var thirdListenerEventArchiveV2 = new ListenerEventArchiveV2
            {
                Id = GetRandomId(),
                EventArchiveV2Id = inputEventArchiveId,
                EventAddressV2Id = GetRandomId(),
                EventListenerV2Id = GetRandomId(),
                CreatedDate = baseCreatedDate.AddDays(2)
            };

            IQueryable<ListenerEventArchiveV2> storedListenerEventArchiveV2s =
                new List<ListenerEventArchiveV2>
                {
                    thirdListenerEventArchiveV2,
                    firstListenerEventArchiveV2,
                    secondListenerEventArchiveV2
                }
                    .AsQueryable();

            var expectedEventArchiveV2 = new EventArchiveV2
            {
                Id = inputEventArchiveId,
                ListenerEventArchiveV2s =
                    new List<ListenerEventArchiveV2> { secondListenerEventArchiveV2 }
            };

            this.listenerEventArchiveV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(storedListenerEventArchiveV2s);

            // when
            EventArchiveV2 actualEventArchiveV2 =
                await this.eventArchiveV2OrchestrationService
                    .RetrieveEventArchiveV2WithListenerEventArchiveV2sByIdAsync(
                        eventArchiveId: inputEventArchiveId,
                        eventListenerIds: null,
                        startDate: null,
                        endDate: null,
                        skip: 1,
                        take: 1,
                        randomCancellationToken);

            // then
            actualEventArchiveV2.Should()
                .BeEquivalentTo(expectedEventArchiveV2);

            this.listenerEventArchiveV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldRetrieveEventArchiveV2WithListenerEventArchiveV2sByIdFilteredByEndDateAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid inputEventArchiveId = GetRandomId();

            DateTimeOffset inputEndDate = GetRandomDateTimeOffset();
            DateTimeOffset onOrBeforeEndDate = inputEndDate.AddDays(-1);
            DateTimeOffset afterEndDate = inputEndDate.AddDays(1);

            var matchingListenerEventArchiveV2 = new ListenerEventArchiveV2
            {
                Id = GetRandomId(),
                EventArchiveV2Id = inputEventArchiveId,
                EventAddressV2Id = GetRandomId(),
                EventListenerV2Id = GetRandomId(),
                CreatedDate = onOrBeforeEndDate
            };

            var otherListenerEventArchiveV2 = new ListenerEventArchiveV2
            {
                Id = GetRandomId(),
                EventArchiveV2Id = inputEventArchiveId,
                EventAddressV2Id = GetRandomId(),
                EventListenerV2Id = GetRandomId(),
                CreatedDate = afterEndDate
            };

            IQueryable<ListenerEventArchiveV2> storedListenerEventArchiveV2s =
                new List<ListenerEventArchiveV2>
                {
                    matchingListenerEventArchiveV2,
                    otherListenerEventArchiveV2
                }
                    .AsQueryable();

            var expectedEventArchiveV2 = new EventArchiveV2
            {
                Id = inputEventArchiveId,
                ListenerEventArchiveV2s =
                    new List<ListenerEventArchiveV2> { matchingListenerEventArchiveV2 }
            };

            this.listenerEventArchiveV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(storedListenerEventArchiveV2s);

            // when
            EventArchiveV2 actualEventArchiveV2 =
                await this.eventArchiveV2OrchestrationService
                    .RetrieveEventArchiveV2WithListenerEventArchiveV2sByIdAsync(
                        eventArchiveId: inputEventArchiveId,
                        eventListenerIds: null,
                        startDate: null,
                        endDate: inputEndDate,
                        skip: 0,
                        take: 10,
                        randomCancellationToken);

            // then
            actualEventArchiveV2.Should()
                .BeEquivalentTo(expectedEventArchiveV2);

            this.listenerEventArchiveV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldRetrieveEventArchiveV2WithListenerEventArchiveV2sByIdFilteredByStartDateAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid inputEventArchiveId = GetRandomId();

            DateTimeOffset inputStartDate = GetRandomDateTimeOffset();
            DateTimeOffset onOrAfterStartDate = inputStartDate.AddDays(1);
            DateTimeOffset beforeStartDate = inputStartDate.AddDays(-1);

            var matchingListenerEventArchiveV2 = new ListenerEventArchiveV2
            {
                Id = GetRandomId(),
                EventArchiveV2Id = inputEventArchiveId,
                EventAddressV2Id = GetRandomId(),
                EventListenerV2Id = GetRandomId(),
                CreatedDate = onOrAfterStartDate
            };

            var otherListenerEventArchiveV2 = new ListenerEventArchiveV2
            {
                Id = GetRandomId(),
                EventArchiveV2Id = inputEventArchiveId,
                EventAddressV2Id = GetRandomId(),
                EventListenerV2Id = GetRandomId(),
                CreatedDate = beforeStartDate
            };

            IQueryable<ListenerEventArchiveV2> storedListenerEventArchiveV2s =
                new List<ListenerEventArchiveV2>
                {
                    matchingListenerEventArchiveV2,
                    otherListenerEventArchiveV2
                }
                    .AsQueryable();

            var expectedEventArchiveV2 = new EventArchiveV2
            {
                Id = inputEventArchiveId,
                ListenerEventArchiveV2s =
                    new List<ListenerEventArchiveV2> { matchingListenerEventArchiveV2 }
            };

            this.listenerEventArchiveV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(storedListenerEventArchiveV2s);

            // when
            EventArchiveV2 actualEventArchiveV2 =
                await this.eventArchiveV2OrchestrationService
                    .RetrieveEventArchiveV2WithListenerEventArchiveV2sByIdAsync(
                        eventArchiveId: inputEventArchiveId,
                        eventListenerIds: null,
                        startDate: inputStartDate,
                        endDate: null,
                        skip: 0,
                        take: 10,
                        randomCancellationToken);

            // then
            actualEventArchiveV2.Should()
                .BeEquivalentTo(expectedEventArchiveV2);

            this.listenerEventArchiveV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldRetrieveEventArchiveV2WithListenerEventArchiveV2sByIdFilteredByEventListenerIdsAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid inputEventArchiveId = GetRandomId();
            Guid inputEventListenerId = GetRandomId();

            var matchingListenerEventArchiveV2 = new ListenerEventArchiveV2
            {
                Id = GetRandomId(),
                EventArchiveV2Id = inputEventArchiveId,
                EventAddressV2Id = GetRandomId(),
                EventListenerV2Id = inputEventListenerId,
                CreatedDate = GetRandomDateTimeOffset()
            };

            var otherListenerEventArchiveV2 = new ListenerEventArchiveV2
            {
                Id = GetRandomId(),
                EventArchiveV2Id = inputEventArchiveId,
                EventAddressV2Id = GetRandomId(),
                EventListenerV2Id = GetRandomId(),
                CreatedDate = GetRandomDateTimeOffset()
            };

            IQueryable<ListenerEventArchiveV2> storedListenerEventArchiveV2s =
                new List<ListenerEventArchiveV2>
                {
                    matchingListenerEventArchiveV2,
                    otherListenerEventArchiveV2
                }
                    .AsQueryable();

            List<Guid> inputEventListenerIds =
                new List<Guid> { inputEventListenerId };

            var expectedEventArchiveV2 = new EventArchiveV2
            {
                Id = inputEventArchiveId,
                ListenerEventArchiveV2s =
                    new List<ListenerEventArchiveV2> { matchingListenerEventArchiveV2 }
            };

            this.listenerEventArchiveV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(storedListenerEventArchiveV2s);

            // when
            EventArchiveV2 actualEventArchiveV2 =
                await this.eventArchiveV2OrchestrationService
                    .RetrieveEventArchiveV2WithListenerEventArchiveV2sByIdAsync(
                        eventArchiveId: inputEventArchiveId,
                        eventListenerIds: inputEventListenerIds,
                        startDate: null,
                        endDate: null,
                        skip: 0,
                        take: 10,
                        randomCancellationToken);

            // then
            actualEventArchiveV2.Should()
                .BeEquivalentTo(expectedEventArchiveV2);

            this.listenerEventArchiveV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
