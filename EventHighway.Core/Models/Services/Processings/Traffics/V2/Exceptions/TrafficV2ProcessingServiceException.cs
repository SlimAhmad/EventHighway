// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.Traffics.V2.Exceptions
{
    internal class TrafficV2ProcessingServiceException : Xeption
    {
        public TrafficV2ProcessingServiceException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
