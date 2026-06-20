// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;

namespace EventHighway.EventHandlers.Services.Delegates
{
    internal partial class DelegateService : IDelegateService
    {
        private readonly Func<
            string,
            CancellationToken,
            ValueTask<EventHandlerResult>> handler;

        public DelegateService(
            Func<string, CancellationToken, ValueTask<EventHandlerResult>> handler)
        {
            this.handler = handler;
        }

        public virtual ValueTask<EventHandlerResult> InvokeAsync(
            string content,
            CancellationToken cancellationToken = default) =>
        TryCatch(async () =>
        {
            ValidateInvokeParams(content);

            return await this.handler(content, cancellationToken);
        });
    }
}
