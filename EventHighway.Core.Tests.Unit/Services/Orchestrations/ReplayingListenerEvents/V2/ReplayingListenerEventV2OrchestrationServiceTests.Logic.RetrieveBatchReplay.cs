// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ReplayingListenerEvents.V2
{
    public partial class ReplayingListenerEventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveBatchOfReplayListenerEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int randomTake = GetRandomNumber();

            List<ListenerEventV2> randomListenerEventV2s =
                CreateListenerEventV2Filler().Create(count: randomTake).ToList();

            IEnumerable<ListenerEventV2> expectedListenerEventV2s = randomListenerEventV2s;

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfReplayListenerEventV2sAsync(randomTake, randomCancellationToken))
                    .ReturnsAsync(randomListenerEventV2s);

            // when
            IEnumerable<ListenerEventV2> actualListenerEventV2s =
                await this.replayingListenerEventV2OrchestrationService
                    .RetrieveBatchOfReplayListenerEventV2sAsync(randomTake, randomCancellationToken);

            // then
            actualListenerEventV2s.Should().BeEquivalentTo(expectedListenerEventV2s);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfReplayListenerEventV2sAsync(randomTake, randomCancellationToken),
                    Times.Once);

            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
