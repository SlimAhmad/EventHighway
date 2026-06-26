// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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
        }

        private static EventParticipantV2ClientValidationException
            CreateClientValidationException(Xeption innerException)
        {
            return new EventParticipantV2ClientValidationException(
                message: "Event participant client validation error occurred, fix the errors and try again.",
                innerException: innerException,
                data: innerException?.Data);
        }
    }
}
