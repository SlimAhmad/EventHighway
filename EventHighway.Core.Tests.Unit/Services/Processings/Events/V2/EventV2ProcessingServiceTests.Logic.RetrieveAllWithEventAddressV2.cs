// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.Events.V2
{
    public partial class EventV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventV2sWithEventAddressV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<EventV2> randomEventV2s = CreateRandomEventV2s();
            IQueryable<EventV2> retrievedEventV2s = randomEventV2s;
            IQueryable<EventV2> expectedEventV2s = randomEventV2s.DeepClone();

            this.eventV2ServiceMock.Setup(service =>
                service.RetrieveAllEventV2sWithEventAddressV2Async(randomCancellationToken))
                    .ReturnsAsync(retrievedEventV2s);

            // when
            IQueryable<EventV2> actualEventV2s =
                await this.eventV2ProcessingService
                    .RetrieveAllEventV2sWithEventAddressV2Async(randomCancellationToken);

            // then
            actualEventV2s.Should().BeEquivalentTo(expectedEventV2s);

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveAllEventV2sWithEventAddressV2Async(randomCancellationToken),
                    Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
