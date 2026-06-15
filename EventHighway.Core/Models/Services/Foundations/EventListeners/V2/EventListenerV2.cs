// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.HandlerConfigurations;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;

namespace EventHighway.Core.Models.Services.Foundations.EventListeners.V2
{
    /// <summary>
    /// Represents an event listener in the V2 service model, defining how events are processed
    /// by handlers with specific configurations and filter criteria.
    /// </summary>
    public class EventListenerV2
    {
        /// <summary>
        /// Gets or sets the unique identifier for this event listener.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of this event listener.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of this event listener.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the event handler associated with this
        /// listener.
        /// </summary>
        public Guid HandlerId { get; set; }

        /// <summary>
        /// Gets or sets the name of the event handler associated with this listener.
        /// </summary>
        public string HandlerName { get; set; }

        /// <summary>
        /// Gets or sets the collection of handler configurations that define how the handler
        /// processes events.
        /// </summary>
        public IEnumerable<HandlerConfiguration> HandlerConfigurations { get; set; }

        /// <summary>
        /// Gets or sets the promoted properties as a serialized string, which are properties
        /// selected for exposure from the event handler.
        /// </summary>
        public string PromotedProperties { get; set; }

        /// <summary>
        /// Gets or sets the filter criteria as a serialized string, which determines which
        /// events this listener will process.
        /// </summary>
        public string FilterCriteria { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this event listener was created.
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this event listener was last updated.
        /// </summary>
        public DateTimeOffset UpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the event address associated with this
        /// listener.
        /// </summary>
        public Guid EventAddressId { get; set; }

        /// <summary>
        /// Gets or sets the event address object that this listener is associated with.
        /// </summary>
        public EventAddressV2 EventAddressV2 { get; set; }

        /// <summary>
        /// Gets or sets the collection of listener events associated with this event
        /// listener.
        /// </summary>
        public IEnumerable<ListenerEventV2> ListenerEventV2s { get; set; }
    }
}
