// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;

namespace EventHighway.Core.Services.Processings.EventCalls.V2
{
    internal interface IEventCallV2ProcessingService
    {
        ValueTask<EventCallV2> RunEventCallV2Async(
            EventCallV2 eventCallV2,
            CancellationToken cancellationToken = default);

        ValueTask<IEnumerable<string>> SplitPromotedPropertyKeysAsync(
            string promotedProperties,
            CancellationToken cancellationToken = default);
    }
}
