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

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateEventHandler"/> class with the specified
        /// identifier and event handler delegate.
        /// </summary>
        /// <param name="Id">The unique identifier for this event handler.</param>
        /// <param name="handler">An asynchronous delegate that processes event content and
        /// parameters, accepting content, handler parameters, and a cancellation token, and returning
        /// an <see cref="EventHandlerResult"/>.</param>
        public DelegateEventHandler(
            Guid Id,
            Func<string, IReadOnlyDictionary<string, string>, CancellationToken, ValueTask<EventHandlerResult>> handler)
        {
            this.Id = Id;
            this.delegateService = new DelegateService(handler);
        }

        internal DelegateEventHandler(Guid Id, IDelegateService delegateService)
        {
            this.Id = Id;
            this.delegateService = delegateService;
        }

        /// <summary>
        /// Gets the unique identifier for this event handler.
        /// </summary>
        public Guid Id { get; }

        /// <summary>
        /// Gets the name of this event handler.
        /// </summary>
        public string Name => nameof(DelegateEventHandler);

        /// <summary>
        /// Gets a collection of required parameters for this event handler.
        /// </summary>
        public IEnumerable<string> RequiredParams => Array.Empty<string>();


        /// <summary>
        /// Handles an event asynchronously by invoking the registered event handler delegate with the
        /// provided content and parameters.
        /// </summary>
        /// <param name="content">The event content to be processed by the handler.</param>
        /// <param name="handlerParams">A read-only dictionary of parameters to be passed to the
        /// handler.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{EventHandlerResult}"/> representing the asynchronous
        /// operation that returns the result of handling the event.</returns>
        /// <exception cref="DelegateEventHandlerValidationException">Thrown when the delegate
        /// service encounters validation errors during event handling.</exception>
        /// <exception cref="DelegateEventHandlerDependencyException">Thrown when the delegate
        /// service encounters dependency-related errors during event handling.</exception>
        /// <exception cref="DelegateEventHandlerServiceException">Thrown when an unexpected error
        /// occurs during event handling.</exception>
        public async ValueTask<EventHandlerResult> HandleAsync(
            string content,
            IReadOnlyDictionary<string, string> handlerParams,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.delegateService.InvokeAsync(content, handlerParams, cancellationToken);
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
