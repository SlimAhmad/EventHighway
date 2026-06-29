// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Portal.Web.Brokers.EventHighways;
using EventHighway.Portal.Web.Brokers.Loggings;
using EventHighway.Portal.Web.Models.Views.ListenerEvents;
using EventHighway.Portal.Web.Services.Views.ListenerEvents;
using Moq;
using Tynamix.ObjectFiller;

namespace EventHighway.Portal.Web.Tests.Unit.Services.Views.ListenerEvents
{
    public partial class ListenerEventsViewServiceTests
    {
        private readonly Mock<IEventHighwayBroker> eventHighwayBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IListenerEventsViewService listenerEventsViewService;

        public ListenerEventsViewServiceTests()
        {
            this.eventHighwayBrokerMock = new Mock<IEventHighwayBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.listenerEventsViewService = new ListenerEventsViewService(
                eventHighwayBroker: this.eventHighwayBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static ListenerEventV2 CreateRandomListenerEvent(DateTimeOffset createdDate) =>
            new ListenerEventV2
            {
                Id = Guid.NewGuid(),
                Status = ListenerEventStatusV2.Success,
                ResponseCode = GetRandomString(),
                ResponseMessage = GetRandomString(),
                EventId = Guid.NewGuid(),
                EventAddressId = Guid.NewGuid(),
                EventListenerId = Guid.NewGuid(),
                ParticipantId = Guid.NewGuid(),
                CreatedDate = createdDate,
                UpdatedDate = createdDate
            };

        private static ListenerEventView MapToView(ListenerEventV2 listenerEvent) =>
            new ListenerEventView
            {
                Id = listenerEvent.Id,
                Status = listenerEvent.Status.ToString(),
                ResponseCode = listenerEvent.ResponseCode,
                ResponseMessage = listenerEvent.ResponseMessage,
                EventId = listenerEvent.EventId,
                EventAddressId = listenerEvent.EventAddressId,
                EventListenerId = listenerEvent.EventListenerId,
                ParticipantId = listenerEvent.ParticipantId,
                CreatedDate = listenerEvent.CreatedDate
            };
    }
}
