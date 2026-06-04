// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventListeners.V1.Exceptions
{
    public class NullEventListenerV1Exception : Xeption
    {
        public NullEventListenerV1Exception(string message)
            : base(message)
        { }
    }
}
