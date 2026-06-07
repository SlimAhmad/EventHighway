// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions
{
    public class NullEventArchiveV1Exception : Xeption
    {
        public NullEventArchiveV1Exception(string message)
            : base(message)
        { }
    }
}
