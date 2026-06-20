// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
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
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<EventV2> randomScheduledEventV2s =
                CreateRandomEventV2s(
                    dates: GetRandomDateTimeOffset(),
                    eventV2Type: EventTypeV2.Scheduled)
                        .ToList();

            List<EventV2> randomImmediateEventV2sWithRetries =
                CreateRandomEventV2s(
                    dates: GetRandomDateTimeOffset(),
                    eventV2Type: EventTypeV2.Immediate)
                        .ToList();

            randomImmediateEventV2sWithRetries.ForEach(eventV2 =>
                eventV2.RemainingRetryAttempts = GetRandomNumber());

            List<EventV2> randomDeadEventV2s =
                CreateRandomEventV2s(
                    dates: GetRandomDateTimeOffset(),
                    eventV2Type: EventTypeV2.Immediate)
                        .ToList();

            randomDeadEventV2s.ForEach(eventV2 =>
            {
                eventV2.RemainingRetryAttempts = 0;
                eventV2.ListenerEventV2s = new List<ListenerEventV2>();
            });

            IQueryable<EventV2> retrievedEventV2s =
                randomScheduledEventV2s
                    .Union(randomImmediateEventV2sWithRetries)
                    .Union(randomDeadEventV2s)
                        .AsQueryable();

            IQueryable<EventV2> expectedEventV2s =
                randomDeadEventV2s.AsQueryable();

            this.eventV2ServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(retrievedEventV2s);

            // when
            IQueryable<EventV2> actualEventV2s =
                await this.eventV2ProcessingService
                    .RetrieveAllDeadEventV2sWithListenersAsync(randomCancellationToken);

            // then
            actualEventV2s.Should().BeEquivalentTo(expectedEventV2s);

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
