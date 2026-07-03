// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Services.Foundations.Events.V2;

namespace EventHighway.Core.Services.Orchestrations.RetrySummaries.V2
{
    internal partial class RetrySummaryV2OrchestrationService : IRetrySummaryV2OrchestrationService
    {
        private readonly IEventAddressV2Service eventAddressV2Service;
        private readonly IEventV2Service eventV2Service;
        private readonly ILoggingBroker loggingBroker;

        public RetrySummaryV2OrchestrationService(
            IEventAddressV2Service eventAddressV2Service,
            IEventV2Service eventV2Service,
            ILoggingBroker loggingBroker)
        {
            this.eventAddressV2Service = eventAddressV2Service;
            this.eventV2Service = eventV2Service;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<RetryHealthSummaryV2> RetrieveRetryHealthV2Async(
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
