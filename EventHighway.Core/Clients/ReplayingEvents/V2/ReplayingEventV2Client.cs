// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Services.Coordinations.ReplayingEvents.V2;

namespace EventHighway.Core.Clients.ReplayingEvents.V2
{
    internal class ReplayingEventV2Client : IReplayingEventV2Client
    {
        private readonly IReplayingEventV2CoordinationService replayingEventV2CoordinationService;

        public ReplayingEventV2Client(IReplayingEventV2CoordinationService replayingEventV2CoordinationService) =>
            this.replayingEventV2CoordinationService = replayingEventV2CoordinationService;

        public ValueTask ReplayEventArchiveV2sAsync(
            Guid? eventAddressId,
            IEnumerable<Guid> eventListenerIds,
            DateTimeOffset? startDate,
            DateTimeOffset? endDate,
            CancellationToken cancellationToken = default) =>
                throw new NotImplementedException();

        public ValueTask ProcessReplayedListenerEventV2sAsync(
            CancellationToken cancellationToken = default) =>
                throw new NotImplementedException();
    }
}
