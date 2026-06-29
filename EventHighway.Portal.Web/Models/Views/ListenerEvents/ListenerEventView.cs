// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.Portal.Web.Models.Views.ListenerEvents
{
    public class ListenerEventView
    {
        public Guid Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ResponseCode { get; set; } = string.Empty;
        public string ResponseMessage { get; set; } = string.Empty;
        public Guid EventId { get; set; }
        public Guid EventAddressId { get; set; }
        public Guid EventListenerId { get; set; }
        public Guid? ParticipantId { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}
