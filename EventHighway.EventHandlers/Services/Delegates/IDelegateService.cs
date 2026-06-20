// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;

namespace EventHighway.EventHandlers.Services.Delegates
{
    internal interface IDelegateService
    {
        ValueTask<EventHandlerResult> InvokeAsync(
            string content,
            CancellationToken cancellationToken = default);
    }
}
