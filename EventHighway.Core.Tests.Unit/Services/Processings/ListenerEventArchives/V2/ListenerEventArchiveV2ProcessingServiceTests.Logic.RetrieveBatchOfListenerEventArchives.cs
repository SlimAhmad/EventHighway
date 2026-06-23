// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEventArchives.V2
{
    public partial class ListenerEventArchiveV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveBatchOfListenerEventArchiveV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int skip = GetRandomNumber();
            int take = GetRandomNumber();

            IQueryable<ListenerEventArchiveV2> storageListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s(count: skip + take + GetRandomNumber());

            List<ListenerEventArchiveV2> expectedListenerEventArchiveV2s =
                storageListenerEventArchiveV2s
                    .OrderBy(listenerEventArchiveV2 => listenerEventArchiveV2.CreatedDate)
                    .ThenBy(listenerEventArchiveV2 => listenerEventArchiveV2.Id)
                    .Skip(skip)
                    .Take(take)
                    .ToList();

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(storageListenerEventArchiveV2s);

            // when
            List<ListenerEventArchiveV2> actualListenerEventArchiveV2s =
                await this.listenerEventArchiveV2ProcessingService
                    .RetrieveBatchOfListenerEventArchiveV2sAsync(
                        eventAddressId: null,
                        eventListenerIds: null,
                        startDate: null,
                        endDate: null,
                        skip: skip,
                        take: take,
                        cancellationToken: randomCancellationToken);

            // then
            actualListenerEventArchiveV2s.Should().BeEquivalentTo(
                expectedListenerEventArchiveV2s,
                options => options.WithStrictOrdering());

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveBatchOfListenerEventArchiveV2sFilteredByEventAddressIdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid eventAddressId = GetRandomId();

            List<ListenerEventArchiveV2> matchingListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s().ToList();

            matchingListenerEventArchiveV2s.ForEach(listenerEventArchiveV2 =>
                listenerEventArchiveV2.EventAddressId = eventAddressId);

            List<ListenerEventArchiveV2> otherListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s().ToList();

            IQueryable<ListenerEventArchiveV2> storageListenerEventArchiveV2s =
                matchingListenerEventArchiveV2s
                    .Concat(otherListenerEventArchiveV2s)
                        .AsQueryable();

            int skip = 0;
            int take = storageListenerEventArchiveV2s.Count();

            List<ListenerEventArchiveV2> expectedListenerEventArchiveV2s =
                matchingListenerEventArchiveV2s
                    .OrderBy(listenerEventArchiveV2 => listenerEventArchiveV2.CreatedDate)
                    .ThenBy(listenerEventArchiveV2 => listenerEventArchiveV2.Id)
                    .Skip(skip)
                    .Take(take)
                    .ToList();

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(storageListenerEventArchiveV2s);

            // when
            List<ListenerEventArchiveV2> actualListenerEventArchiveV2s =
                await this.listenerEventArchiveV2ProcessingService
                    .RetrieveBatchOfListenerEventArchiveV2sAsync(
                        eventAddressId: eventAddressId,
                        eventListenerIds: null,
                        startDate: null,
                        endDate: null,
                        skip: skip,
                        take: take,
                        cancellationToken: randomCancellationToken);

            // then
            actualListenerEventArchiveV2s.Should().BeEquivalentTo(
                expectedListenerEventArchiveV2s,
                options => options.WithStrictOrdering());

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveBatchOfListenerEventArchiveV2sFilteredByEventListenerIdsAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid firstEventListenerId = GetRandomId();
            Guid secondEventListenerId = GetRandomId();

            List<Guid> eventListenerIds =
                new List<Guid> { firstEventListenerId, secondEventListenerId };

            List<ListenerEventArchiveV2> firstMatchingListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s().ToList();

            firstMatchingListenerEventArchiveV2s.ForEach(listenerEventArchiveV2 =>
                listenerEventArchiveV2.EventListenerId = firstEventListenerId);

            List<ListenerEventArchiveV2> secondMatchingListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s().ToList();

            secondMatchingListenerEventArchiveV2s.ForEach(listenerEventArchiveV2 =>
                listenerEventArchiveV2.EventListenerId = secondEventListenerId);

            List<ListenerEventArchiveV2> otherListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s().ToList();

            IQueryable<ListenerEventArchiveV2> storageListenerEventArchiveV2s =
                firstMatchingListenerEventArchiveV2s
                    .Concat(secondMatchingListenerEventArchiveV2s)
                    .Concat(otherListenerEventArchiveV2s)
                        .AsQueryable();

            int skip = 0;
            int take = storageListenerEventArchiveV2s.Count();

            List<ListenerEventArchiveV2> expectedListenerEventArchiveV2s =
                firstMatchingListenerEventArchiveV2s
                    .Concat(secondMatchingListenerEventArchiveV2s)
                    .OrderBy(listenerEventArchiveV2 => listenerEventArchiveV2.CreatedDate)
                    .ThenBy(listenerEventArchiveV2 => listenerEventArchiveV2.Id)
                    .Skip(skip)
                    .Take(take)
                    .ToList();

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(storageListenerEventArchiveV2s);

            // when
            List<ListenerEventArchiveV2> actualListenerEventArchiveV2s =
                await this.listenerEventArchiveV2ProcessingService
                    .RetrieveBatchOfListenerEventArchiveV2sAsync(
                        eventAddressId: null,
                        eventListenerIds: eventListenerIds,
                        startDate: null,
                        endDate: null,
                        skip: skip,
                        take: take,
                        cancellationToken: randomCancellationToken);

            // then
            actualListenerEventArchiveV2s.Should().BeEquivalentTo(
                expectedListenerEventArchiveV2s,
                options => options.WithStrictOrdering());

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveBatchOfListenerEventArchiveV2sFilteredByCreatedDateWindowAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset baseDate = GetRandomDateTimeOffset();

            List<ListenerEventArchiveV2> listenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s(count: 6).ToList();

            for (int index = 0; index < listenerEventArchiveV2s.Count; index++)
            {
                listenerEventArchiveV2s[index].CreatedDate = baseDate.AddDays(index);
            }

            DateTimeOffset startDate = baseDate.AddDays(1);
            DateTimeOffset endDate = baseDate.AddDays(4);

            IQueryable<ListenerEventArchiveV2> storageListenerEventArchiveV2s =
                listenerEventArchiveV2s.AsQueryable();

            int skip = 0;
            int take = listenerEventArchiveV2s.Count;

            List<ListenerEventArchiveV2> expectedListenerEventArchiveV2s =
                listenerEventArchiveV2s
                    .Where(listenerEventArchiveV2 =>
                        listenerEventArchiveV2.CreatedDate >= startDate
                            && listenerEventArchiveV2.CreatedDate <= endDate)
                    .OrderBy(listenerEventArchiveV2 => listenerEventArchiveV2.CreatedDate)
                    .ThenBy(listenerEventArchiveV2 => listenerEventArchiveV2.Id)
                    .Skip(skip)
                    .Take(take)
                    .ToList();

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(storageListenerEventArchiveV2s);

            // when
            List<ListenerEventArchiveV2> actualListenerEventArchiveV2s =
                await this.listenerEventArchiveV2ProcessingService
                    .RetrieveBatchOfListenerEventArchiveV2sAsync(
                        eventAddressId: null,
                        eventListenerIds: null,
                        startDate: startDate,
                        endDate: endDate,
                        skip: skip,
                        take: take,
                        cancellationToken: randomCancellationToken);

            // then
            actualListenerEventArchiveV2s.Should().BeEquivalentTo(
                expectedListenerEventArchiveV2s,
                options => options.WithStrictOrdering());

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
