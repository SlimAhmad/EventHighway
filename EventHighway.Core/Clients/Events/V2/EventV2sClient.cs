// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Coordinations.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Services.Coordinations.Events.V2;
using Xeptions;

namespace EventHighway.Core.Clients.Events.V2
{
    internal class EventV2sClient : IEventV2sClient
    {
        private readonly IEventV2CoordinationService eventV2CoordinationService;

        public EventV2sClient(IEventV2CoordinationService eventV2CoordinationService) =>
            this.eventV2CoordinationService = eventV2CoordinationService;

        public async ValueTask<EventV2> SubmitEventV2Async(
            EventV2 eventV2,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.eventV2CoordinationService
                    .SubmitEventV2Async(eventV2, cancellationToken);
            }
            catch (EventV2CoordinationValidationException
                eventV2CoordinationValidationException)
            {
                throw CreateEventV2ClientValidationException(
                    eventV2CoordinationValidationException.InnerException as Xeption);
            }
            catch (EventV2CoordinationDependencyValidationException
                eventV2CoordinationDependencyValidationException)
            {
                throw CreateEventV2ClientValidationException(
                    eventV2CoordinationDependencyValidationException.InnerException as Xeption);
            }
            catch (EventV2CoordinationDependencyException
                eventV2CoordinationDependencyException)
            {
                throw CreateEventV2ClientDependencyException(
                    eventV2CoordinationDependencyException.InnerException as Xeption);
            }
            catch (EventV2CoordinationServiceException
                eventV2CoordinationServiceException)
            {
                throw CreateEventV2ClientDependencyException(
                    eventV2CoordinationServiceException.InnerException as Xeption);
            }
            catch (Exception exception)
            {
                throw CreateEventV2ClientServiceException(exception as Xeption);
            }
        }

        public async ValueTask FireScheduledPendingEventV2sAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                await this.eventV2CoordinationService
                    .FireScheduledPendingEventV2sAsync(cancellationToken);
            }
            catch (EventV2CoordinationDependencyValidationException
                eventV2CoordinationDependencyValidationException)
            {
                throw CreateEventV2ClientValidationException(
                    eventV2CoordinationDependencyValidationException.InnerException as Xeption);
            }
            catch (EventV2CoordinationDependencyException
                eventV2CoordinationDependencyException)
            {
                throw CreateEventV2ClientDependencyException(
                    eventV2CoordinationDependencyException.InnerException as Xeption);
            }
            catch (EventV2CoordinationServiceException
                eventV2CoordinationServiceException)
            {
                throw CreateEventV2ClientDependencyException(
                    eventV2CoordinationServiceException.InnerException as Xeption);
            }
            catch (Exception exception)
            {
                throw CreateEventV2ClientServiceException(exception as Xeption);
            }
        }

        public async ValueTask<EventV2> RemoveEventV2ByIdAsync(
            Guid eventV2Id,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.eventV2CoordinationService
                    .RemoveEventV2ByIdAsync(eventV2Id, cancellationToken);
            }
            catch (EventV2CoordinationValidationException
                eventV2CoordinationValidationException)
            {
                throw CreateEventV2ClientValidationException(
                    eventV2CoordinationValidationException.InnerException as Xeption);
            }
            catch (EventV2CoordinationDependencyValidationException
                eventV2CoordinationDependencyValidationException)
            {
                throw CreateEventV2ClientValidationException(
                    eventV2CoordinationDependencyValidationException.InnerException as Xeption);
            }
            catch (EventV2CoordinationDependencyException
                eventV2CoordinationDependencyException)
            {
                throw CreateEventV2ClientDependencyException(
                    eventV2CoordinationDependencyException.InnerException as Xeption);
            }
            catch (EventV2CoordinationServiceException
                eventV2CoordinationServiceException)
            {
                throw CreateEventV2ClientDependencyException(
                    eventV2CoordinationServiceException.InnerException as Xeption);
            }
            catch (Exception exception)
            {
                throw CreateEventV2ClientServiceException(exception as Xeption);
            }
        }

        private static EventV2ClientValidationException
            CreateEventV2ClientValidationException(Xeption innerException)
        {
            return new EventV2ClientValidationException(
                message: "Event client validation error occurred, fix the errors and try again.",
                innerException: innerException,
                data: innerException.Data);
        }

        private static EventV2ClientDependencyException
            CreateEventV2ClientDependencyException(Xeption innerException)
        {
            return new EventV2ClientDependencyException(
                message: "Event client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException.Data);
        }

        private static EventV2ClientServiceException
            CreateEventV2ClientServiceException(Xeption innerException)
        {
            return new EventV2ClientServiceException(
                message: "Event client service error occurred, contact support.",
                innerException: innerException,
                data: innerException.Data);
        }
    }
}
