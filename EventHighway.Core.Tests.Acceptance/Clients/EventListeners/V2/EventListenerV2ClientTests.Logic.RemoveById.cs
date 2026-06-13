// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using FluentAssertions;
using Force.DeepCloner;

namespace EventHighway.Core.Tests.Acceptance.Clients.EventListeners.V2
{
    public partial class EventListenerV2ClientTests
    {
        [Fact]
        public async Task ShouldRemoveEventListenerV2ByIdAsync()
        {
            // given
            EventListenerV2 randomEventListenerV2 =
                await CreateRandomEventListenerV2Async();

            EventListenerV2 inputEventListenerV2 =
                randomEventListenerV2;

            EventListenerV2 expectedEventListenerV2 =
                inputEventListenerV2.DeepClone();

            Guid inputEventListenerV2Id =
                inputEventListenerV2.Id;

            // when
            EventListenerV2 actualEventListenerV2 =
                await this.clientBroker
                    .RemoveEventListenerV2ByIdAsync(
                        inputEventListenerV2Id);

            // then
            actualEventListenerV2.Should()
                .BeEquivalentTo(expectedEventListenerV2);

            await this.clientBroker
                .RemoveEventAddressV2ByIdAsync(
                    inputEventListenerV2.EventAddressId);
        }
    }
}
