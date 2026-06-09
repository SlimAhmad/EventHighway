// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventAddresses.V2.Exceptions
{
    public class InvalidEventAddressV2Exception : Xeption
    {
        public InvalidEventAddressV2Exception(string message)
            : base(message)
        { }
    }
}
