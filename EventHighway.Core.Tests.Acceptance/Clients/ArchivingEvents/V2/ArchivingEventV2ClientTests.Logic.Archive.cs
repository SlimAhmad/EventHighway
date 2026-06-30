// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;

namespace EventHighway.Core.Tests.Acceptance.Clients.ArchivingEvents.V2
{
    public partial class ArchivingEventV2ClientTests
    {
        [Fact]
        public async Task ShouldArchiveDeadEventV2sAsync()
        {
            // given
            EventAddressV2 randomEventAddressV2 =
                await CreateRandomEventAddressV2Async();

            Guid inputEventAddressV2Id =
                randomEventAddressV2.Id;

            EventV2 randomDeadEventV2 =
                await SubmitDeadEventV2Async(inputEventAddressV2Id);

            // when
            Func<Task> archiveDeadEventV2sTask = async () =>
                await this.clientBroker.ArchiveDeadEventV2sAsync();

            // then
            await archiveDeadEventV2sTask.Should()
                .NotThrowAsync();

            await this.clientBroker
                .PurgeEventArchiveV2sAsync(DateTimeOffset.UtcNow.AddSeconds(1));

            await this.clientBroker
                .RemoveEventAddressV2ByIdAsync(inputEventAddressV2Id);
        }
    }
}
