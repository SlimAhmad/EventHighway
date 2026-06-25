// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEvents.V2
{
    public partial class ListenerEventV2ServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllReplayListenerEventV2sWithNavigationsWhenTakeIsZeroAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int take = 0;

            List<ListenerEventV2> randomListenerEventV2List =
                CreateRandomListenerEventV2s().ToList();

            randomListenerEventV2List.ForEach(
                listenerEventV2 => listenerEventV2.Status = ListenerEventStatusV2.Replay);

            IQueryable<ListenerEventV2> randomListenerEventV2s = randomListenerEventV2List.AsQueryable();
            IEnumerable<ListenerEventV2> expectedListenerEventV2s = randomListenerEventV2s.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllListenerEventV2sWithEventV2WithEventListenerV2Async(
                    randomCancellationToken))
                        .ReturnsAsync(randomListenerEventV2s);

            // when
            IEnumerable<ListenerEventV2> actualListenerEventV2s =
                await this.listenerEventV2Service
                    .RetrieveReplayBatchListenerEventV2sWithEventWithEventListenerAsync(
                        take,
                        randomCancellationToken);

            // then
            actualListenerEventV2s.Should().BeEquivalentTo(expectedListenerEventV2s);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllListenerEventV2sWithEventV2WithEventListenerV2Async(
                    randomCancellationToken),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldRetrieveBatchOfReplayListenerEventV2sWithNavigationsAndTakeAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int take = 2;

            List<ListenerEventV2> randomListenerEventV2List =
                CreateRandomListenerEventV2s().ToList();

            randomListenerEventV2List.ForEach(
                listenerEventV2 => listenerEventV2.Status = ListenerEventStatusV2.Replay);

            IQueryable<ListenerEventV2> randomListenerEventV2s = randomListenerEventV2List.AsQueryable();

            IEnumerable<ListenerEventV2> expectedListenerEventV2s =
                randomListenerEventV2s.Take(take).DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllListenerEventV2sWithEventV2WithEventListenerV2Async(
                    randomCancellationToken))
                        .ReturnsAsync(randomListenerEventV2s);

            // when
            IEnumerable<ListenerEventV2> actualListenerEventV2s =
                await this.listenerEventV2Service
                    .RetrieveReplayBatchListenerEventV2sWithEventWithEventListenerAsync(
                        take,
                        randomCancellationToken);

            // then
            actualListenerEventV2s.Should().BeEquivalentTo(expectedListenerEventV2s);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllListenerEventV2sWithEventV2WithEventListenerV2Async(
                    randomCancellationToken),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
