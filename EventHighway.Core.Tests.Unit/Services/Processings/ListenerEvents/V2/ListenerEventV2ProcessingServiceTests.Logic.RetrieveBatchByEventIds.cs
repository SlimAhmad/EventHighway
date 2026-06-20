// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEvents.V2
{
    public partial class ListenerEventV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveBatchOfListenerEventV2sByEventIdsWithoutTakeAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<Guid> someEventIds =
                Enumerable.Range(0, GetRandomNumber())
                    .Select(_ => Guid.NewGuid())
                        .ToList();

            int inputTake = 0;

            IQueryable<ListenerEventV2> retrievedListenerEventV2s =
                CreateRandomListenerEventV2s();

            IEnumerable<ListenerEventV2> expectedListenerEventV2s =
                retrievedListenerEventV2s.AsEnumerable();

            this.listenerEventV2ServiceMock.Setup(service =>
                service.RetrieveListenerEventV2sByEventIdsAsync(someEventIds, randomCancellationToken))
                    .ReturnsAsync(retrievedListenerEventV2s);

            // when
            IEnumerable<ListenerEventV2> actualListenerEventV2s =
                await this.listenerEventV2ProcessingService
                    .RetrieveBatchOfListenerEventV2sByEventIdsAsync(someEventIds, inputTake, randomCancellationToken);

            // then
            actualListenerEventV2s.Should().BeEquivalentTo(expectedListenerEventV2s);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.RetrieveListenerEventV2sByEventIdsAsync(someEventIds, randomCancellationToken),
                    Times.Once);

            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveBatchOfListenerEventV2sByEventIdsWithTakeAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<Guid> someEventIds =
                Enumerable.Range(0, GetRandomNumber())
                    .Select(_ => Guid.NewGuid())
                        .ToList();

            int randomTake = GetRandomNumber();
            int inputTake = randomTake;

            IQueryable<ListenerEventV2> retrievedListenerEventV2s =
                CreateRandomListenerEventV2s();

            IEnumerable<ListenerEventV2> expectedListenerEventV2s =
                retrievedListenerEventV2s.Take(inputTake);

            this.listenerEventV2ServiceMock.Setup(service =>
                service.RetrieveListenerEventV2sByEventIdsAsync(someEventIds, randomCancellationToken))
                    .ReturnsAsync(retrievedListenerEventV2s);

            // when
            IEnumerable<ListenerEventV2> actualListenerEventV2s =
                await this.listenerEventV2ProcessingService
                    .RetrieveBatchOfListenerEventV2sByEventIdsAsync(someEventIds, inputTake, randomCancellationToken);

            // then
            actualListenerEventV2s.Should().BeEquivalentTo(expectedListenerEventV2s);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.RetrieveListenerEventV2sByEventIdsAsync(someEventIds, randomCancellationToken),
                    Times.Once);

            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
