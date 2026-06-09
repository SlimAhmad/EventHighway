// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.Events.V2.Exceptions
{
    public class NotFoundEventV2Exception : Xeption
    {
        public NotFoundEventV2Exception(string message)
            : base(message)
        { }
    }
}
