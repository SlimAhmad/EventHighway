// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using EventHighway.Abstractions.EventHandlers;
using Tynamix.ObjectFiller;

namespace EventHighway.EventHandlers.Tests.Acceptance.Exposers.DelegateEventHandlers
{
    public partial class DelegateEventHandlerTests
    {
        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static IReadOnlyDictionary<string, string> CreateRandomHandlerParams() =>
            new Dictionary<string, string>
            {
                { GetRandomString(), GetRandomString() }
            };

        private static EventHandlerResult CreateRandomEventHandlerResult() =>
            new EventHandlerResult
            {
                Response = GetRandomString(),
                ResponseCode = GetRandomString(),
                ResponseMessage = GetRandomString(),
                IsSuccess = true
            };
    }
}
