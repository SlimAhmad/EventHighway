// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ListenerEvents.V2
{
    public partial class ListenerEventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveBatchOfListenerEventV2sByEventIdsAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int randomTake = GetRandomNumber();
            int inputTake = randomTake;

            IEnumerable<Guid> randomEventV2Ids = CreateRandomEventV2Ids();
            IEnumerable<Guid> inputEventV2Ids = randomEventV2Ids;

            IEnumerable<ListenerEventV2> randomListenerEventV2s = CreateRandomListenerEventV2s();
            IEnumerable<ListenerEventV2> retrievedListenerEventV2s = randomListenerEventV2s;
            IEnumerable<ListenerEventV2> expectedListenerEventV2s = retrievedListenerEventV2s;

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                    inputEventV2Ids,
                    inputTake,
                    randomCancellationToken))
                        .ReturnsAsync(retrievedListenerEventV2s);

            // when
            IEnumerable<ListenerEventV2> actualListenerEventV2s =
                await this.listenerEventV2OrchestrationService
                    .RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                        inputEventV2Ids,
                        inputTake,
                        randomCancellationToken);

            // then
            actualListenerEventV2s.Should().BeEquivalentTo(expectedListenerEventV2s);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                    inputEventV2Ids,
                    inputTake,
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
