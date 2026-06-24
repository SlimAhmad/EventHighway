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
        public async Task ShouldReplayEventArchiveV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid? randomEventAddressId = Guid.NewGuid();
            IEnumerable<Guid> randomEventListenerIds = new List<Guid> { Guid.NewGuid() };
            DateTimeOffset? randomStartDate = GetRandomDateTimeOffset();
            DateTimeOffset? randomEndDate = randomStartDate.Value.AddDays(1);

            this.replayingEventV2CoordinationServiceMock.Setup(service =>
                service.ReplayEventArchiveV2sAsync(
                    randomEventAddressId,
                    randomEventListenerIds,
                    randomStartDate,
                    randomEndDate,
                    randomCancellationToken))
                .Returns(ValueTask.CompletedTask);

            // when
            await this.replayingEventV2Client.ReplayEventArchiveV2sAsync(
                randomEventAddressId,
                randomEventListenerIds,
                randomStartDate,
                randomEndDate,
                randomCancellationToken);

            // then
            this.replayingEventV2CoordinationServiceMock.Verify(service =>
                service.ReplayEventArchiveV2sAsync(
                    randomEventAddressId,
                    randomEventListenerIds,
                    randomStartDate,
                    randomEndDate,
                    randomCancellationToken),
                Times.Once);

            this.replayingEventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
