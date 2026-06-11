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

        private static void ValidatePostWithBearerTokenParams(
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

                (Rule: !handlerParams.ContainsKey("clientId"),
                Parameter: "clientId",
                Message: "Config item required"),

                (Rule: handlerParams.ContainsKey("clientId") && IsInvalid(handlerParams["clientId"]),
                Parameter: "clientId",
                Message: "Value required"),

                (Rule: !handlerParams.ContainsKey("clientSecret"),
                Parameter: "clientSecret",
                Message: "Config item required"),

                (Rule: handlerParams.ContainsKey("clientSecret") && IsInvalid(handlerParams["clientSecret"]),
                Parameter: "clientSecret",
                Message: "Value required"),

                (Rule: !handlerParams.ContainsKey("scope"),
                Parameter: "scope",
                Message: "Config item required"),

                (Rule: handlerParams.ContainsKey("scope") && IsInvalid(handlerParams["scope"]),
                Parameter: "scope",
                Message: "Value required"),

                (Rule: !handlerParams.ContainsKey("grantType"),
                Parameter: "grantType",
                Message: "Config item required"),

                (Rule: handlerParams.ContainsKey("grantType") && IsInvalid(handlerParams["grantType"]),
                Parameter: "grantType",
                Message: "Value required"),

                (Rule: !handlerParams.ContainsKey("tokenUrl"),
                Parameter: "tokenUrl",
                Message: "Config item required"),

                (Rule: handlerParams.ContainsKey("tokenUrl") && IsInvalid(handlerParams["tokenUrl"]),
                Parameter: "tokenUrl",
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
