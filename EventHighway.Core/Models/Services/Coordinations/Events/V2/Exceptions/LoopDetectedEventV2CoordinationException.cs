// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Coordinations.Events.V2.Exceptions
{
    public class LoopDetectedEventV2CoordinationException : Xeption
    {
        public LoopDetectedEventV2CoordinationException(string message)
            : base(message)
        { }
    }
}
