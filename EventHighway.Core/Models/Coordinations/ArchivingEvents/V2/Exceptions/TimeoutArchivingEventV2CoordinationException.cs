// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Coordinations.ArchivingEvents.V2.Exceptions
{
    internal class TimeoutArchivingEventV2CoordinationException : Xeption
    {
        public TimeoutArchivingEventV2CoordinationException(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
