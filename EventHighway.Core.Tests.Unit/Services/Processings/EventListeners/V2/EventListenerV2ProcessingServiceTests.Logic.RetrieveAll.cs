// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventListeners.V2
{
    public partial class EventListenerV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventListenerV2sAsync()
        {
            // given
            IQueryable<EventListenerV2> randomEventListenerV2s = CreateRandomEventListenerV2s();
            IQueryable<EventListenerV2> retrievedEventListenerV2s = randomEventListenerV2s;
            IQueryable<EventListenerV2> expectedEventListenerV2s = randomEventListenerV2s.DeepClone();

            this.eventListenerV2ServiceMock.Setup(service =>
                service.RetrieveAllEventListenerV2sAsync())
                    .ReturnsAsync(retrievedEventListenerV2s);

            // when
            IQueryable<EventListenerV2> actualEventListenerV2s =
                await this.eventListenerV2ProcessingService.RetrieveAllEventListenerV2sAsync();

            // then
            actualEventListenerV2s.Should().BeEquivalentTo(expectedEventListenerV2s);

            this.eventListenerV2ServiceMock.Verify(service =>
                service.RetrieveAllEventListenerV2sAsync(),
                    Times.Once);

            this.eventListenerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
