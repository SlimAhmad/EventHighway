// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.Portal.Web.Models.Views.EventListeners
{
    public class EventListenerView
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string HandlerName { get; set; } = string.Empty;
        public Guid EventAddressId { get; set; }
        public Guid? ParticipantId { get; set; }
    }
}
