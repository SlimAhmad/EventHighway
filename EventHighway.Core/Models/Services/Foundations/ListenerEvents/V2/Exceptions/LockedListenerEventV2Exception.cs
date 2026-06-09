// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2.Exceptions
{
    public class LockedListenerEventV2Exception : Xeption
    {
        public LockedListenerEventV2Exception(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
