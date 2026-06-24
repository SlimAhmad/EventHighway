// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.ReplayingEvents.V2.Exceptions;
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
