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
        public async Task ShouldProcessReplayedListenerEventV2sAsync()
        {
            // given . when
            Func<Task> processReplayedListenerEventV2sTask = async () =>
                await this.clientBroker.ProcessReplayedListenerEventV2sAsync();

            // then
            await processReplayedListenerEventV2sTask.Should()
                .NotThrowAsync();
        }
    }
}
