// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.EventHandlers.Services.Delegates;

namespace EventHighway.EventHandlers
{
    public class DelegateEventHandler : IEventHandler
    {
        private readonly IDelegateService delegateService;

        public DelegateEventHandler(
            Func<string, IReadOnlyDictionary<string, string>, CancellationToken, ValueTask<EventHandlerResult>> handler)
            => this.delegateService = new DelegateService(handler);

        internal DelegateEventHandler(IDelegateService delegateService)
            => this.delegateService = delegateService;

        public string Name => nameof(DelegateEventHandler);
        public IEnumerable<string> RequiredParams => Array.Empty<string>();

        public async ValueTask<EventHandlerResult> HandleAsync(
            string content,
            IReadOnlyDictionary<string, string> handlerParams,
            CancellationToken cancellationToken = default)
        {
            return await this.delegateService.InvokeAsync(content, handlerParams, cancellationToken);
        }
    }
}
