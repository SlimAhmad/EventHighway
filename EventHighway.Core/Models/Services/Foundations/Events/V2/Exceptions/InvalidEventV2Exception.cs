// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.Events.V2.Exceptions
{
    public class InvalidEventV2Exception : Xeption
    {
        public InvalidEventV2Exception(string message)
            : base(message)
        { }
    }
}
