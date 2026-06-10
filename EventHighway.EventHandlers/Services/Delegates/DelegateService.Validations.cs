// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.EventHandlers.Models.Foundations.Delegates.Exceptions;

namespace EventHighway.EventHandlers.Services.Delegates
{
    internal partial class DelegateService
    {
        internal virtual void ValidateInvokeParams(
            string content,
            Func<string, IReadOnlyDictionary<string, string>, CancellationToken, ValueTask<EventHandlerResult>> handler)
        {
            Validate(
                message: "Delegate service params are invalid, fix the errors and try again.",

                (Rule: IsInvalid(content),
                Parameter: "content",
                Message: "Text required"),

                (Rule: handler is null,
                Parameter: "handler",
                Message: "Handler required"));
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
