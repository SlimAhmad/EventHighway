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

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEvents.V2
{
    public partial class ListenerEventV2ServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveListenerEventV2sByEventIdsAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<Guid> randomEventIds =
                Enumerable.Range(0, GetRandomNumber())
                    .Select(_ => Guid.NewGuid())
                    .ToList();

            List<Guid> inputEventIds = randomEventIds;

            IQueryable<ListenerEventV2> allListenerEventV2s =
                CreateRandomListenerEventV2s();

            foreach (ListenerEventV2 listenerEventV2 in allListenerEventV2s)
                listenerEventV2.EventV2Id = inputEventIds[0];

            IQueryable<ListenerEventV2> expectedListenerEventV2s =
                allListenerEventV2s.Where(
                    listenerEvent => inputEventIds.Contains(listenerEvent.EventV2Id));

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(allListenerEventV2s);

            // when
            IQueryable<ListenerEventV2> actualListenerEventV2s =
                await this.listenerEventV2Service
                    .RetrieveListenerEventV2sByEventIdsAsync(inputEventIds, randomCancellationToken);

            // then
            actualListenerEventV2s.Should().BeEquivalentTo(expectedListenerEventV2s);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllListenerEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
