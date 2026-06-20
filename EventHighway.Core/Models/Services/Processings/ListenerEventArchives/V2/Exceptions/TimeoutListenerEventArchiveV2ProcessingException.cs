// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.ListenerEventArchives.V2.Exceptions
{
    public class TimeoutListenerEventArchiveV2ProcessingException : Xeption
    {
        public TimeoutListenerEventArchiveV2ProcessingException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
