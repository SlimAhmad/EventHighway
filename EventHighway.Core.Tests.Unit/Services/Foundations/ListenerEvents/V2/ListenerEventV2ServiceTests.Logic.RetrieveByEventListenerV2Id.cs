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
        public async Task ShouldRetrieveListenerEventV2sByEventListenerV2IdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid inputEventListenerV2Id = GetRandomId();

            List<ListenerEventV2> allListenerEventV2List =
                CreateRandomListenerEventV2s().ToList();

            for (int index = 0; index < allListenerEventV2List.Count; index++)
            {
                allListenerEventV2List[index].EventListenerV2Id =
                    index % 2 == 0 ? inputEventListenerV2Id : GetRandomId();
            }

            IQueryable<ListenerEventV2> allListenerEventV2s =
                allListenerEventV2List.AsQueryable();

            IQueryable<ListenerEventV2> expectedListenerEventV2s =
                allListenerEventV2s.Where(listenerEventV2 =>
                    listenerEventV2.EventListenerV2Id == inputEventListenerV2Id);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(allListenerEventV2s);

            // when
            IQueryable<ListenerEventV2> actualListenerEventV2s =
                await this.listenerEventV2Service
                    .RetrieveListenerEventV2sByEventListenerV2IdAsync(
                        inputEventListenerV2Id,
                        randomCancellationToken);

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
