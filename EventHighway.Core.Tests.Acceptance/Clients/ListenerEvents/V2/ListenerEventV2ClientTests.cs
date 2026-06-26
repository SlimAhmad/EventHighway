// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;

using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Tests.Acceptance.Brokers;
using EventHighway.EventHandlers;
using Tynamix.ObjectFiller;
using WireMock.Server;

namespace EventHighway.Core.Tests.Acceptance.Clients.ListenerEvents.V2
{
    [Collection(nameof(ClientTestCollection))]
    public partial class ListenerEventV2ClientTests
    {
        private readonly WireMockServer wireMockServer;
        private readonly ClientBroker clientBroker;
        private readonly DelegateEventHandler delegateEventHandler;

        public ListenerEventV2ClientTests()
        {
            this.wireMockServer = WireMockServer.Start();

            this.delegateEventHandler = new DelegateEventHandler(
                Guid.NewGuid(),
                (_, _) => ValueTask.FromResult(new EventHandlerResult
                {
                    IsSuccess = true,
                    Response = "OK",
                    ResponseCode = "200",
                    ResponseMessage = "OK"
                }));

            this.clientBroker = new ClientBroker();
            this.clientBroker.RegisterEventHandler(this.delegateEventHandler);
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static string GetRandomString() =>
            new MnemonicString(wordCount: 1).GetValue();

        private async ValueTask<IQueryable<ListenerEventV2>> CreateRandomListenerEventV2sAsync()
        {
            EventAddressV2 randomEventAddressV2 =
                await CreateRandomEventAddressV2Async();

            Guid inputEventAddressV2Id = randomEventAddressV2.Id;

            await CreateRandomEventV2sAsync(inputEventAddressV2Id);
            await CreateRandomEventListenerV2sAsync(inputEventAddressV2Id);
            await Task.Delay(TimeSpan.FromSeconds(2));
            await this.clientBroker.FireScheduledPendingEventV2sAsync();

            return await this.clientBroker.RetrieveAllListenerEventV2sAsync();
        }

        private async ValueTask CreateRandomEventListenerV2sAsync(Guid eventAddressV2Id)
        {
            int randomNumber = GetRandomNumber();

            for (int index = 0; index < randomNumber; index++)
            {
                EventListenerV2 listenerV2 =
                    CreateDelegateHandlerListenerV2(eventAddressV2Id);

                await this.clientBroker.RegisterEventListenerV2Async(listenerV2);
            }
        }

        private async ValueTask CreateRandomEventV2sAsync(Guid eventAddressV2Id)
        {
            int randomNumber = GetRandomNumber();

            for (int index = 0; index < randomNumber; index++)
            {
                EventV2 randomEventV2 = CreateEventV2Filler(
                    eventAddressV2Id,
                    scheduledDate: DateTimeOffset.Now.AddSeconds(1))
                        .Create();

                await this.clientBroker.SubmitEventV2Async(randomEventV2);
            }
        }

        private async ValueTask<EventAddressV2> CreateRandomEventAddressV2Async()
        {
            EventAddressV2 randomEventAddressV2 =
                CreateEventAddressV2Filler().Create();

            await this.clientBroker.RegisterEventAddressV2Async(
                randomEventAddressV2);

            return randomEventAddressV2;
        }

        private EventListenerV2 CreateDelegateHandlerListenerV2(Guid eventAddressId)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            return new EventListenerV2
            {
                Id = Guid.NewGuid(),
                Name = GetRandomString(),
                Description = GetRandomString(),
                HandlerId = this.delegateEventHandler.Id,
                HandlerName = this.delegateEventHandler.Name,
                EventAddressId = eventAddressId,
                CreatedDate = now,
                UpdatedDate = now,
            };
        }

        private static Filler<EventAddressV2> CreateEventAddressV2Filler()
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var filler = new Filler<EventAddressV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(now)
                .OnProperty(eventAddressV2 => eventAddressV2.Events).IgnoreIt()
                .OnProperty(eventAddressV2 => eventAddressV2.EventListenerV2s).IgnoreIt()
                .OnProperty(eventAddressV2 => eventAddressV2.ListenerEventV2s).IgnoreIt();

            return filler;
        }

        private static Filler<EventV2> CreateEventV2Filler(
            Guid eventAddressV2Id,
            DateTimeOffset scheduledDate)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var filler = new Filler<EventV2>();

            filler.Setup()
                .OnProperty(eventV2 => eventV2.EventAddressV2).IgnoreIt()
                .OnProperty(eventV2 => eventV2.ListenerEventV2s).IgnoreIt()
                .OnProperty(eventV2 => eventV2.EventAddressId).Use(eventAddressV2Id)
                .OnProperty(eventV2 => eventV2.ScheduledDate).Use(scheduledDate)
                .OnType<DateTimeOffset>().Use(now)
                .OnType<EventParticipantV2>().IgnoreIt();

            return filler;
        }
    }
}
