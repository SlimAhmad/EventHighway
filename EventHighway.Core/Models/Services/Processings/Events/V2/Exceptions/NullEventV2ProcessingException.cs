// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.Events.V2.Exceptions
{
    public class NullEventV2ProcessingException : Xeption
    {
        public NullEventV2ProcessingException(string message)
            : base(message)
        { }
    }
}
