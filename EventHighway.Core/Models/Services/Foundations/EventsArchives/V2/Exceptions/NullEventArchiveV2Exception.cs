// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventsArchives.V2.Exceptions
{
    public class NullEventArchiveV2Exception : Xeption
    {
        public NullEventArchiveV2Exception(string message)
            : base(message)
        { }
    }
}
