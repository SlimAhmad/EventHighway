// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using EventHighway.EventHandlers.Models.Foundations.Delegates.Exceptions;

namespace EventHighway.EventHandlers.Services.Delegates
{
    internal partial class DelegateService
    {
        internal virtual void ValidateInvokeParams(string content)
        {
            Validate(
                message: "Delegate service params are invalid, fix the errors and try again.",

                (Rule: IsInvalid(content),
                Parameter: "content",
                Message: "Text required"));
        }

        private static bool IsInvalid(string text) =>
            string.IsNullOrWhiteSpace(text);

        private static void Validate(
            string message,
            params (bool Rule, string Parameter, string Message)[] validations)
        {
            var invalidDelegateServiceException =
                new InvalidDelegateServiceException(message);

            foreach ((bool rule, string parameter, string errorMessage) in validations)
            {
                if (rule)
                {
                    invalidDelegateServiceException.UpsertDataList(
                        key: parameter,
                        value: errorMessage);
                }
            }

            invalidDelegateServiceException.ThrowIfContainsErrors();
        }
    }
}
