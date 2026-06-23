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

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V2
{
    public partial class EventArchiveV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveBatchOfListenerEventArchiveV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid inputEventAddressId = Guid.NewGuid();
            IEnumerable<Guid> inputEventListenerIds = new List<Guid> { Guid.NewGuid() };
            DateTimeOffset inputStartDate = GetRandomDateTimeOffset();
            DateTimeOffset inputEndDate = GetRandomDateTimeOffset();
            int inputSkip = GetRandomNumber();
            int inputTake = GetRandomNumber();

            List<ListenerEventArchiveV2> randomListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s().ToList();

            List<ListenerEventArchiveV2> retrievedListenerEventArchiveV2s =
                randomListenerEventArchiveV2s;

            IEnumerable<ListenerEventArchiveV2> expectedListenerEventArchiveV2s =
                retrievedListenerEventArchiveV2s;

            this.listenerEventArchiveV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfListenerEventArchiveV2sAsync(
                    inputEventAddressId,
                    inputEventListenerIds,
                    inputStartDate,
                    inputEndDate,
                    inputSkip,
                    inputTake,
                    randomCancellationToken))
                        .ReturnsAsync(retrievedListenerEventArchiveV2s);

            // when
            IEnumerable<ListenerEventArchiveV2> actualListenerEventArchiveV2s =
                await this.eventArchiveV2OrchestrationService
                    .RetrieveBatchOfListenerEventArchiveV2sAsync(
                        inputEventAddressId,
                        inputEventListenerIds,
                        inputStartDate,
                        inputEndDate,
                        inputSkip,
                        inputTake,
                        randomCancellationToken);

            // then
            actualListenerEventArchiveV2s.Should().BeEquivalentTo(expectedListenerEventArchiveV2s);

            this.listenerEventArchiveV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfListenerEventArchiveV2sAsync(
                    inputEventAddressId,
                    inputEventListenerIds,
                    inputStartDate,
                    inputEndDate,
                    inputSkip,
                    inputTake,
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
