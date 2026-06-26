// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions;
using Xeptions;

namespace EventHighway.Core.Services.Foundations.EventParticipants.V2
{
    internal partial class EventParticipantV2Service
    {
        private delegate ValueTask<EventParticipantV2> ReturningEventParticipantV2Function();

        private async ValueTask<EventParticipantV2> TryCatch(
            ReturningEventParticipantV2Function returningEventParticipantV2Function)
        {
            try
            {
                return await returningEventParticipantV2Function();
            }
            catch (NullEventParticipantV2Exception nullEventParticipantV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(nullEventParticipantV2Exception);
            }
            catch (InvalidEventParticipantV2Exception invalidEventParticipantV2Exception)
            {
                throw await CreateAndLogValidationExceptionAsync(invalidEventParticipantV2Exception);
            }
        }

        private async ValueTask<EventParticipantV2ValidationException>
            CreateAndLogValidationExceptionAsync(Xeption exception)
        {
            var eventParticipantV2ValidationException =
                new EventParticipantV2ValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(eventParticipantV2ValidationException);

            return eventParticipantV2ValidationException;
        }
    }
}
