// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

namespace EventHighway.ClientV2.SubstrateApp.Brokers.EventSubstrates
{
    public partial interface IEventSubstrateBroker
    {
        ValueTask ArchiveEventsAsync(CancellationToken cancellationToken = default);
    }
}
