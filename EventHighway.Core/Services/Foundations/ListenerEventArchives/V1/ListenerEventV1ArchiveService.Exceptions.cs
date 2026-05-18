// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace EventHighway.Core.Services.Foundations.ListenerEventArchives.V1
{
    internal partial class ListenerEventV1ArchiveService
    {
        private delegate ValueTask<ListenerEventArchiveV1> ReturningListenerEventV1ArchiveFunction();

        private async ValueTask<ListenerEventArchiveV1> TryCatch(
            ReturningListenerEventV1ArchiveFunction returningListenerEventV1ArchiveFunction)
        {
            try
            {
                return await returningListenerEventV1ArchiveFunction();
            }
            catch (NullListenerEventArchiveV1Exception
                nullListenerEventV1ArchiveException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullListenerEventV1ArchiveException);
            }
            catch (InvalidListenerEventArchiveV1Exception
                invalidListenerEventV1ArchiveException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidListenerEventV1ArchiveException);
            }
            catch (SqlException sqlException)
            {
                var failedListenerEventV1ArchiveStorageException =
                    new FailedListenerEventArchiveV1StorageException(
                        message: "Failed listener event archive storage error occurred, contact support.",
                        innerException: sqlException);

                throw await CreateAndLogCriticalDependencyExceptionAsync(
                    failedListenerEventV1ArchiveStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsListenerEventV1ArchiveException =
                    new AlreadyExistsListenerEventArchiveV1Exception(
                        message: "Listener event archive with the same id already exists.",
                        innerException: duplicateKeyException);

                throw await CreateAndLogDependencyValidationExceptionAsync(
                    alreadyExistsListenerEventV1ArchiveException);
            }
            catch (DbUpdateException dbUpdateException)
            {
                var failedListenerEventV1ArchiveStorageException =
                    new FailedListenerEventArchiveV1StorageException(
                        message: "Failed listener event archive storage error occurred, contact support.",
                        innerException: dbUpdateException);

                throw await CreateAndLogDependencyExceptionAsync(
                    failedListenerEventV1ArchiveStorageException);
            }
            catch (Exception serviceException)
            {
                var failedListenerEventV1ArchiveServiceException =
                    new FailedListenerEventArchiveV1ServiceException(
                        message: "Failed listener event archive service error occurred, contact support.",
                        innerException: serviceException);

                throw await CreateAndLogServiceExceptionAsync(
                    failedListenerEventV1ArchiveServiceException);
            }
        }

        private async ValueTask<ListenerEventArchiveV1ValidationException> CreateAndLogValidationExceptionAsync(
            Xeption exception)
        {
            var listenerEventV1ArchiveValidationException =
                new ListenerEventArchiveV1ValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(listenerEventV1ArchiveValidationException);

            return listenerEventV1ArchiveValidationException;
        }

        private async ValueTask<ListenerEventArchiveV1DependencyException> CreateAndLogCriticalDependencyExceptionAsync(
            Xeption exception)
        {
            var listenerEventV1ArchiveDependencyException =
                new ListenerEventArchiveV1DependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogCriticalAsync(listenerEventV1ArchiveDependencyException);

            return listenerEventV1ArchiveDependencyException;
        }

        private async ValueTask<ListenerEventArchiveV1DependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var listenerEventV1ArchiveDependencyValidationException =
                new ListenerEventArchiveV1DependencyValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(listenerEventV1ArchiveDependencyValidationException);

            return listenerEventV1ArchiveDependencyValidationException;
        }

        private async ValueTask<ListenerEventArchiveV1DependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var listenerEventV1ArchiveDependencyException =
                new ListenerEventArchiveV1DependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(listenerEventV1ArchiveDependencyException);

            return listenerEventV1ArchiveDependencyException;
        }

        private async ValueTask<ListenerEventArchiveV1ServiceException> CreateAndLogServiceExceptionAsync(Xeption exception)
        {
            var listenerEventV1ArchiveServiceException =
                new ListenerEventArchiveV1ServiceException(
                    message: "Listener event archive service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(listenerEventV1ArchiveServiceException);

            return listenerEventV1ArchiveServiceException;
        }
    }
}
