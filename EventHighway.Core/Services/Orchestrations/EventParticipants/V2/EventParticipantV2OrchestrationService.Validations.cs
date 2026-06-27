// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventParticipants.V2.Exceptions;

namespace EventHighway.Core.Services.Orchestrations.EventParticipants.V2
{
    internal partial class EventParticipantV2OrchestrationService
    {
        private static void ValidateParticipant(EventParticipantV2 participant, DateTimeOffset now)
        {
            Validate(
                (Rule: IsNotFound(participant), Parameter: "Participant"),
                (Rule: IsInactive(participant), Parameter: "IsActive"),
                (Rule: IsOutsideActiveWindow(participant, now), Parameter: "ActiveWindow"));
        }

        private static dynamic IsNotFound(EventParticipantV2 participant) => new
        {
            Condition = participant is null,
            Message = "Participant not found."
        };

        private static dynamic IsInactive(EventParticipantV2 participant) => new
        {
            Condition = participant is not null && participant.IsActive is false,
            Message = "Participant is not active."
        };

        private static dynamic IsOutsideActiveWindow(EventParticipantV2 participant, DateTimeOffset now) => new
        {
            Condition = participant is not null
                && participant.IsActive
                && ((participant.ActiveFrom != null && participant.ActiveFrom > now)
                    || (participant.ActiveTo != null && participant.ActiveTo < now)),

            Message = "Participant is outside its active window."
        };

        private static void ValidateSecret(EventParticipantSecretV2 secret, DateTimeOffset now)
        {
            Validate(
                (Rule: IsSecretNotFound(secret), Parameter: "Secret"),
                (Rule: IsSecretInactive(secret), Parameter: "SecretIsActive"),
                (Rule: IsSecretOutsideActiveWindow(secret, now), Parameter: "SecretActiveWindow"));
        }

        private static dynamic IsSecretNotFound(EventParticipantSecretV2 secret) => new
        {
            Condition = secret is null,
            Message = "Secret not found."
        };

        private static dynamic IsSecretInactive(EventParticipantSecretV2 secret) => new
        {
            Condition = secret is not null && secret.IsActive is false,
            Message = "Secret is not active."
        };

        private static dynamic IsSecretOutsideActiveWindow(EventParticipantSecretV2 secret, DateTimeOffset now) => new
        {
            Condition = secret is not null
                && secret.IsActive
                && ((secret.ActiveFrom != null && secret.ActiveFrom > now)
                    || (secret.ActiveTo != null && secret.ActiveTo < now)),

            Message = "Secret is outside its active window."
        };

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidEventParticipantV2OrchestrationException =
                new InvalidEventParticipantV2OrchestrationException(
                    message: "Invalid event participant or secret, fix the errors and try again.");

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidEventParticipantV2OrchestrationException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidEventParticipantV2OrchestrationException.ThrowIfContainsErrors();
        }
    }
}
