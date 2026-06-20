// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventArchives.V2
{
    public partial class EventArchiveV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveBatchOfEventArchiveV2sOlderThanWithoutTakeAsync()
        {
            // given
            DateTimeOffset randomOlderThan = GetRandomDateTimeOffset();
            DateTimeOffset inputOlderThan = randomOlderThan;
            int inputTake = 0;

            IQueryable<EventArchiveV2> allEventArchiveV2s =
                CreateRandomEventArchiveV2s();

            IEnumerable<EventArchiveV2> expectedEventArchiveV2s =
                allEventArchiveV2s
                    .Where(archive => archive.ArchivedDate < inputOlderThan)
                    .AsEnumerable();

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync())
                    .ReturnsAsync(allEventArchiveV2s);

            // when
            IEnumerable<EventArchiveV2> actualEventArchiveV2s =
                await this.eventArchiveV2ProcessingService
                    .RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                        inputOlderThan,
                        inputTake);

            // then
            actualEventArchiveV2s.Should().BeEquivalentTo(expectedEventArchiveV2s);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(),
                    Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
        [Fact]
        public async Task ShouldRetrieveBatchOfEventArchiveV2sOlderThanWithTakeAsync()
        {
            // given
            DateTimeOffset randomOlderThan = GetRandomDateTimeOffset();
            DateTimeOffset inputOlderThan = randomOlderThan;
            int randomTake = GetRandomNumber();
            int inputTake = randomTake;

            IQueryable<EventArchiveV2> allEventArchiveV2s =
                CreateRandomEventArchiveV2s();

            IEnumerable<EventArchiveV2> expectedEventArchiveV2s =
                allEventArchiveV2s
                    .Where(archive => archive.ArchivedDate < inputOlderThan)
                    .Take(inputTake)
                    .AsEnumerable();

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync())
                    .ReturnsAsync(allEventArchiveV2s);

            // when
            IEnumerable<EventArchiveV2> actualEventArchiveV2s =
                await this.eventArchiveV2ProcessingService
                    .RetrieveBatchOfEventArchiveV2sOlderThanAsync(
                        inputOlderThan,
                        inputTake);

            // then
            actualEventArchiveV2s.Should().BeEquivalentTo(expectedEventArchiveV2s);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(),
                    Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
