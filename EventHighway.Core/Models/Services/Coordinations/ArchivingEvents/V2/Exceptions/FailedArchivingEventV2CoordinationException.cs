// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Coordinations.ArchivingEvents.V2.Exceptions
{
    internal class FailedArchivingEventV2CoordinationException : Xeption
    {
        public FailedArchivingEventV2CoordinationException(string message)
            : base(message)
        { }
    }
}
