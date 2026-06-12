// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Coordinations.ArchivingEvents.V2.Exceptions
{
    internal class ArchivingEventV2CoordinationDependencyException : Xeption
    {
        public ArchivingEventV2CoordinationDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
