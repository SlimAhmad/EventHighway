// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventParticipants.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions;
using EventHighway.Core.Services.Foundations.EventParticipants.V2;
using Xeptions;

namespace EventHighway.Core.Clients.EventParticipants.V2
{
    internal class EventParticipantV2Client : IEventParticipantV2Client
    {
        private readonly IEventParticipantV2Service eventParticipantV2Service;

        public EventParticipantV2Client(IEventParticipantV2Service eventParticipantV2Service) =>
            this.eventParticipantV2Service = eventParticipantV2Service;

        public async ValueTask<EventParticipantV2> AddEventParticipantV2Async(
            EventParticipantV2 eventParticipantV2,
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.eventParticipantV2Service
                    .AddEventParticipantV2Async(eventParticipantV2, cancellationToken);
            }
            catch (EventParticipantV2ValidationException eventParticipantV2ValidationException)
            {
                throw CreateClientValidationException(
                    eventParticipantV2ValidationException.InnerException as Xeption);
            }
            catch (EventParticipantV2DependencyValidationException
                eventParticipantV2DependencyValidationException)
            {
                throw CreateClientValidationException(
                    eventParticipantV2DependencyValidationException.InnerException as Xeption);
            }
            catch (EventParticipantV2DependencyException eventParticipantV2DependencyException)
            {
                throw CreateClientDependencyException(
                    eventParticipantV2DependencyException.InnerException as Xeption);
            }
            catch (EventParticipantV2ServiceException eventParticipantV2ServiceException)
            {
                throw CreateClientDependencyException(
                    eventParticipantV2ServiceException.InnerException as Xeption);
            }
            catch (Exception exception)
            {
                throw CreateClientServiceException(exception as Xeption);
            }
        }

        public async ValueTask<IEnumerable<EventParticipantV2>> RetrieveAllEventParticipantV2sAsync(
            CancellationToken cancellationToken = default) =>
                await this.eventParticipantV2Service
                    .RetrieveAllEventParticipantV2sAsync(cancellationToken);

        private static EventParticipantV2ClientValidationException
            CreateClientValidationException(Xeption innerException)
        {
            return new EventParticipantV2ClientValidationException(
                message: "Event participant client validation error occurred, fix the errors and try again.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static EventParticipantV2ClientDependencyException
            CreateClientDependencyException(Xeption innerException)
        {
            return new EventParticipantV2ClientDependencyException(
                message: "Event participant client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static EventParticipantV2ClientServiceException
            CreateClientServiceException(Xeption innerException)
        {
            return new EventParticipantV2ClientServiceException(
                message: "Event participant client service error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }
    }
}
