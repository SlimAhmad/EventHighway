// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V2
{
    public partial class EventArchiveV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveBatchOfEventArchiveV2sOlderThanAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset inputOlderThan = randomDateTimeOffset;
            int inputTake = GetRandomNumber();

            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEnumerable<EventArchiveV2> randomEventArchiveV2s =
                CreateRandomEventArchiveV2s();

            IEnumerable<EventArchiveV2> retrievedEventArchiveV2s = randomEventArchiveV2s;
            IEnumerable<EventArchiveV2> expectedEventArchiveV2s = retrievedEventArchiveV2s;

            this.eventArchiveV2ProcessingServiceMock.Setup(service =>
                service.RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                    inputOlderThan,
                    inputTake,
                    randomCancellationToken))
                        .ReturnsAsync(retrievedEventArchiveV2s);

            // when
            IEnumerable<EventArchiveV2> actualEventArchiveV2s =
                await this.eventArchiveV2OrchestrationService
                    .RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                        inputOlderThan,
                        inputTake,
                        randomCancellationToken);

            // then
            actualEventArchiveV2s.Should().BeEquivalentTo(expectedEventArchiveV2s);

            this.eventArchiveV2ProcessingServiceMock.Verify(service =>
                service.RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                    inputOlderThan,
                    inputTake,
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
