// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Coordinations.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Services.Coordinations.ArchivingEvents.V2;
using Xeptions;

namespace EventHighway.Core.Clients.ArchivingEvents.V2
{
    internal class ArchivingEventV2Client : IArchivingEventV2Client
    {
        private readonly IArchivingEventV2CoordinationService archivingEventV2CoordinationService;

        public ArchivingEventV2Client(IArchivingEventV2CoordinationService archivingEventV2CoordinationService) =>
            this.archivingEventV2CoordinationService = archivingEventV2CoordinationService;

        public async ValueTask ArchiveDeadEventV2sAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await this.archivingEventV2CoordinationService
                    .ArchiveDeadEventV2sAsync(cancellationToken);
            }
            catch (ArchivingEventV2CoordinationValidationException
                archivingEventV2CoordinationValidationException)
            {
                throw CreateArchivingEventV2ClientValidationException(
                    archivingEventV2CoordinationValidationException.InnerException as Xeption);
            }
            catch (ArchivingEventV2CoordinationDependencyValidationException
                archivingEventV2CoordinationDependencyValidationException)
            {
                throw CreateArchivingEventV2ClientValidationException(
                    archivingEventV2CoordinationDependencyValidationException.InnerException as Xeption);
            }
            catch (ArchivingEventV2CoordinationDependencyException
                archivingEventV2CoordinationDependencyException)
            {
                throw CreateArchivingEventV2ClientDependencyException(
                    archivingEventV2CoordinationDependencyException.InnerException as Xeption);
            }
            catch (ArchivingEventV2CoordinationServiceException
                archivingEventV2CoordinationServiceException)
            {
                throw CreateArchivingEventV2ClientDependencyException(
                    archivingEventV2CoordinationServiceException.InnerException as Xeption);
            }
            catch (Exception exception)
            {
                throw CreateArchivingEventV2ClientServiceException(exception as Xeption);
            }
        }

        private static ArchivingEventV2ClientValidationException
            CreateArchivingEventV2ClientValidationException(Xeption innerException)
        {
            return new ArchivingEventV2ClientValidationException(
                message: "Archiving event client validation error occurred, fix the errors and try again.",
                innerException: innerException,
                data: innerException.Data);
        }

        private static ArchivingEventV2ClientDependencyException
            CreateArchivingEventV2ClientDependencyException(Xeption innerException)
        {
            return new ArchivingEventV2ClientDependencyException(
                message: "Archiving event client dependency error occurred, contact support.",
                innerException: innerException,
                data: innerException.Data);
        }

        private static ArchivingEventV2ClientServiceException
            CreateArchivingEventV2ClientServiceException(Xeption innerException)
        {
            return new ArchivingEventV2ClientServiceException(
                message: "Archiving event client service error occurred, contact support.",
                innerException: innerException,
                data: innerException.Data);
        }
    }
}
