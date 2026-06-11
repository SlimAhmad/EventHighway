// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2.Exceptions
{
    internal class ListenerEventArchiveV2DependencyException : Xeption
    {
        public ListenerEventArchiveV2DependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
