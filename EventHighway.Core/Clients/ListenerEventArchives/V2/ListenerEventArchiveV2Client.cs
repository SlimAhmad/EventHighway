// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Services.Foundations.ListenerEventArchives.V2;

namespace EventHighway.Core.Clients.ListenerEventArchives.V2
{
    /// <summary>
    /// Represents the V2 listener event archive client implementation, exposing read operations
    /// over archived listener events while managing foundation service exceptions.
    /// </summary>
    internal class ListenerEventArchiveV2Client : IListenerEventArchiveV2Client
    {
        private readonly IListenerEventArchiveV2Service listenerEventArchiveV2Service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListenerEventArchiveV2Client"/> class with
        /// the specified listener event archive service.
        /// </summary>
        /// <param name="listenerEventArchiveV2Service">The foundation service for archived listener
        /// events.</param>
        public ListenerEventArchiveV2Client(IListenerEventArchiveV2Service listenerEventArchiveV2Service) =>
            this.listenerEventArchiveV2Service = listenerEventArchiveV2Service;

        public async ValueTask<IQueryable<ListenerEventArchiveV2>> RetrieveAllListenerEventArchiveV2sAsync(
            CancellationToken cancellationToken = default)
        {
            return await this.listenerEventArchiveV2Service
                .RetrieveAllListenerEventArchiveV2sAsync(cancellationToken);
        }
    }
}
