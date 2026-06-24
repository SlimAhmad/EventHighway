// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Processings.EventListeners.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.ListenerEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.RestoringEvents.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Orchestrations.RestoringEvents.V2
{
    internal partial class RestoringEventV2OrchestrationService
    {
        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (NullRestoringEventV2OrchestrationException nullRestoringEventV2OrchestrationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    nullRestoringEventV2OrchestrationException);
            }
            catch (EventV2ProcessingValidationException eventV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV2ProcessingValidationException);
            }
            catch (EventV2ProcessingDependencyValidationException eventV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventV2ProcessingDependencyValidationException);
            }
            catch (ListenerEventV2ProcessingValidationException listenerEventV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventV2ProcessingValidationException);
            }
            catch (ListenerEventV2ProcessingDependencyValidationException listenerEventV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    listenerEventV2ProcessingDependencyValidationException);
            }
            catch (EventListenerV2ProcessingValidationException eventListenerV2ProcessingValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventListenerV2ProcessingValidationException);
            }
            catch (EventListenerV2ProcessingDependencyValidationException eventListenerV2ProcessingDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventListenerV2ProcessingDependencyValidationException);
            }
        }

        private async ValueTask<RestoringEventV2OrchestrationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var restoringEventV2OrchestrationDependencyValidationException =
                new RestoringEventV2OrchestrationDependencyValidationException(
                    message: "Restoring event validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(
                restoringEventV2OrchestrationDependencyValidationException);

            return restoringEventV2OrchestrationDependencyValidationException;
        }

        private async ValueTask<RestoringEventV2OrchestrationValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var restoringEventV2OrchestrationValidationException =
                new RestoringEventV2OrchestrationValidationException(
                    message: "Restoring event validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(restoringEventV2OrchestrationValidationException);

            return restoringEventV2OrchestrationValidationException;
        }
    }
}
