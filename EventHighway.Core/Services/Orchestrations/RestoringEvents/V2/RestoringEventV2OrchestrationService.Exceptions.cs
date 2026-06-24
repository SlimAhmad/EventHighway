// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
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
