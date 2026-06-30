// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using FluentAssertions;

namespace EventHighway.Core.Tests.Acceptance.Clients.EventArchives.V2
{
    public partial class EventArchiveV2ClientTests
    {
        [Fact]
        public async Task ShouldRetrieveEventArchiveV2ByIdAsync()
        {
            // given
            EventAddressV2 randomEventAddressV2 =
                await CreateRandomEventAddressV2Async();

            Guid inputEventAddressV2Id =
                randomEventAddressV2.Id;

            EventV2 randomDeadEventV2 =
                await SubmitDeadEventV2Async(inputEventAddressV2Id);

            Guid inputEventArchiveV2Id = randomDeadEventV2.Id;

            await this.clientBroker.ArchiveDeadEventV2sAsync();

            // when
            EventArchiveV2 actualEventArchiveV2 =
                await this.clientBroker
                    .RetrieveEventArchiveV2ByIdAsync(
                        inputEventArchiveV2Id);

            // then
            actualEventArchiveV2.Should().NotBeNull();
            actualEventArchiveV2.Id.Should().Be(inputEventArchiveV2Id);
            actualEventArchiveV2.EventName.Should().Be(randomDeadEventV2.EventName);
            actualEventArchiveV2.EventAddressId.Should().Be(inputEventAddressV2Id);

            await this.clientBroker
                .PurgeEventArchiveV2sAsync(DateTimeOffset.UtcNow.AddSeconds(1));

            await this.clientBroker
                .RemoveEventAddressV2ByIdAsync(inputEventAddressV2Id);
        }
    }
}
