// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventParticipants.V2.Exceptions;
using EventHighway.Portal.Web.Models.Views.EventParticipants;
using EventHighway.Portal.Web.Models.Views.EventParticipants.Exceptions;
using Xeptions;

namespace EventHighway.Portal.Web.Services.Views.EventParticipants
{
    public partial class EventParticipantsViewService
    {
        private delegate ValueTask<List<EventParticipantView>> ReturningParticipantsFunction();

        private async ValueTask<List<EventParticipantView>> TryCatch(
            ReturningParticipantsFunction returningParticipantsFunction)
        {
            try
            {
                return await returningParticipantsFunction();
            }
            catch (EventParticipantV2ClientValidationException clientValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    clientValidationException);
            }
            catch (EventParticipantV2ClientDependencyValidationException
                clientDependencyValidationException)
            {
                throw await CreateAndLogDependencyValidationExceptionAsync(
                    clientDependencyValidationException);
            }
        }

        private async ValueTask<EventParticipantsViewDependencyValidationException>
            CreateAndLogDependencyValidationExceptionAsync(Xeption exception)
        {
            var eventParticipantsViewDependencyValidationException =
                new EventParticipantsViewDependencyValidationException(
                    innerException: exception);

            await this.loggingBroker.LogErrorAsync(
                eventParticipantsViewDependencyValidationException);

            return eventParticipantsViewDependencyValidationException;
        }
    }
}
