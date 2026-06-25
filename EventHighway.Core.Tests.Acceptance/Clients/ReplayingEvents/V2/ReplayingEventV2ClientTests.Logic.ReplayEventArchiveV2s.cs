// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using FluentAssertions;

namespace EventHighway.Core.Tests.Acceptance.Clients.ReplayingEvents.V2
{
    public partial class ReplayingEventV2ClientTests
    {
        [Fact]
        public async Task ShouldReplayEventArchiveV2sAsync()
        {
            // given . when
            Func<Task> replayEventArchiveV2sTask = async () =>
                await this.clientBroker.ReplayEventArchiveV2sAsync(
                    eventAddressId: null,
                    eventListenerIds: null,
                    startDate: null,
                    endDate: null);

            // then
            await replayEventArchiveV2sTask.Should()
                .NotThrowAsync();
        }
    }
}
