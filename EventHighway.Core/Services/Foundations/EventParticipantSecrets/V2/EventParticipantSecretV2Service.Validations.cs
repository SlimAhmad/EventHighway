// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions;

namespace EventHighway.Core.Services.Foundations.EventParticipantSecrets.V2
{
    internal partial class EventParticipantSecretV2Service
    {
        private static void ValidateEventParticipantSecretV2IsNotNull(
            EventParticipantSecretV2 eventParticipantSecretV2)
        {
            if (eventParticipantSecretV2 is null)
            {
                throw new NullEventParticipantSecretV2Exception(
                    message: "Event participant secret is null.");
            }
        }
    }
}
