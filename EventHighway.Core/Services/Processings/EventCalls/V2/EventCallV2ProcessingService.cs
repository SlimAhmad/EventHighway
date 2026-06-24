// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Jsons;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;
using EventHighway.Core.Models.Services.Foundations.PromotedProperties;
using EventHighway.Core.Services.Foundations.EventCalls.V2;

namespace EventHighway.Core.Services.Processings.EventCalls.V2
{
    internal partial class EventCallV2ProcessingService : IEventCallV2ProcessingService
    {
        private readonly IEventCallV2Service eventCallV2Service;
        private readonly IJsonBroker jsonBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventCallV2ProcessingService(
            IEventCallV2Service eventCallV2Service,
            IJsonBroker jsonBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventCallV2Service = eventCallV2Service;
            this.jsonBroker = jsonBroker;
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
            CancellationToken cancellationToken = default) =>
        TryCatch(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            ValidatePromotedProperties(promotedProperties);

            IEnumerable<string> keys =
                string.IsNullOrWhiteSpace(promotedProperties)
                    ? Array.Empty<string>()
                    : promotedProperties.Split(
                        ',',
                        StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            return new ValueTask<IEnumerable<string>>(keys);
        });

        public ValueTask<List<PromotedProperty>> PromotePropertiesAsync(
            string content,
            string promotedProperties,
            CancellationToken cancellationToken = default)
        {
            var promotedPropertyList = new List<PromotedProperty>();

            if (string.IsNullOrWhiteSpace(content)
                || string.IsNullOrWhiteSpace(promotedProperties))
            {
                return new ValueTask<List<PromotedProperty>>(promotedPropertyList);
            }

            string[] keys = promotedProperties.Split(
                ',',
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            foreach (string key in keys)
            {
                if (this.jsonBroker.CheckIfPropertyExist(content, key))
                {
                    promotedPropertyList.Add(new PromotedProperty
                    {
                        Name = key,
                        Value = this.jsonBroker.GetJsonPropertyValue(content, key)
                    });
                }
            }

            return new ValueTask<List<PromotedProperty>>(promotedPropertyList);
        }
    }
}
