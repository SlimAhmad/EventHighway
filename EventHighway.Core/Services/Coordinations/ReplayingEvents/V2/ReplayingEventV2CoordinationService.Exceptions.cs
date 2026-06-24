// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.ReplayingEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.RestoringEvents.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Coordinations.ReplayingEvents.V2
{
    internal partial class ReplayingEventV2CoordinationService
    {
        private delegate ValueTask ReturningNothingFunction();

        private async ValueTask TryCatch(ReturningNothingFunction returningNothingFunction)
        {
            try
            {
                await returningNothingFunction();
            }
            catch (InvalidReplayingEventV2CoordinationException invalidReplayingEventV2CoordinationException)
            {
                throw await CreateAndLogValidationExceptionAsync(
                    invalidReplayingEventV2CoordinationException);
            }
            catch (EventArchiveV2OrchestrationValidationException eventArchiveV2OrchestrationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventArchiveV2OrchestrationValidationException);
            }
            catch (EventArchiveV2OrchestrationDependencyValidationException eventArchiveV2OrchestrationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    eventArchiveV2OrchestrationDependencyValidationException);
            }
            catch (RestoringEventV2OrchestrationValidationException restoringEventV2OrchestrationValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    restoringEventV2OrchestrationValidationException);
            }
            catch (RestoringEventV2OrchestrationDependencyValidationException restoringEventV2OrchestrationDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    restoringEventV2OrchestrationDependencyValidationException);
            }
        }

        private async ValueTask<ReplayingEventV2CoordinationDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var replayingEventV2CoordinationDependencyValidationException =
                new ReplayingEventV2CoordinationDependencyValidationException(
                    message: "Replaying event validation error occurred, fix the errors and try again.",
                    innerException: exception.InnerException as Xeption);

            await this.loggingBroker.LogErrorAsync(
                replayingEventV2CoordinationDependencyValidationException);

            return replayingEventV2CoordinationDependencyValidationException;
        }

        private async ValueTask<ReplayingEventV2CoordinationValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var replayingEventV2CoordinationValidationException =
                new ReplayingEventV2CoordinationValidationException(
                    message: "Replaying event validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(
                replayingEventV2CoordinationValidationException);

            return replayingEventV2CoordinationValidationException;
        }
    }
}
