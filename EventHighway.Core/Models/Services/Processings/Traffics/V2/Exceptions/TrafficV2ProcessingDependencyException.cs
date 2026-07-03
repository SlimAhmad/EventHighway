// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Xeptions;

namespace EventHighway.Core.Models.Services.Processings.Traffics.V2.Exceptions
{
    internal class TrafficV2ProcessingDependencyException : Xeption
    {
        public TrafficV2ProcessingDependencyException(string message, Xeption innerException)
            : base(message, innerException)
        { }
    }
}
