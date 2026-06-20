// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventListeners.V2
{
    public partial class EventListenerV2ServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventListenerV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<EventListenerV2> randomEventListenerV2s =
                CreateRandomEventListenerV2s();

            IQueryable<EventListenerV2> retrievedEventListenerV2s =
                randomEventListenerV2s;

            IQueryable<EventListenerV2> expectedEventListenerV2s =
                randomEventListenerV2s.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventListenerV2sAsync(randomCancellationToken))
                    .ReturnsAsync(retrievedEventListenerV2s);

            // when
            IQueryable<EventListenerV2> actualEventListenerV2s =
                await this.eventListenerV2Service
                    .RetrieveAllEventListenerV2sAsync(randomCancellationToken);

            // then
            actualEventListenerV2s.Should().BeEquivalentTo(
                expectedEventListenerV2s);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventListenerV2sAsync(randomCancellationToken),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
