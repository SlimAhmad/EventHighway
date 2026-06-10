// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using EventHighway.EventHandlers.Models.Foundations.Rest.Exceptions;

namespace EventHighway.EventHandlers.Servies.Rest
{
    internal partial class RestService
    {
        private static void ValidatePostWithSecretParams(
            string content,
            IReadOnlyDictionary<string, string> handlerParams)
        {
            Validate(
                message: "Rest service params are invalid, fix the errors and try again.",

                (Rule: IsInvalid(content),
                Parameter: "content",
                Message: "Text required"),

                (Rule: handlerParams is null,
                Parameter: "handlerParams",
                Message: "Params required"));

            Validate(
                message: "Rest service params are invalid, fix the errors and try again.",

                (Rule: !handlerParams.ContainsKey("url"),
                Parameter: "url",
                Message: "Config item required"),

                (Rule: handlerParams.ContainsKey("url") && IsInvalid(handlerParams["url"]),
                Parameter: "url",
                Message: "Value required"),

                (Rule: !handlerParams.ContainsKey("secret"),
                Parameter: "secret",
                Message: "Config item required"),

                (Rule: handlerParams.ContainsKey("secret") && IsInvalid(handlerParams["secret"]),
                Parameter: "secret",
                Message: "Value required"));
        }

        private static bool IsInvalid(string text) =>
            string.IsNullOrWhiteSpace(text);

        private static void Validate(
            string message,
            params (bool Rule, string Parameter, string Message)[] validations)
        {
            var invalidRestServiceException =
                new InvalidRestServiceException(message);

            foreach ((bool rule, string parameter, string errorMessage) in validations)
            {
                if (rule)
                {
                    invalidRestServiceException.UpsertDataList(
                        key: parameter,
                        value: errorMessage);
                }
            }

            invalidRestServiceException.ThrowIfContainsErrors();
        }
    }
}
