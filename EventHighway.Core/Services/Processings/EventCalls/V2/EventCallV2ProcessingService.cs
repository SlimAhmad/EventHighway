// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;
using EventHighway.Core.Services.Foundations.EventCalls.V2;

namespace EventHighway.Core.Services.Processings.EventCalls.V2
{
    internal partial class EventCallV2ProcessingService : IEventCallV2ProcessingService
    {
        private readonly IEventCallV2Service eventCallV2Service;
        private readonly ILoggingBroker loggingBroker;

        public EventCallV2ProcessingService(
            IEventCallV2Service eventCallV2Service,
            ILoggingBroker loggingBroker)
        {
            this.eventCallV2Service = eventCallV2Service;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<EventCallV2> RunEventCallV2Async(
            EventCallV2 eventCallV2,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidateEventCallV2IsNotNull(eventCallV2);

            return await this.eventCallV2Service.RunEventCallV2Async(eventCallV2, cancellationToken);
        });

        public ValueTask<IEnumerable<string>> SplitPromotedPropertyKeysAsync(
            string promotedProperties,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            IEnumerable<string> keys =
                string.IsNullOrWhiteSpace(promotedProperties)
                    ? Array.Empty<string>()
                    : promotedProperties.Split(
                        ',',
                        StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            return new ValueTask<IEnumerable<string>>(keys);
        }
    }
}
