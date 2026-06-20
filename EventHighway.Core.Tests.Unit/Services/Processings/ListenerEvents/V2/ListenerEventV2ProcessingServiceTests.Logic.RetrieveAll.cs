// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEvents.V2
{
    public partial class ListenerEventV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllListenerEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<ListenerEventV2> randomListenerEventV2s =
                CreateRandomListenerEventV2s();

            IQueryable<ListenerEventV2> retrievedListenerEventV2s =
                randomListenerEventV2s;

            IQueryable<ListenerEventV2> expectedListenerEventV2s =
                retrievedListenerEventV2s.DeepClone();

            this.listenerEventV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken))
                    .ReturnsAsync(retrievedListenerEventV2s);

            // when
            IQueryable<ListenerEventV2> actualListenerEventV2s =
                await this.listenerEventV2ProcessingService
                    .RetrieveAllListenerEventV2sAsync(randomCancellationToken);

            // then
            actualListenerEventV2s.Should().BeEquivalentTo(
                expectedListenerEventV2s);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
