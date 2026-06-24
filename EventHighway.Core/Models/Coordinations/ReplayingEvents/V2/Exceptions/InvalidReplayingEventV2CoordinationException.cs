// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Coordinations.ReplayingEvents.V2.Exceptions
{
    internal class InvalidReplayingEventV2CoordinationException : Xeption
    {
        public InvalidReplayingEventV2CoordinationException(string message)
            : base(message)
        { }
    }
}
