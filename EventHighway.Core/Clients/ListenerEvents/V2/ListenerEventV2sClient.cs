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
    internal class ListenerEventV2sClient : IListenerEventV2sClient
    {
        private readonly IEventListenerV2OrchestrationService eventListenerV2OrchestrationService;

        public ListenerEventV2sClient(IEventListenerV2OrchestrationService eventListenerV2OrchestrationService) =>
            this.eventListenerV2OrchestrationService = eventListenerV2OrchestrationService;

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
