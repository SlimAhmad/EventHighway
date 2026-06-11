// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.EventAddresses.V2.Exceptions
{
    internal class EventAddressV2ProcessingValidationException : Xeption
    {
        public EventAddressV2ProcessingValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
