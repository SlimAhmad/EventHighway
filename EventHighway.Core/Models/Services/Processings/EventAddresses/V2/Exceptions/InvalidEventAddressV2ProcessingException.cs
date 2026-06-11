// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.EventAddresses.V2.Exceptions
{
    public class InvalidEventAddressV2ProcessingException : Xeption
    {
        public InvalidEventAddressV2ProcessingException(string message)
            : base(message)
        { }
    }
}
