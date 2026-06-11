// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.ListenerEventArchives.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Processings.ListenerEventArchives.V2
{
    internal partial class ListenerEventArchiveV2ProcessingService
    {
        private delegate ValueTask<ListenerEventArchiveV2> ReturningListenerEventArchiveV2Function();

        private async ValueTask<ListenerEventArchiveV2> TryCatch(
            ReturningListenerEventArchiveV2Function returningListenerEventArchiveV2Function)
        {
            try
            {
                return await returningListenerEventArchiveV2Function();
            }
            catch (NullListenerEventArchiveV2ProcessingException nullListenerEventArchiveV2ProcessingException)
            {
                throw await CreateAndLogValidationExceptionAsync(nullListenerEventArchiveV2ProcessingException);
            }
            catch (ListenerEventArchiveV2ValidationException listenerEventArchiveV2ValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(listenerEventArchiveV2ValidationException);
            }
            catch (ListenerEventArchiveV2DependencyValidationException listenerEventArchiveV2DependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(listenerEventArchiveV2DependencyValidationException);
            }
            catch (ListenerEventArchiveV2DependencyException listenerEventArchiveV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(listenerEventArchiveV2DependencyException);
            }
            catch (ListenerEventArchiveV2ServiceException listenerEventArchiveV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(listenerEventArchiveV2ServiceException);
            }
            catch (Exception exception)
            {
                var failedListenerEventArchiveV2ProcessingServiceException =
                    new FailedListenerEventArchiveV2ProcessingServiceException(
                        message: "Failed listener event archive service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(failedListenerEventArchiveV2ProcessingServiceException);
            }
        }

        private async ValueTask<ListenerEventArchiveV2ProcessingValidationException>
            CreateAndLogValidationExceptionAsync(
                Xeption exception)
        {
            var listenerEventArchiveV2ProcessingValidationException =
                new ListenerEventArchiveV2ProcessingValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(listenerEventArchiveV2ProcessingValidationException);

            return listenerEventArchiveV2ProcessingValidationException;
        }

        private async ValueTask<ListenerEventArchiveV2ProcessingDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(
                Xeption exception)
        {
            var listenerEventArchiveV2ProcessingDependencyValidationException =
                new ListenerEventArchiveV2ProcessingDependencyValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(listenerEventArchiveV2ProcessingDependencyValidationException);

            return listenerEventArchiveV2ProcessingDependencyValidationException;
        }

        private async ValueTask<ListenerEventArchiveV2ProcessingDependencyException>
            CreateAndLogDependencyExceptionAsync(
                Xeption exception)
        {
            var listenerEventArchiveV2ProcessingDependencyException =
                new ListenerEventArchiveV2ProcessingDependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(listenerEventArchiveV2ProcessingDependencyException);

            return listenerEventArchiveV2ProcessingDependencyException;
        }

        private async ValueTask<ListenerEventArchiveV2ProcessingServiceException>
            CreateAndLogServiceExceptionAsync(
                Xeption exception)
        {
            var listenerEventArchiveV2ProcessingServiceException =
                new ListenerEventArchiveV2ProcessingServiceException(
                    message: "Listener event archive service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(listenerEventArchiveV2ProcessingServiceException);

            return listenerEventArchiveV2ProcessingServiceException;
        }
    }
}
