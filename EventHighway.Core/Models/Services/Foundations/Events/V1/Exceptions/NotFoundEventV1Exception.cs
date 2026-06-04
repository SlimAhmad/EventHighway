// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.Events.V1.Exceptions
{
    public class NotFoundEventV1Exception : Xeption
    {
        public NotFoundEventV1Exception(string message)
            : base(message)
        { }
    }
}
