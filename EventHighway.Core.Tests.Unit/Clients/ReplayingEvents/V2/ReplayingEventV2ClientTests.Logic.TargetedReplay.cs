// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.ReplayingEvents.V2
{
    public partial class ReplayingEventV2ClientTests
    {
        [Fact]
        public async Task ShouldTargetedReplayEventArchiveV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid randomEventV2Id = Guid.NewGuid();
            Guid? randomEventAddressId = Guid.NewGuid();
            IEnumerable<Guid> randomEventListenerIds = new List<Guid> { Guid.NewGuid() };
            bool randomAllowReplayOfQuarantinedItem = true;

            this.replayingEventV2CoordinationServiceMock.Setup(service =>
                service.ReplayEventArchiveV2sAsync(
                    randomEventV2Id,
                    randomEventAddressId,
                    randomEventListenerIds,
                    randomAllowReplayOfQuarantinedItem,
                    randomCancellationToken))
                .Returns(ValueTask.CompletedTask);

            // when
            await this.replayingEventV2Client.ReplayEventArchiveV2sAsync(
                randomEventV2Id,
                randomEventAddressId,
                randomEventListenerIds,
                randomAllowReplayOfQuarantinedItem,
                randomCancellationToken);

            // then
            this.replayingEventV2CoordinationServiceMock.Verify(service =>
                service.ReplayEventArchiveV2sAsync(
                    randomEventV2Id,
                    randomEventAddressId,
                    randomEventListenerIds,
                    randomAllowReplayOfQuarantinedItem,
                    randomCancellationToken),
                Times.Once);

            this.replayingEventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
