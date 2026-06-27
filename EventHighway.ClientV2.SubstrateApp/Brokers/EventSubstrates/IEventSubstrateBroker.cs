// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace EventHighway.ClientV2.BasicApp.Brokers.EventSubstrates
{
    public partial interface IEventSubstrateBroker
    {
        ValueTask FirePendingEventsAsync(CancellationToken cancellationToken = default);
    }
}
