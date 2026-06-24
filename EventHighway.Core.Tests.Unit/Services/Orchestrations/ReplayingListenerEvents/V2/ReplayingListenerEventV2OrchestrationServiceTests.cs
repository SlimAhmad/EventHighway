// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Processings.EventCalls.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.ListenerEvents.V2.Exceptions;
using EventHighway.Core.Services.Orchestrations.ReplayingListenerEvents.V2;
using EventHighway.Core.Services.Processings.EventCalls.V2;
using EventHighway.Core.Services.Processings.ListenerEvents.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ReplayingListenerEvents.V2
{
    public partial class ReplayingListenerEventV2OrchestrationServiceTests
    {
        private readonly Mock<IEventCallV2ProcessingService> eventCallV2ProcessingServiceMock;
        private readonly Mock<IListenerEventV2ProcessingService> listenerEventV2ProcessingServiceMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IReplayingListenerEventV2OrchestrationService replayingListenerEventV2OrchestrationService;

        public ReplayingListenerEventV2OrchestrationServiceTests()
        {
            this.eventCallV2ProcessingServiceMock = new Mock<IEventCallV2ProcessingService>();
            this.listenerEventV2ProcessingServiceMock = new Mock<IListenerEventV2ProcessingService>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.replayingListenerEventV2OrchestrationService =
                new ReplayingListenerEventV2OrchestrationService(
                    eventCallV2ProcessingService: this.eventCallV2ProcessingServiceMock.Object,
                    listenerEventV2ProcessingService: this.listenerEventV2ProcessingServiceMock.Object,
                    dateTimeBroker: this.dateTimeBrokerMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);

            return new TheoryData<Xeption>
            {
                new EventCallV2ProcessingValidationException(someMessage, someInnerException),
                new EventCallV2ProcessingDependencyValidationException(someMessage, someInnerException),
                new ListenerEventV2ProcessingValidationException(someMessage, someInnerException),
                new ListenerEventV2ProcessingDependencyValidationException(someMessage, someInnerException),
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);

            return new TheoryData<Xeption>
            {
                new EventCallV2ProcessingDependencyException(someMessage, someInnerException),
                new EventCallV2ProcessingServiceException(someMessage, someInnerException),
                new ListenerEventV2ProcessingDependencyException(someMessage, someInnerException),
                new ListenerEventV2ProcessingServiceException(someMessage, someInnerException),
            };
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static ListenerEventV2 CreateRandomListenerEventV2WithNavProps()
        {
            ListenerEventV2 listenerEventV2 = CreateListenerEventV2Filler().Create();
            listenerEventV2.Event = CreateEventV2Filler().Create();
            listenerEventV2.EventListener = CreateEventListenerV2Filler().Create();

            return listenerEventV2;
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static Filler<ListenerEventV2> CreateListenerEventV2Filler()
        {
            var filler = new Filler<ListenerEventV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset)
                .OnProperty(lev => lev.Event).IgnoreIt()
                .OnProperty(lev => lev.EventAddress).IgnoreIt()
                .OnProperty(lev => lev.EventListener).IgnoreIt();

            return filler;
        }

        private static Filler<EventV2> CreateEventV2Filler()
        {
            var filler = new Filler<EventV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset)
                .OnType<DateTimeOffset?>().Use(GetRandomDateTimeOffset());

            return filler;
        }

        private static Filler<EventListenerV2> CreateEventListenerV2Filler()
        {
            var filler = new Filler<EventListenerV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset)
                .OnType<DateTimeOffset?>().Use(GetRandomDateTimeOffset());

            return filler;
        }
    }
}
