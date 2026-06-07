// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.ListenerEvents.V1.Exceptions
{
    public class NullListenerEventV1ProcessingException : Xeption
    {
        public NullListenerEventV1ProcessingException(string message)
            : base(message)
        { }
    }
}
