// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace EventHighway.ClientV2.SubstrateApp.Brokers.DateTimes
{
    public sealed class DateTimeBroker : IDateTimeBroker
    {
        public async ValueTask<DateTimeOffset> GetCurrentDateTimeOffsetAsync() => DateTimeOffset.UtcNow;
    }
}
