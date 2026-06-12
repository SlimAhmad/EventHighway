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
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V2.Exceptions;
using EventHighway.Core.Services.Foundations.EventAddresses.V2;
using Xeptions;

namespace EventHighway.Core.Clients.EventAddresses.V2
{
    internal class EventAddressV2Client : IEventAddressV2Client
    {
        private readonly IEventAddressV2Service eventAddressV2Service;

        public EventAddressV2Client(IEventAddressV2Service eventAddressV2Service) =>
            this.eventAddressV2Service = eventAddressV2Service;

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

        public async ValueTask<IQueryable<EventAddressV2>> RetrieveAllEventAddressV2sAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.eventAddressV2Service.RetrieveAllEventAddressV2sAsync();
            }
            catch (EventListenerV2OrchestrationDependencyException
                eventListenerV2OrchestrationDependencyException)
            {
                throw CreateEventAddressV2ClientDependencyException(
                    eventListenerV2OrchestrationDependencyException.InnerException as Xeption);
            }
            catch (EventListenerV2OrchestrationServiceException
                eventListenerV2OrchestrationServiceException)
            {
                throw CreateEventAddressV2ClientDependencyException(
                    eventListenerV2OrchestrationServiceException.InnerException as Xeption);
            }
            catch (Exception exception)
            {
                throw CreateEventAddressV2ClientServiceException(exception as Xeption);
            }
        }

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
