// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.EventHandlers.Models.Exposers.DelegateEventHandlers.Exceptions;
using EventHighway.EventHandlers.Models.Foundations.Delegates.Exceptions;
using EventHighway.EventHandlers.Services.Delegates;
using Xeptions;

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
            try
            {
                return await this.delegateService.InvokeAsync(content, null, cancellationToken);
            }
            catch (DelegateServiceValidationException delegateServiceValidationException)
            {
                throw CreateValidationException(
                    delegateServiceValidationException.InnerException as Xeption);
            }
            catch (DelegateServiceException delegateServiceException)
            {
                throw CreateDependencyException(
                    delegateServiceException.InnerException as Xeption);
            }
            catch (Exception exception)
            {
                throw CreateServiceException(
                    exception.InnerException as Xeption);
            }
        }

        private static DelegateEventHandlerValidationException CreateValidationException(
            Xeption innerException) =>
            new DelegateEventHandlerValidationException(
                message: "Delegate event handler validation error occurred, fix errors and try again.",
                innerException,
                data: innerException.Data);

        private static DelegateEventHandlerDependencyException CreateDependencyException(
            Xeption innerException) =>
            new DelegateEventHandlerDependencyException(
                message: "Delegate event handler dependency error occurred, contact support.",
                innerException,
                data: innerException.Data);

        private static DelegateEventHandlerServiceException CreateServiceException(
            Xeption innerException) =>
            new DelegateEventHandlerServiceException(
                message: "Delegate event handler service error occurred, contact support.",
                innerException,
                data: innerException.Data);
    }
}
