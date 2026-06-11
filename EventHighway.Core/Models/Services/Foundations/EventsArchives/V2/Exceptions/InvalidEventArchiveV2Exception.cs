// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventsArchives.V2.Exceptions
{
    public class InvalidEventArchiveV2Exception : Xeption
    {
        public InvalidEventArchiveV2Exception(string message)
            : base(message)
        { }
    }
}
