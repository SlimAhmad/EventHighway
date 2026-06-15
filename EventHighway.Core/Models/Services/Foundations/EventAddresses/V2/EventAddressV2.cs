// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;

namespace EventHighway.Core.Models.Services.Foundations.EventAddresses.V2
{
    /// <summary>
    /// Represents an event address in the V2 service model, defining a logical address where
    /// events can be published and received by listeners.
    /// </summary>
    public class EventAddressV2
    {
        /// <summary>
        /// Gets or sets the unique identifier for this event address.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of this event address.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of this event address.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this event address was created.
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this event address was last updated.
        /// </summary>
        public DateTimeOffset UpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets the collection of events associated with this event address.
        /// </summary>
        public IEnumerable<EventV2> Events { get; set; }

        /// <summary>
        /// Gets or sets the collection of event listeners associated with this event address.
        /// </summary>
        public IEnumerable<EventListenerV2> EventListenerV2s { get; set; }

        /// <summary>
        /// Gets or sets the collection of listener events associated with this event address.
        /// </summary>
        public IEnumerable<ListenerEventV2> ListenerEventV2s { get; set; }
    }
}
