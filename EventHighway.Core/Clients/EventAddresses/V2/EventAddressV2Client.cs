// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventAddresses.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2.Exceptions;
using EventHighway.Core.Services.Foundations.EventAddresses.V2;
using Xeptions;

namespace EventHighway.Core.Clients.EventAddresses.V2
{
    /// <summary>
    /// Represents the V2 event address client implementation, handling event address
    /// registration, retrieval, and removal operations while managing service exceptions.
    /// </summary>
    internal class EventAddressV2Client : IEventAddressV2Client
    {
        private readonly IEventAddressV2Service eventAddressV2Service;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventAddressV2Client"/> class with
        /// the specified event address service.
        /// </summary>
        /// <param name="eventAddressV2Service">The service for managing event addresses.</param>
        /// <exception cref="ArgumentNullException">Thrown when eventAddressV2Service is
        /// null.</exception>
        public EventAddressV2Client(IEventAddressV2Service eventAddressV2Service) =>
            this.eventAddressV2Service = eventAddressV2Service;

        /// <summary>
        /// Registers a new event address asynchronously by delegating to the service and
        /// handling any exceptions that occur.
        /// </summary>
        /// <param name="eventAddressV2">The event address to register.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{EventAddressV2}"/> representing the asynchronous
        /// operation that returns the registered event address.</returns>
        /// <exception cref="EventAddressV2ClientValidationException">Thrown when validation
        /// errors occur in the service.</exception>
        /// <exception cref="EventAddressV2ClientDependencyException">Thrown when dependency
        /// or service errors occur.</exception>
        /// <exception cref="EventAddressV2ClientServiceException">Thrown when an unexpected
        /// error occurs during registration.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        public async ValueTask<EventAddressV2> RegisterEventAddressV2Async(
            EventAddressV2 eventAddressV2,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.eventAddressV2Service
                    .AddEventAddressV2Async(eventAddressV2, cancellationToken);
            }
            catch (EventAddressV2ValidationException eventAddressV2ValidationException)
            {
                throw CreateEventAddressV2ClientValidationException(
                    eventAddressV2ValidationException.InnerException as Xeption);
            }
            catch (EventAddressV2DependencyValidationException eventAddressV2DependencyValidationException)
            {
                throw CreateEventAddressV2ClientValidationException(
                    eventAddressV2DependencyValidationException.InnerException as Xeption);
            }
            catch (EventAddressV2DependencyException eventAddressV2DependencyException)
            {
                throw CreateEventAddressV2ClientDependencyException(
                    eventAddressV2DependencyException.InnerException as Xeption);
            }
            catch (EventAddressV2ServiceException eventAddressV2ServiceException)
            {
                throw CreateEventAddressV2ClientDependencyException(
                    eventAddressV2ServiceException.InnerException as Xeption);
            }
            catch (Exception exception)
            {
                throw CreateEventAddressV2ClientServiceException(exception as Xeption);
            }
        }

        /// <summary>
        /// Retrieves all event addresses asynchronously by delegating to the service and
        /// handling any exceptions that occur.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{IQueryable}"/> representing the asynchronous
        /// operation that returns a queryable collection of all event addresses.</returns>
        /// <exception cref="EventAddressV2ClientDependencyException">Thrown when dependency
        /// or service errors occur.</exception>
        /// <exception cref="EventAddressV2ClientServiceException">Thrown when an unexpected
        /// error occurs during retrieval.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        public async ValueTask<IQueryable<EventAddressV2>> RetrieveAllEventAddressV2sAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.eventAddressV2Service.RetrieveAllEventAddressV2sAsync();
            }
            catch (EventAddressV2DependencyException eventAddressV2DependencyException)
            {
                throw CreateEventAddressV2ClientDependencyException(
                    eventAddressV2DependencyException.InnerException as Xeption);
            }
            catch (EventAddressV2ServiceException eventAddressV2ServiceException)
            {
                throw CreateEventAddressV2ClientDependencyException(
                    eventAddressV2ServiceException.InnerException as Xeption);
            }
            catch (Exception exception)
            {
                throw CreateEventAddressV2ClientServiceException(exception as Xeption);
            }
        }

        /// <summary>
        /// Removes an event address by its identifier asynchronously by delegating to the
        /// service and handling any exceptions that occur.
        /// </summary>
        /// <param name="eventAddressV2Id">The identifier of the event address to remove.</param>
        /// <param name="cancellationToken">A cancellation token to allow cancellation of the
        /// asynchronous operation. The default value is
        /// <see cref="CancellationToken.None"/>.</param>
        /// <returns>A <see cref="ValueTask{EventAddressV2}"/> representing the asynchronous
        /// operation that returns the removed event address.</returns>
        /// <exception cref="EventAddressV2ClientValidationException">Thrown when validation
        /// errors occur in the service.</exception>
        /// <exception cref="EventAddressV2ClientDependencyException">Thrown when dependency
        /// or service errors occur.</exception>
        /// <exception cref="EventAddressV2ClientServiceException">Thrown when an unexpected
        /// error occurs during removal.</exception>
        /// <exception cref="OperationCanceledException">Thrown when the cancellation token is
        /// signaled.</exception>
        public async ValueTask<EventAddressV2> RemoveEventAddressV2ByIdAsync(
            Guid eventAddressV2Id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.eventAddressV2Service
                    .RemoveEventAddressV2ByIdAsync(eventAddressV2Id, cancellationToken);
            }
            catch (EventAddressV2ValidationException eventAddressV2ValidationException)
            {
                throw CreateEventAddressV2ClientValidationException(
                    eventAddressV2ValidationException.InnerException as Xeption);
            }
            catch (EventAddressV2DependencyValidationException eventAddressV2DependencyValidationException)
            {
                throw CreateEventAddressV2ClientValidationException(
                    eventAddressV2DependencyValidationException.InnerException as Xeption);
            }
            catch (EventAddressV2DependencyException eventAddressV2DependencyException)
            {
                throw CreateEventAddressV2ClientDependencyException(
                    eventAddressV2DependencyException.InnerException as Xeption);
            }
            catch (EventAddressV2ServiceException eventAddressV2ServiceException)
            {
                throw CreateEventAddressV2ClientDependencyException(
                    eventAddressV2ServiceException.InnerException as Xeption);
            }
            catch (Exception exception)
            {
                throw CreateEventAddressV2ClientServiceException(exception as Xeption);
            }
        }

        private static EventAddressV2ClientValidationException
            CreateEventAddressV2ClientValidationException(Xeption innerException)
        {
            return new EventAddressV2ClientValidationException(
                message: "Event address client validation error occurred, fix the errors and try again.",
                innerException: innerException,
                data: innerException.Data);
        }

        private static EventAddressV2ClientDependencyException
            CreateEventAddressV2ClientDependencyException(Xeption innerException)
        {
            return new EventAddressV2ClientDependencyException(
                message: "Event address client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException.Data);
        }

        private static EventAddressV2ClientServiceException
            CreateEventAddressV2ClientServiceException(Xeption innerException)
        {
            return new EventAddressV2ClientServiceException(
                message: "Event address client service error occurred, contact support.",
                innerException: innerException,
                data: innerException.Data);
        }
    }
}
