// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Coordinations.Events.V1.Exceptions
{
    public class InvalidEventV1CoordinationException : Xeption
    {
        public InvalidEventV1CoordinationException(string message)
            : base(message)
        { }
    }
}
