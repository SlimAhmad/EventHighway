// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Services.Foundations.EventArchives.V2;

namespace EventHighway.Core.Clients.EventArchives.V2
{
    /// <summary>
    /// Represents the V2 event archive client implementation, exposing read operations over
    /// archived events while managing foundation service exceptions.
    /// </summary>
    internal class EventArchiveV2Client : IEventArchiveV2Client
    {
        private readonly IEventArchiveV2Service eventArchiveV2Service;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventArchiveV2Client"/> class with the
        /// specified event archive service.
        /// </summary>
        /// <param name="eventArchiveV2Service">The foundation service for archived events.</param>
        public EventArchiveV2Client(IEventArchiveV2Service eventArchiveV2Service) =>
            this.eventArchiveV2Service = eventArchiveV2Service;

        public async ValueTask<IQueryable<EventArchiveV2>> RetrieveAllEventArchiveV2sAsync(
            CancellationToken cancellationToken = default)
        {
            return await this.eventArchiveV2Service
                .RetrieveAllEventArchiveV2sAsync(cancellationToken);
        }

        public ValueTask<EventArchiveV2> RetrieveEventArchiveV2ByIdAsync(
            Guid eventArchiveV2Id,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }
}
