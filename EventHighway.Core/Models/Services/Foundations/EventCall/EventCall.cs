// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace EventHighway.Core.Models.Services.Foundations.EventCall
{
    internal class EventCall
    {
        public string Endpoint { get; set; }
        public string Secret { get; set; }
        public string Content { get; set; }
        public string Response { get; set; }
    }
}
