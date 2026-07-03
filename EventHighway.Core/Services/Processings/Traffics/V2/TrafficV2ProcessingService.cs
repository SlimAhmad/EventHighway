// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Services.Foundations.Events.V2;

namespace EventHighway.Core.Services.Processings.Traffics.V2
{
    internal partial class TrafficV2ProcessingService : ITrafficV2ProcessingService
    {
        private readonly IEventV2Service eventV2Service;
        private readonly ILoggingBroker loggingBroker;

        public TrafficV2ProcessingService(
            IEventV2Service eventV2Service,
            ILoggingBroker loggingBroker)
        {
            this.eventV2Service = eventV2Service;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<TrafficSnapshotV2> RetrieveTrafficSnapshotV2Async(
            TrafficPeriodV2 period,
            DateTimeOffset windowStart,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
