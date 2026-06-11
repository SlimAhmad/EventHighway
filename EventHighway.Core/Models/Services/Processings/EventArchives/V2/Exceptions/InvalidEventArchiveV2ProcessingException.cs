// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.EventArchives.V2.Exceptions
{
    public class InvalidEventArchiveV2ProcessingException : Xeption
    {
        public InvalidEventArchiveV2ProcessingException(string message)
            : base(message)
        { }
    }
}
