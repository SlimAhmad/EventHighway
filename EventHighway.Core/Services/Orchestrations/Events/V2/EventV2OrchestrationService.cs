// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Services.Processings.EventAddresses.V2;
using EventHighway.Core.Services.Processings.EventCalls.V2;
using EventHighway.Core.Services.Processings.Events.V2;

namespace EventHighway.Core.Services.Orchestrations.Events.V2
{
    internal partial class EventV2OrchestrationService : IEventV2OrchestrationService
    {
        private readonly IEventV2ProcessingService eventV2ProcessingService;
        private readonly IEventAddressV2ProcessingService eventAddressV2ProcessingService;
        private readonly IEventCallV2ProcessingService eventCallV2ProcessingService;
        private readonly ILoggingBroker loggingBroker;

        public EventV2OrchestrationService(
            IEventV2ProcessingService eventV2ProcessingService,
            IEventAddressV2ProcessingService eventAddressV2ProcessingService,
            IEventCallV2ProcessingService eventCallV2ProcessingService,
            ILoggingBroker loggingBroker)
        {
            this.eventV2ProcessingService = eventV2ProcessingService;
            this.eventAddressV2ProcessingService = eventAddressV2ProcessingService;
            this.eventCallV2ProcessingService = eventCallV2ProcessingService;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<IQueryable<EventV2>> RetrieveAllEventV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
            await this.eventV2ProcessingService.RetrieveAllEventV2sAsync());

        public ValueTask<IQueryable<EventAddressV2>> RetrieveAllEventAddressV2sAsync(
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public ValueTask<EventV2> SubmitEventV2Async(
            EventV2 eventV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateEventV2IsNotNull(eventV2);

            EventAddressV2 maybeEventAddressV2 =
                await this.eventAddressV2ProcessingService
                    .RetrieveEventAddressV2ByIdAsync(
                        eventV2.EventAddressId,
                        cancellationToken);

            ValidateEventAddressV2Exists(
                maybeEventAddressV2,
                eventV2.EventAddressId);

            return await this.eventV2ProcessingService
                .AddEventV2Async(eventV2, cancellationToken);
        });

        public ValueTask<IQueryable<EventV2>> RetrieveScheduledPendingEventV2sAsync(
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            return await this.eventV2ProcessingService
                .RetrieveScheduledPendingEventV2sAsync();
        });

        public ValueTask<EventV2> MarkEventV2AsImmediateAsync(
            EventV2 eventV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateEventV2IsNotNull(eventV2);

            return await this.eventV2ProcessingService
                .MarkEventV2AsImmediateAsync(eventV2, cancellationToken);
        });

        public ValueTask<EventV2> RemoveEventV2ByIdAsync(
            Guid eventV2Id,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateEventV2Id(eventV2Id);

            return await this.eventV2ProcessingService
                .RemoveEventV2ByIdAsync(eventV2Id, cancellationToken);
        });

        public ValueTask<EventCallV2> RunEventCallV2Async(
            EventCallV2 eventCallV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateEventCallV2IsNotNull(eventCallV2);

            return await this.eventCallV2ProcessingService.RunEventCallV2Async(
                eventCallV2,
                cancellationToken);
        });
    }
}
