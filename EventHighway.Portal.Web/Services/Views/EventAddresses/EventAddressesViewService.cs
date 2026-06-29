// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Portal.Web.Brokers.DateTimes;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Views.EventAddresses;

namespace EventHighway.Portal.Web.Services.Views.EventAddresses
{
    public partial class EventAddressesViewService : IEventAddressesViewService
    {
        private readonly IEventHighwayBroker eventHighwayBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public EventAddressesViewService(
            IEventHighwayBroker eventHighwayBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.eventHighwayBroker = eventHighwayBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<List<EventAddressView>> RetrieveAllAddressesAsync(
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public ValueTask<EventAddressView> RegisterAddressAsync(
            EventAddressView address,
            CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        private static EventAddressView AsView(EventAddressV2 address) =>
            new EventAddressView
            {
                Id = address.Id,
                Name = address.Name,
                Description = address.Description
            };
    }
}
