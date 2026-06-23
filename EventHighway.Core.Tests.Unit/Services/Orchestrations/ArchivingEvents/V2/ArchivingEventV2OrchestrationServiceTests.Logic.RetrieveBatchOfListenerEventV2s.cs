// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.BatchProcessings;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ArchivingEvents.V2
{
    public partial class ArchivingEventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveBatchOfListenerEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken = TestContext.Current.CancellationToken;
            BatchConfiguration randomBatchConfiguration = CreateRandomBatchConfiguration();
            BatchConfiguration retrievedBatchConfiguration = randomBatchConfiguration;
            int inputTake = retrievedBatchConfiguration.BatchSizeForBulkProcessing;

            IEnumerable<Guid> randomEventV2Ids =
                Enumerable.Range(0, GetRandomNumber()).Select(_ => Guid.NewGuid()).ToList();

            IEnumerable<Guid> inputEventV2Ids = randomEventV2Ids;

            IEnumerable<ListenerEventV2> randomListenerEventV2s = CreateRandomListenerEventV2s();
            IEnumerable<ListenerEventV2> retrievedListenerEventV2s = randomListenerEventV2s;
            IEnumerable<ListenerEventV2> expectedListenerEventV2s = retrievedListenerEventV2s;

            var mockSequence = new MockSequence();

            this.configurationBrokerMock
                .InSequence(mockSequence)
                .Setup(broker => broker.GetBatchConfiguration())
                    .Returns(retrievedBatchConfiguration);

            this.listenerEventV2ProcessingServiceMock
                .InSequence(mockSequence)
                .Setup(service => service.RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                    inputEventV2Ids,
                    inputTake,
                    randomCancellationToken))
                        .ReturnsAsync(retrievedListenerEventV2s);

            // when
            IEnumerable<ListenerEventV2> actualListenerEventV2s =
                await this.archivingEventV2OrchestrationService
                    .RetrieveBatchOfListenerEventV2sAsync(
                        inputEventV2Ids,
                        randomCancellationToken);

            // then
            actualListenerEventV2s.Should().BeEquivalentTo(expectedListenerEventV2s);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfListenerEventV2sByEventIdsAsync(
                    inputEventV2Ids,
                    inputTake,
                    randomCancellationToken),
                        Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
