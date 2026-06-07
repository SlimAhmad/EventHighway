// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.ListenerEvents.V1.Exceptions
{
    public class NullListenerEventV1Exception : Xeption
    {
        public NullListenerEventV1Exception(string message)
            : base(message)
        { }
    }
}
