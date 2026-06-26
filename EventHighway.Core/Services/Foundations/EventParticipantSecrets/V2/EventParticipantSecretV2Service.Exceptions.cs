// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Foundations.EventParticipantSecrets.V2
{
    internal partial class EventParticipantSecretV2Service
    {
        private delegate ValueTask<EventParticipantSecretV2> ReturningEventParticipantSecretV2Function();

        private async ValueTask<EventParticipantSecretV2> TryCatch(
            ReturningEventParticipantSecretV2Function returningEventParticipantSecretV2Function)
        {
            try
            {
                return await returningEventParticipantSecretV2Function();
            }
            catch (NullEventParticipantSecretV2Exception nullEventParticipantSecretV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(nullEventParticipantSecretV2Exception);
            }
            catch (InvalidEventParticipantSecretV2Exception invalidEventParticipantSecretV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidEventParticipantSecretV2Exception);
            }
        }

        private async ValueTask<EventParticipantSecretV2ValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var eventParticipantSecretV2ValidationException =
                new EventParticipantSecretV2ValidationException(
                    message: "Event participant secret validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventParticipantSecretV2ValidationException);

            return eventParticipantSecretV2ValidationException;
        }
    }
}
