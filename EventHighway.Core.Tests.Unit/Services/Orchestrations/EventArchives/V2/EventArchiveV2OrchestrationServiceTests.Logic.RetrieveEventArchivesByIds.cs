// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task ShouldRetrieveEventArchiveV2sByIdsAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<EventArchiveV2> matchingEventArchiveV2s =
                CreateRandomEventArchiveV2s().ToList();

            List<EventArchiveV2> otherEventArchiveV2s =
                CreateRandomEventArchiveV2s().ToList();

            IEnumerable<Guid> inputEventArchiveIds =
                matchingEventArchiveV2s.Select(eventArchiveV2 => eventArchiveV2.Id).ToList();

            IQueryable<EventArchiveV2> storageEventArchiveV2s =
                matchingEventArchiveV2s
                    .Concat(otherEventArchiveV2s)
                        .AsQueryable();

            IEnumerable<EventArchiveV2> expectedEventArchiveV2s = matchingEventArchiveV2s;

            this.eventArchiveV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(storageEventArchiveV2s);

            // when
            IEnumerable<EventArchiveV2> actualEventArchiveV2s =
                await this.eventArchiveV2OrchestrationService
                    .RetrieveEventArchiveV2sByIdsAsync(
                        inputEventArchiveIds,
                        randomCancellationToken);

            // then
            actualEventArchiveV2s.Should().BeEquivalentTo(expectedEventArchiveV2s);

            this.eventArchiveV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.listenerEventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
