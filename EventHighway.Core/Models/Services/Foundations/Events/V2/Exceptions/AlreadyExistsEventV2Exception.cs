// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections;
using Xeptions;

namespace EventHighway.Core.Models.Services.Foundations.Events.V2.Exceptions
{
    public class AlreadyExistsEventV2Exception : Xeption
    {
        public AlreadyExistsEventV2Exception(string message, Exception innerException, IDictionary data)
            : base(message, innerException, data)
        { }
    }
}
