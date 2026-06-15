// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace EventHighway.Abstractions.EventHandlers
{
    /// <summary>
    /// Represents the result of an event handler execution, containing the response data, status
    /// code, and success indicator.
    /// </summary>
    public class EventHandlerResult
    {
        /// <summary>
        /// Gets or sets the response data returned by the event handler.
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// Gets or sets the response code indicating the status of the event handler execution.
        /// </summary>
        public string ResponseCode { get; set; }

        /// <summary>
        /// Gets or sets the response message providing additional details about the execution
        /// result.
        /// </summary>
        public string ResponseMessage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event handler execution was successful.
        /// </summary>
        public bool IsSuccess { get; set; }
    }
}
