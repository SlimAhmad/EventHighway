// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations;
using EventHighway.Core.Tests.Acceptance.Brokers;
using EventHighway.EventHandlers;
using Tynamix.ObjectFiller;
using WireMock.Server;

namespace EventHighway.Core.Tests.Acceptance.Clients.Events.V2
{
    [Collection(nameof(ClientTestCollection))]
    public partial class EventV2ClientTests
    {
        private readonly WireMockServer wireMockServer;
        private readonly ClientBroker clientBroker;
        private readonly DelegateEventHandler delegateEventHandler;
        private readonly RestBearerEventHandler restBearerEventHandler;

        public EventV2ClientTests()
        {
            this.wireMockServer = WireMockServer.Start();

            this.delegateEventHandler = new DelegateEventHandler(
                Guid.NewGuid(),
                (content, _, cancellationToken) =>
                {
                    string[] parts = content.Split(',');
                    int sum = int.Parse(parts[0].Trim()) + int.Parse(parts[1].Trim());

                    return ValueTask.FromResult(new EventHandlerResult
                    {
                        IsSuccess = true,
                        Response = sum.ToString(),
                        ResponseCode = "200",
                        ResponseMessage = "OK"
                    });
                });

            this.restBearerEventHandler = new RestBearerEventHandler(Guid.NewGuid());

            this.clientBroker = new ClientBroker();

            this.clientBroker
                .RegisterEventHandler(this.delegateEventHandler)
                .RegisterEventHandler(this.restBearerEventHandler);
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static int GetRandomPositiveInt() =>
            new IntRange(min: 1, max: 100).GetValue();

        private static string GetRandomString() =>
            new MnemonicString(wordCount: 1).GetValue();

        private async ValueTask<EventAddressV2> CreateRandomEventAddressV2Async()
        {
            EventAddressV2 randomEventAddressV2 =
                CreateEventAddressV2Filler().Create();

            await this.clientBroker.RegisterEventAddressV2Async(
                randomEventAddressV2);

            return randomEventAddressV2;
        }

        private async ValueTask<EventV2> SubmitScheduledEventV2Async(
            Guid eventAddressV2Id,
            string content = null)
        {
            EventV2 eventV2 = CreateEventV2Filler(
                eventAddressV2Id,
                scheduledDate: DateTimeOffset.Now.AddSeconds(1),
                content: content)
                    .Create();

            await this.clientBroker.SubmitEventV2Async(eventV2);
            await Task.Delay(TimeSpan.FromSeconds(2));

            return eventV2;
        }

        private async ValueTask<EventV2> SubmitEventV2Async(
            Guid eventAddressV2Id,
            DateTimeOffset scheduledDate)
        {
            EventV2 eventV2 = CreateEventV2Filler(
                eventAddressV2Id,
                scheduledDate)
                    .Create();

            await this.clientBroker.SubmitEventV2Async(eventV2);

            return eventV2;
        }

        private static EventV2 CreateRandomEventV2(
            Guid eventAddressV2Id,
            DateTimeOffset scheduledDate)
        {
            return CreateEventV2Filler(
                eventAddressV2Id,
                scheduledDate)
                    .Create();
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
                HandlerConfigurations = new List<HandlerConfiguration>(),
                EventAddressId = eventAddressId,
                CreatedDate = now,
                UpdatedDate = now,
            };
        }

        private EventListenerV2 CreateRestBearerHandlerListenerV2(Guid eventAddressId)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            string wireMockUrl = this.wireMockServer.Url;

            return new EventListenerV2
            {
                Id = Guid.NewGuid(),
                Name = GetRandomString(),
                Description = GetRandomString(),
                HandlerId = this.restBearerEventHandler.Id,
                HandlerName = this.restBearerEventHandler.Name,
                HandlerConfigurations = new List<HandlerConfiguration>
                {
                    new() { Id = Guid.NewGuid(), Name = "Url", Value = $"{wireMockUrl}/events", CreatedDate = now, UpdatedDate = now },
                    new() { Id = Guid.NewGuid(), Name = "TokenUrl", Value = $"{wireMockUrl}/token", CreatedDate = now, UpdatedDate = now },
                    new() { Id = Guid.NewGuid(), Name = "ClientId", Value = "test-client", CreatedDate = now, UpdatedDate = now },
                    new() { Id = Guid.NewGuid(), Name = "ClientSecret", Value = "test-secret", CreatedDate = now, UpdatedDate = now },
                    new() { Id = Guid.NewGuid(), Name = "Scope", Value = "test-scope", CreatedDate = now, UpdatedDate = now },
                    new() { Id = Guid.NewGuid(), Name = "GrantType", Value = "client_credentials", CreatedDate = now, UpdatedDate = now },
                },
                EventAddressId = eventAddressId,
                CreatedDate = now,
                UpdatedDate = now,
            };
        }

        private static Filler<EventV2> CreateEventV2Filler(
            Guid eventAddressV2Id,
            DateTimeOffset scheduledDate,
            string content = null)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            var filler = new Filler<EventV2>();

            filler.Setup()
                .OnProperty(eventV2 => eventV2.EventAddressV2).IgnoreIt()
                .OnProperty(eventV2 => eventV2.ListenerEventV2s).IgnoreIt()
                .OnProperty(eventV2 => eventV2.EventAddressId).Use(eventAddressV2Id)
                .OnProperty(eventV2 => eventV2.ScheduledDate).Use(scheduledDate)
                .OnType<DateTimeOffset>().Use(now);

            if (content is not null)
                filler.Setup().OnProperty(eventV2 => eventV2.Content).Use(content);

            return filler;
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
    }
}
