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

namespace EventHighway.Core.Tests.Unit.Clients.ListenerEvents.V2
{
    public partial class ListenerEventV2ClientTests
    {
        [Fact]
        public async Task ShouldRetrieveAllListenerEventV2sWithEventListenerV2Async()
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

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sWithEventListenerV2Async(
                    randomCancellationToken))
                        .ReturnsAsync(retrievedListenerEventV2s);

            // when
            IQueryable<ListenerEventV2> actualListenerEventV2s =
                await this.listenerEventV2Client
                    .RetrieveAllListenerEventV2sWithEventListenerV2Async(
                        randomCancellationToken);

            // then
            actualListenerEventV2s.Should().BeEquivalentTo(expectedListenerEventV2s);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sWithEventListenerV2Async(
                    randomCancellationToken),
                        Times.Once);

            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
