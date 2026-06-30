// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;

namespace EventHighway.Portal.Web.Brokers.EventHighways
{
    public partial interface IEventHighwayBroker
    {
        ValueTask<IQueryable<ListenerEventArchiveV2>> RetrieveAllListenerEventArchiveV2sAsync(
            CancellationToken cancellationToken = default);
    }
}
