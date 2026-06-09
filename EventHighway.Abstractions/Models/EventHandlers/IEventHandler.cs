// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventHighway.Abstractions.Models.EventHandlers
{
    public interface IEventHandler
    {
        string Name { get; }
        IEnumerable<string> RequiredParams { get; }

        ValueTask<EventHandlerResult> HandleAsync(
            string content,
            IReadOnlyDictionary<string, string> handlerParams,
            CancellationToken cancellationToken = default);
    }
}
