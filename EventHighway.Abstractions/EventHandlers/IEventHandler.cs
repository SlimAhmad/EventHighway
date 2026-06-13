// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EventHighway.Abstractions.EventHandlers
{
    /// <summary>
    /// Defines a contract for implementing a custom event handler within EventHighway.
    /// Implementations receive event content and optional parameters, then return a
    /// result indicating success or failure.
    /// </summary>
    /// <remarks>
    /// Exceptions thrown by implementations are expected to derive from one of the
    /// following marker interfaces:
    /// <list type="bullet">
    ///   <item><description>
    ///     <see cref="Exceptions.IEventHandlerValidationException"/> — invalid input or state.
    ///   </description></item>
    ///   <item><description>
    ///     <see cref="Exceptions.IEventHandlerDependencyException"/> — an external dependency failed.
    ///   </description></item>
    ///   <item><description>
    ///     <see cref="Exceptions.IEventHandlerServiceException"/> — an unexpected service-level fault.
    ///   </description></item>
    /// </list>
    /// </remarks>
    public interface IEventHandler
    {
        Guid Id { get; }

        /// <summary>
        /// Gets the unique name that identifies this handler within the EventHighway pipeline.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the set of parameter keys that must be present in <c>handlerParams</c>
        /// when <see cref="HandleAsync"/> is called.
        /// </summary>
        IEnumerable<string> RequiredParams { get; }

        /// <summary>
        /// Processes the given event content and returns a result.
        /// </summary>
        /// <param name="content">The raw event payload to process.</param>
        /// <param name="handlerParams">
        /// A read-only dictionary of named parameters supplied by the caller.
        /// All keys listed in <see cref="RequiredParams"/> must be present.
        /// </param>
        /// <param name="cancellationToken">
        /// A token that can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        /// A <see cref="ValueTask{EventHandlerResult}"/> that resolves to an
        /// <see cref="EventHandlerResult"/> describing the outcome of the event handling.
        /// </returns>
        /// <exception cref="Exceptions.IEventHandlerValidationException">
        /// Thrown when <paramref name="content"/> or <paramref name="handlerParams"/> are
        /// invalid or missing required values.
        /// </exception>
        /// <exception cref="Exceptions.IEventHandlerDependencyException">
        /// Thrown when an external dependency (e.g. a downstream service or database) fails.
        /// </exception>
        /// <exception cref="Exceptions.IEventHandlerServiceException">
        /// Thrown when an unexpected fault occurs within the handler itself.
        /// </exception>
        ValueTask<EventHandlerResult> HandleAsync(
            string content,
            IReadOnlyDictionary<string, string> handlerParams,
            CancellationToken cancellationToken = default);
    }
}
