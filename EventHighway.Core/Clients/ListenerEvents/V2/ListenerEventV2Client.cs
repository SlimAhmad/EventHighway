// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.ListenerEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V2.Exceptions;
using EventHighway.Core.Services.Orchestrations.EventListeners.V2;
using Xeptions;

namespace EventHighway.Core.Clients.ListenerEvents.V2
{
    /// <summary>
    /// Represents the V2 listener event client implementation, handling listener event
    /// retrieval and removal operations while managing orchestration service exceptions.
    /// </summary>
    internal class ListenerEventV2Client : IListenerEventV2Client
    {
        private readonly IEventListenerV2OrchestrationService eventListenerV2OrchestrationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListenerEventV2Client"/> class with
        /// the specified orchestration service.
        /// </summary>
        /// <param name="eventListenerV2OrchestrationService">The orchestration service for
        /// managing listener events.</param>
        /// <exception cref="ArgumentNullException">Thrown when
        /// eventListenerV2OrchestrationService is null.</exception>
        public ListenerEventV2Client(IEventListenerV2OrchestrationService eventListenerV2OrchestrationService) =>
            this.eventListenerV2OrchestrationService = eventListenerV2OrchestrationService;

        /// <summary>
        /// Retrieves all listener events asynchronously by delegating to the orchestration
        /// service and handling any exceptions that occur.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{IQueryable}"/> representing the asynchronous
        /// operation that returns a queryable collection of all listener events.</returns>
        /// <exception cref="ListenerEventV2ClientValidationException">Thrown when validation
        /// errors occur in the orchestration service.</exception>
        /// <exception cref="ListenerEventV2ClientDependencyException">Thrown when dependency
        /// or service errors occur.</exception>
        /// <exception cref="ListenerEventV2ClientServiceException">Thrown when an unexpected
        /// error occurs during retrieval.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        public async ValueTask<IQueryable<ListenerEventV2>> RetrieveAllListenerEventV2sAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.eventListenerV2OrchestrationService
                    .RetrieveAllListenerEventV2sAsync(cancellationToken);
            }
            catch (EventListenerV2OrchestrationValidationException
                eventListenerV2OrchestrationValidationException)
            {
                throw CreateListenerEventV2ClientValidationException(
                    eventListenerV2OrchestrationValidationException.InnerException as Xeption);
            }
            catch (EventListenerV2OrchestrationDependencyValidationException
                eventListenerV2OrchestrationDependencyValidationException)
            {
                throw CreateListenerEventV2ClientValidationException(
                    eventListenerV2OrchestrationDependencyValidationException.InnerException as Xeption);
            }
            catch (EventListenerV2OrchestrationDependencyException
                eventListenerV2OrchestrationDependencyException)
            {
                throw CreateListenerEventV2ClientDependencyException(
                    eventListenerV2OrchestrationDependencyException.InnerException as Xeption);
            }
            catch (EventListenerV2OrchestrationServiceException
                eventListenerV2OrchestrationServiceException)
            {
                throw CreateListenerEventV2ClientDependencyException(
                    eventListenerV2OrchestrationServiceException.InnerException as Xeption);
            }
            catch (Exception exception)
            {
                throw CreateListenerEventV2ClientServiceException(exception as Xeption);
            }
        }

        /// <summary>
        /// Removes a listener event by its identifier asynchronously by delegating to the
        /// orchestration service and handling any exceptions that occur.
        /// </summary>
        /// <param name="listenerEventV2Id">The identifier of the listener event to
        /// remove.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{ListenerEventV2}"/> representing the asynchronous
        /// operation that returns the removed listener event.</returns>
        /// <exception cref="ListenerEventV2ClientValidationException">Thrown when validation
        /// errors occur in the orchestration service.</exception>
        /// <exception cref="ListenerEventV2ClientDependencyException">Thrown when dependency
        /// or service errors occur.</exception>
        /// <exception cref="ListenerEventV2ClientServiceException">Thrown when an unexpected
        /// error occurs during removal.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        public async ValueTask<ListenerEventV2> RemoveListenerEventV2ByIdAsync(
            Guid listenerEventV2Id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.eventListenerV2OrchestrationService
                    .RemoveListenerEventV2ByIdAsync(listenerEventV2Id, cancellationToken);
            }
            catch (EventListenerV2OrchestrationValidationException
                eventListenerV2OrchestrationValidationException)
            {
                throw CreateListenerEventV2ClientValidationException(
                    eventListenerV2OrchestrationValidationException.InnerException as Xeption);
            }
            catch (EventListenerV2OrchestrationDependencyValidationException
                eventListenerV2OrchestrationDependencyValidationException)
            {
                throw CreateListenerEventV2ClientValidationException(
                    eventListenerV2OrchestrationDependencyValidationException.InnerException as Xeption);
            }
            catch (EventListenerV2OrchestrationDependencyException
                eventListenerV2OrchestrationDependencyException)
            {
                throw CreateListenerEventV2ClientDependencyException(
                    eventListenerV2OrchestrationDependencyException.InnerException as Xeption);
            }
            catch (EventListenerV2OrchestrationServiceException
                eventListenerV2OrchestrationServiceException)
            {
                throw CreateListenerEventV2ClientDependencyException(
                    eventListenerV2OrchestrationServiceException.InnerException as Xeption);
            }
            catch (Exception exception)
            {
                throw CreateListenerEventV2ClientServiceException(exception as Xeption);
            }
        }

        private static ListenerEventV2ClientValidationException
            CreateListenerEventV2ClientValidationException(Xeption innerException)
        {
            return new ListenerEventV2ClientValidationException(
                message: "Listener event client validation error occurred, fix the errors and try again.",
                innerException: innerException,
                data: innerException.Data);
        }

        private static ListenerEventV2ClientDependencyException
            CreateListenerEventV2ClientDependencyException(Xeption innerException)
        {
            return new ListenerEventV2ClientDependencyException(
                message: "Listener event client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException.Data);
        }

        private static ListenerEventV2ClientServiceException
            CreateListenerEventV2ClientServiceException(Xeption innerException)
        {
            return new ListenerEventV2ClientServiceException(
                message: "Listener event client service error occurred, contact support.",
                innerException: innerException,
                data: innerException.Data);
        }
    }
}
