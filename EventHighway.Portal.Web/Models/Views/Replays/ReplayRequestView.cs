// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace EventHighway.Portal.Web.Models.Views.Replays
{
    public class ReplayRequestView
    {
        public Guid? EventAddressId { get; set; }
        public List<Guid> EventListenerIds { get; set; } = new();
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
    }
}
