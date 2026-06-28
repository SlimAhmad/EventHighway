// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.ClientV2.SubstrateApp.Models.ExternalMediaItems.Exceptions
{
    public class ExternalMediaItemValidationException : Xeption
    {
        public ExternalMediaItemValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
