// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.ListenerEvents.V2.Exceptions
{
    internal class ListenerEventV2ProcessingValidationException : Xeption
    {
        public ListenerEventV2ProcessingValidationException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
