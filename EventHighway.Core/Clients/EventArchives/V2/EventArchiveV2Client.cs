// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventArchives.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2.Exceptions;
using EventHighway.Core.Services.Foundations.EventArchives.V2;
using Xeptions;

namespace EventHighway.Core.Clients.EventArchives.V2
{
    /// <summary>
    /// Represents the V2 event archive client implementation, exposing read operations over
    /// archived events while managing foundation service exceptions.
    /// </summary>
    internal class EventArchiveV2Client : IEventArchiveV2Client
    {
        private readonly IEventArchiveV2Service eventArchiveV2Service;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventArchiveV2Client"/> class with the
        /// specified event archive service.
        /// </summary>
        /// <param name="eventArchiveV2Service">The foundation service for archived events.</param>
        public EventArchiveV2Client(IEventArchiveV2Service eventArchiveV2Service) =>
            this.eventArchiveV2Service = eventArchiveV2Service;

        public async ValueTask<IQueryable<EventArchiveV2>> RetrieveAllEventArchiveV2sAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.eventArchiveV2Service
                    .RetrieveAllEventArchiveV2sAsync(cancellationToken);
            }
            catch (EventArchiveV2ValidationException
                eventArchiveV2ValidationException)
            {
                throw CreateEventArchiveV2ClientValidationException(
                    eventArchiveV2ValidationException.InnerException as Xeption);
            }
            catch (EventArchiveV2DependencyValidationException
                eventArchiveV2DependencyValidationException)
            {
                throw CreateEventArchiveV2ClientValidationException(
                    eventArchiveV2DependencyValidationException.InnerException as Xeption);
            }
            catch (EventArchiveV2DependencyException
                eventArchiveV2DependencyException)
            {
                throw CreateEventArchiveV2ClientDependencyException(
                    eventArchiveV2DependencyException.InnerException as Xeption);
            }
            catch (EventArchiveV2ServiceException
                eventArchiveV2ServiceException)
            {
                throw CreateEventArchiveV2ClientDependencyException(
                    eventArchiveV2ServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateEventArchiveV2ClientServiceException(exception as Xeption);
            }
        }

        public async ValueTask<EventArchiveV2> RetrieveEventArchiveV2ByIdAsync(
            Guid eventArchiveV2Id,
            CancellationToken cancellationToken = default)
        {
            return await this.eventArchiveV2Service
                .RetrieveEventArchiveV2ByIdAsync(eventArchiveV2Id, cancellationToken);
        }

        private static EventArchiveV2ClientValidationException
            CreateEventArchiveV2ClientValidationException(Xeption innerException)
        {
            return new EventArchiveV2ClientValidationException(
                message: "Event archive client validation error occurred, fix the errors and try again.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static EventArchiveV2ClientDependencyException
            CreateEventArchiveV2ClientDependencyException(Xeption innerException)
        {
            return new EventArchiveV2ClientDependencyException(
                message: "Event archive client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static EventArchiveV2ClientServiceException
            CreateEventArchiveV2ClientServiceException(Xeption innerException)
        {
            return new EventArchiveV2ClientServiceException(
                message: "Event archive client service error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }
    }
}
