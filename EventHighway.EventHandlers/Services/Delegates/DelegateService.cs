// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;

namespace EventHighway.EventHandlers.Services.Delegates
{
    internal partial class DelegateService : IDelegateService
    {
        private readonly Func<
            string,
            IReadOnlyDictionary<string, string>,
            CancellationToken,
            ValueTask<EventHandlerResult>> handler;

        public DelegateService(
            Func<string, IReadOnlyDictionary<string, string>, CancellationToken, ValueTask<EventHandlerResult>> handler)
        {
            this.handler = handler;
        }

        public virtual async ValueTask<EventHandlerResult> InvokeAsync(
            string content,
            IReadOnlyDictionary<string, string> handlerParams,
            CancellationToken cancellationToken = default)
        {
            return await this.handler(content, handlerParams, cancellationToken);
        }
    }
}
