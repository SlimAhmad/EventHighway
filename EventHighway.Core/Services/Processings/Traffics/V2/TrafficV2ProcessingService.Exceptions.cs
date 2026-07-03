// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.Traffics.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Processings.Traffics.V2
{
    internal partial class TrafficV2ProcessingService
    {
        private delegate ValueTask<TrafficSnapshotV2> ReturningTrafficSnapshotV2Function();

        private async ValueTask<TrafficSnapshotV2> TryCatch(
            ReturningTrafficSnapshotV2Function returningTrafficSnapshotV2Function)
        {
            try
            {
                return await returningTrafficSnapshotV2Function();
            }
            catch (OperationCanceledException operationCanceledException)
                when (operationCanceledException.CancellationToken.IsCancellationRequested is false)
            {
                var timeoutException =
                    new TimeoutException("The dependency operation timed out.", operationCanceledException);

                var timeoutTrafficV2ProcessingException =
                    new TimeoutTrafficV2ProcessingException(
                        message: "Failed traffic processing timeout error occurred, contact support.",
                        innerException: timeoutException,
                        data: operationCanceledException.Data);

                throw await CreateAndLogTimeoutDependencyExceptionAsync(
                    timeoutTrafficV2ProcessingException);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (EventV2DependencyException eventV2DependencyException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV2DependencyException);
            }
            catch (EventV2ServiceException eventV2ServiceException)
            {
                throw await CreateAndLogDependencyExceptionAsync(eventV2ServiceException);
            }
            catch (Exception exception)
            {
                var failedTrafficV2ProcessingServiceException =
                    new FailedTrafficV2ProcessingServiceException(
                        message: "Failed traffic service error occurred, contact support.",
                        innerException: exception,
                        data: exception.Data);

                throw await CreateAndLogServiceExceptionAsync(
                    failedTrafficV2ProcessingServiceException);
            }
        }

        private async ValueTask<TrafficV2ProcessingDependencyException> CreateAndLogTimeoutDependencyExceptionAsync(
            Xeption exception)
        {
            var trafficV2ProcessingDependencyException =
                new TrafficV2ProcessingDependencyException(
                    message: "Traffic dependency error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(trafficV2ProcessingDependencyException);

            return trafficV2ProcessingDependencyException;
        }

        private async ValueTask<TrafficV2ProcessingDependencyException> CreateAndLogDependencyExceptionAsync(
            Xeption exception)
        {
            var trafficV2ProcessingDependencyException =
                new TrafficV2ProcessingDependencyException(
                    message: "Traffic dependency error occurred, contact support.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(trafficV2ProcessingDependencyException);

            return trafficV2ProcessingDependencyException;
        }

        private async ValueTask<TrafficV2ProcessingServiceException> CreateAndLogServiceExceptionAsync(
            Xeption exception)
        {
            var trafficV2ProcessingServiceException =
                new TrafficV2ProcessingServiceException(
                    message: "Traffic service error occurred, contact support.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(trafficV2ProcessingServiceException);

            return trafficV2ProcessingServiceException;
        }
    }
}
