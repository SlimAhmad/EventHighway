// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventHandler.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Processings.EventListeners.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.ListenerEvents.V2.Exceptions;
using EventHighway.Core.Services.Foundations.EventHandlers.V2;
using EventHighway.Core.Services.Orchestrations.EventListeners.V2;
using EventHighway.Core.Services.Processings.EventListeners.V2;
using EventHighway.Core.Services.Processings.ListenerEvents.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventListeners.V2
{
    public partial class EventListenerV2OrchestrationServiceTests
    {
        private readonly Mock<IEventListenerV2ProcessingService> eventListenerV2ProcessingServiceMock;
        private readonly Mock<IListenerEventV2ProcessingService> listenerEventV2ProcessingServiceMock;
        private readonly Mock<IEventHandlerV2Service> eventHandlerV2ServiceMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventListenerV2OrchestrationService eventListenerV2OrchestrationService;

        public EventListenerV2OrchestrationServiceTests()
        {
            this.eventListenerV2ProcessingServiceMock =
                new Mock<IEventListenerV2ProcessingService>();

            this.listenerEventV2ProcessingServiceMock =
                new Mock<IListenerEventV2ProcessingService>();

            this.eventHandlerV2ServiceMock =
                new Mock<IEventHandlerV2Service>();

            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.eventListenerV2OrchestrationService =
                new EventListenerV2OrchestrationService(
                    eventListenerV2ProcessingService: this.eventListenerV2ProcessingServiceMock.Object,
                    listenerEventV2ProcessingService: this.listenerEventV2ProcessingServiceMock.Object,
                    eventHandlerV2Service: this.eventHandlerV2ServiceMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();
            someInnerException.Data.Add("ErrorCode", new List<string> { "ValidationError" });

            return new TheoryData<Xeption>
            {
                new EventListenerV2ProcessingValidationException(
                    someMessage,
                    someInnerException),

                new EventListenerV2ProcessingDependencyValidationException(
                    someMessage,
                    someInnerException),

                new ListenerEventV2ProcessingValidationException(
                    someMessage,
                    someInnerException),

                new ListenerEventV2ProcessingDependencyValidationException(
                    someMessage,
                    someInnerException),
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();
            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyError" });

            return new TheoryData<Xeption>
            {
                new EventListenerV2ProcessingDependencyException(
                    someMessage,
                    someInnerException),

                new EventListenerV2ProcessingServiceException(
                    someMessage,
                    someInnerException),

                new ListenerEventV2ProcessingDependencyException(
                    someMessage,
                    someInnerException),

                new ListenerEventV2ProcessingServiceException(
                    someMessage,
                    someInnerException),

                new EventHandlerV2ServiceException(
                    someMessage,
                    someInnerException),
            };
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static IEnumerable<IEventHandler> CreateRandomEventHandlers()
        {
            int count = GetRandomNumber();
            var handlers = new List<IEventHandler>();

            for (int i = 0; i < count; i++)
            {
                var mock = new Mock<IEventHandler>();
                mock.SetupGet(h => h.Id).Returns(Guid.NewGuid());
                mock.SetupGet(h => h.Name).Returns(GetRandomString());
                handlers.Add(mock.Object);
            }

            return handlers;
        }

        private static IQueryable<ListenerEventV2> CreateRandomListenerEventV2s() =>
            CreateListenerEventV2Filler().Create(count: GetRandomNumber()).AsQueryable();

        private static ListenerEventV2 CreateRandomListenerEventV2() =>
            CreateListenerEventV2Filler().Create();

        private static IQueryable<EventListenerV2> CreateRandomEventListenerV2s() =>
            CreateEventListenerV2Filler().Create(count: GetRandomNumber()).AsQueryable();

        private static EventListenerV2 CreateRandomEventListenerV2() =>
            CreateEventListenerV2Filler().Create();

        private static Filler<ListenerEventV2> CreateListenerEventV2Filler()
        {
            var filler = new Filler<ListenerEventV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset)
                .OnType<EventParticipantV2>().IgnoreIt()

                .OnProperty(listenerEventV2 => listenerEventV2.Event)
                    .IgnoreIt()

                .OnProperty(listenerEventV2 => listenerEventV2.EventAddress)
                    .IgnoreIt()

                .OnProperty(listenerEventV2 => listenerEventV2.EventListener)
                    .IgnoreIt();

            return filler;
        }

        private static Filler<EventListenerV2> CreateEventListenerV2Filler()
        {
            var filler = new Filler<EventListenerV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset)

                .OnProperty(eventListenerV2 => eventListenerV2.EventAddressV2)
                    .IgnoreIt()

                .OnProperty(eventListenerV2 => eventListenerV2.ListenerEventV2s)
                    .IgnoreIt()

                .OnProperty(eventListenerV2 => eventListenerV2.ListenerEventArchiveV2s)
                    .IgnoreIt()

                .OnProperty(eventListenerV2 => eventListenerV2.Participant)
                    .IgnoreIt();

            return filler;
        }
    }
}
