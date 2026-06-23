// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.Events.V2
{
    public partial class EventV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldBulkRestoreEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEnumerable<EventV2> randomEventV2s = CreateRandomEventV2s();
            IEnumerable<EventV2> inputEventV2s = randomEventV2s;
            IEnumerable<EventV2> returnedEventV2s = randomEventV2s;
            IEnumerable<EventV2> expectedEventV2s = returnedEventV2s;

            this.eventV2ServiceMock.Setup(service =>
                service.BulkRestoreEventV2sAsync(
                    inputEventV2s,
                    randomCancellationToken))
                        .ReturnsAsync(returnedEventV2s);

            // when
            IEnumerable<EventV2> actualEventV2s =
                await this.eventV2ProcessingService.BulkRestoreEventV2sAsync(
                    inputEventV2s,
                    randomCancellationToken);

            // then
            actualEventV2s.Should().BeEquivalentTo(expectedEventV2s);

            this.eventV2ServiceMock.Verify(service =>
                service.BulkRestoreEventV2sAsync(
                    inputEventV2s,
                    randomCancellationToken),
                        Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
