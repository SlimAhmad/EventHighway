// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.ListenerEventArchives.V2.Exceptions
{
    internal class ListenerEventArchiveV2ProcessingValidationException : Xeption
    {
        public ListenerEventArchiveV2ProcessingValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
