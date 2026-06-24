// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.ReplayingListenerEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.EventCalls.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.ListenerEvents.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Orchestrations.ReplayingListenerEvents.V2
{
    internal partial class ReplayingListenerEventV2OrchestrationService
    {
        private delegate ValueTask<ListenerEventV2> ReturningListenerEventV2Function();

        private async ValueTask<ListenerEventV2> TryCatch(
            ReturningListenerEventV2Function returningListenerEventV2Function)
        {
            try
            {
                return await returningListenerEventV2Function();
            }
            catch (NullReplayingListenerEventV2OrchestrationException
                nullReplayingListenerEventV2OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullReplayingListenerEventV2OrchestrationException);
            }
            catch (EventCallV2ProcessingValidationException
                eventCallV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventCallV2ProcessingValidationException);
            }
            catch (EventCallV2ProcessingDependencyValidationException
                eventCallV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventCallV2ProcessingDependencyValidationException);
            }
            catch (ListenerEventV2ProcessingValidationException
                listenerEventV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventV2ProcessingValidationException);
            }
            catch (ListenerEventV2ProcessingDependencyValidationException
                listenerEventV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventV2ProcessingDependencyValidationException);
            }
        }

        private async ValueTask<ReplayingListenerEventV2OrchestrationValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var replayingListenerEventV2OrchestrationValidationException =
                new ReplayingListenerEventV2OrchestrationValidationException(
                    message: "Replaying listener event validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(
                replayingListenerEventV2OrchestrationValidationException);

            return replayingListenerEventV2OrchestrationValidationException;
        }

        private async ValueTask<ReplayingListenerEventV2OrchestrationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var replayingListenerEventV2OrchestrationDependencyValidationException =
                new ReplayingListenerEventV2OrchestrationDependencyValidationException(
                    message: "Replaying listener event validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(
                replayingListenerEventV2OrchestrationDependencyValidationException);

            return replayingListenerEventV2OrchestrationDependencyValidationException;
        }
    }
}
