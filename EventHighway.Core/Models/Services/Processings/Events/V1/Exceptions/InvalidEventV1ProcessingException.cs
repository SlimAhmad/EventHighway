// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.Events.V1.Exceptions
{
    public class InvalidEventV1ProcessingException : Xeption
    {
        public InvalidEventV1ProcessingException(string message)
            : base(message)
        { }
    }
}
