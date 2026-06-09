// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventCall.V2.Exceptions
{
    public class HandlerNotFoundEventCallV2Exception : Xeption
    {
        public HandlerNotFoundEventCallV2Exception(string message)
            : base(message)
        { }
    }
}
