// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;

namespace EventHighway.Portal.Web.Models.Views.EventArchives
{
    public class EventArchiveView
    {
        public Guid Id { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int RemainingRetryAttempts { get; set; }
        public Guid EventAddressV2Id { get; set; }
        public string EventAddressName { get; set; } = string.Empty;
        public Guid? EventParticipantV2Id { get; set; }
        public DateTimeOffset? ScheduledDate { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public DateTimeOffset ArchivedDate { get; set; }
    }
}
