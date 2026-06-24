// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.EventCalls.V2.Exceptions
{
    public class InvalidEventCallV2ProcessingException : Xeption
    {
        public InvalidEventCallV2ProcessingException(string message)
            : base(message)
        { }
    }
}
