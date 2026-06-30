// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.ListenerEventArchives.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2.Exceptions;
using EventHighway.Core.Services.Foundations.ListenerEventArchives.V2;
using Xeptions;

namespace EventHighway.Core.Clients.ListenerEventArchives.V2
{
    /// <summary>
    /// Represents the V2 listener event archive client implementation, exposing read operations
    /// over archived listener events while managing foundation service exceptions.
    /// </summary>
    internal class ListenerEventArchiveV2Client : IListenerEventArchiveV2Client
    {
        private readonly IListenerEventArchiveV2Service listenerEventArchiveV2Service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListenerEventArchiveV2Client"/> class with
        /// the specified listener event archive service.
        /// </summary>
        /// <param name="listenerEventArchiveV2Service">The foundation service for archived listener
        /// events.</param>
        public ListenerEventArchiveV2Client(IListenerEventArchiveV2Service listenerEventArchiveV2Service) =>
            this.listenerEventArchiveV2Service = listenerEventArchiveV2Service;

        public async ValueTask<IQueryable<ListenerEventArchiveV2>> RetrieveAllListenerEventArchiveV2sAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                return await this.listenerEventArchiveV2Service
                    .RetrieveAllListenerEventArchiveV2sAsync(cancellationToken);
            }
            catch (ListenerEventArchiveV2ValidationException
                listenerEventArchiveV2ValidationException)
            {
                throw CreateListenerEventArchiveV2ClientValidationException(
                    listenerEventArchiveV2ValidationException.InnerException as Xeption);
            }
            catch (ListenerEventArchiveV2DependencyValidationException
                listenerEventArchiveV2DependencyValidationException)
            {
                throw CreateListenerEventArchiveV2ClientValidationException(
                    listenerEventArchiveV2DependencyValidationException.InnerException as Xeption);
            }
            catch (ListenerEventArchiveV2DependencyException
                listenerEventArchiveV2DependencyException)
            {
                throw CreateListenerEventArchiveV2ClientDependencyException(
                    listenerEventArchiveV2DependencyException.InnerException as Xeption);
            }
            catch (ListenerEventArchiveV2ServiceException
                listenerEventArchiveV2ServiceException)
            {
                throw CreateListenerEventArchiveV2ClientDependencyException(
                    listenerEventArchiveV2ServiceException.InnerException as Xeption);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                throw CreateListenerEventArchiveV2ClientServiceException(exception as Xeption);
            }
        }

        private static ListenerEventArchiveV2ClientValidationException
            CreateListenerEventArchiveV2ClientValidationException(Xeption innerException)
        {
            return new ListenerEventArchiveV2ClientValidationException(
                message: "Listener event archive client validation error occurred, fix the errors and try again.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static ListenerEventArchiveV2ClientDependencyException
            CreateListenerEventArchiveV2ClientDependencyException(Xeption innerException)
        {
            return new ListenerEventArchiveV2ClientDependencyException(
                message: "Listener event archive client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }

        private static ListenerEventArchiveV2ClientServiceException
            CreateListenerEventArchiveV2ClientServiceException(Xeption innerException)
        {
            return new ListenerEventArchiveV2ClientServiceException(
                message: "Listener event archive client service error occurred, contact support.",
                innerException: innerException,
                data: innerException?.Data);
        }
    }
}
