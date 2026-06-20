// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.Events.V2
{
    public partial class EventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventAddressV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<EventAddressV2> randomEventAddressV2s = CreateRandomEventAddressV2s();
            IQueryable<EventAddressV2> retrievedEventAddressV2s = randomEventAddressV2s;
            IQueryable<EventAddressV2> expectedEventAddressV2s = randomEventAddressV2s.DeepClone();

            this.eventAddressV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken))
                    .ReturnsAsync(retrievedEventAddressV2s);

            // when
            IQueryable<EventAddressV2> actualEventAddressV2s =
                await this.eventV2OrchestrationService
                    .RetrieveAllEventAddressV2sAsync(randomCancellationToken);

            // then
            actualEventAddressV2s.Should().BeEquivalentTo(expectedEventAddressV2s);

            this.eventAddressV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventAddressV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
