// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventHandler.V2.Exceptions
{
    public class InvalidEventHandlerV2Exception : Xeption
    {
        public InvalidEventHandlerV2Exception(string message)
            : base(message)
        { }
    }
}
