// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.ReplayingEvents.V2
{
    public partial class ReplayingEventV2ClientTests
    {
        [Fact]
        public async Task ShouldProcessReplayedListenerEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            this.replayingEventV2CoordinationServiceMock.Setup(service =>
                service.ProcessReplayedListenerEventV2sAsync(randomCancellationToken))
                    .Returns(ValueTask.CompletedTask);

            // when
            await this.replayingEventV2Client
                .ProcessReplayedListenerEventV2sAsync(randomCancellationToken);

            // then
            this.replayingEventV2CoordinationServiceMock.Verify(service =>
                service.ProcessReplayedListenerEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.replayingEventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
