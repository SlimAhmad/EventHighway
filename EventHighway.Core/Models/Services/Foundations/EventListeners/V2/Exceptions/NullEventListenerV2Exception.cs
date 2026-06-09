// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventListeners.V2.Exceptions
{
    public class NullEventListenerV2Exception : Xeption
    {
        public NullEventListenerV2Exception(string message)
            : base(message)
        { }
    }
}
