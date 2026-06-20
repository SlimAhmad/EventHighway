// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;

namespace EventHighway.Core.Tests.Acceptance.Clients.Events.V2
{
    public partial class EventV2ClientTests
    {
        [Fact]
        public async Task ShouldFireScheduledPendingEventV2WithDelegateHandlerAsync()
        {
            // given
            int randomNumberA = GetRandomPositiveInt();
            int randomNumberB = GetRandomPositiveInt();
            string inputContent = $"{randomNumberA},{randomNumberB}";
            string expectedResponse = $"{randomNumberA + randomNumberB}";

            EventAddressV2 eventAddressV2 =
                await CreateRandomEventAddressV2Async();

            EventListenerV2 listenerV2 =
                CreateDelegateHandlerListenerV2(eventAddressV2.Id);

            await this.clientBroker.RegisterEventListenerV2Async(listenerV2);

            EventV2 eventV2 =
                await SubmitScheduledEventV2Async(
                    eventAddressV2.Id,
                    content: inputContent);

            // when
            await this.clientBroker.FireScheduledPendingEventV2sAsync();

            IQueryable<ListenerEventV2> allListenerEventV2s =
                await this.clientBroker.RetrieveAllListenerEventV2sAsync();

            // then
            allListenerEventV2s
                .Where(le => le.EventId == eventV2.Id)
                .Should().ContainSingle(le =>
                    le.Status == ListenerEventStatusV2.Success &&
                    le.Response == expectedResponse);

            // cleanup
            foreach (ListenerEventV2 le in allListenerEventV2s.Where(le => le.EventId == eventV2.Id))
                await this.clientBroker.RemoveListenerEventV2ByIdAsync(le.Id);

            await this.clientBroker.RemoveEventV2ByIdAsync(eventV2.Id);
            await this.clientBroker.RemoveEventListenerV2ByIdAsync(listenerV2.Id);
            await this.clientBroker.RemoveEventAddressV2ByIdAsync(eventAddressV2.Id);
        }
    }
}
