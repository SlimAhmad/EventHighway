// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.Events.V2
{
    public partial class EventV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllDeadEventV2sWithListenersAsync()
        {
            // given
            List<EventV2> randomScheduledEventV2s =
                CreateRandomEventV2s(
                    dates: GetRandomDateTimeOffset(),
                    eventV2Type: EventTypeV2.Scheduled)
                        .ToList();

            List<EventV2> randomImmediateEventV2s =
                CreateRandomEventV2s(
                    dates: GetRandomDateTimeOffset(),
                    eventV2Type: EventTypeV2.Immediate)
                        .ToList();

            IQueryable<EventV2> retrievedEventV2s =
                randomScheduledEventV2s.Union(
                    randomImmediateEventV2s)
                        .AsQueryable();

            IQueryable<EventV2> expectedEventV2s =
                randomImmediateEventV2s.AsQueryable();

            this.eventV2ServiceMock.Setup(service =>
                service.RetrieveAllEventV2sWithListenerEventV2sAsync())
                    .ReturnsAsync(retrievedEventV2s);

            // when
            IQueryable<EventV2> actualEventV2s =
                await this.eventV2ProcessingService
                    .RetrieveAllDeadEventV2sWithListenersAsync();

            // then
            actualEventV2s.Should().BeEquivalentTo(expectedEventV2s);

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveAllEventV2sWithListenerEventV2sAsync(),
                    Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
