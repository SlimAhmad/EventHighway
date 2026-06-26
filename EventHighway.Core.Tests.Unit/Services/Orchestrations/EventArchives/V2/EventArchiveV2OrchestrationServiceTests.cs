// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V2;
using EventHighway.Core.Models.Services.Processings.EventArchives.V2.Exceptions;
using EventHighway.Core.Models.Services.Processings.ListenerEventArchives.V2.Exceptions;
using EventHighway.Core.Services.Orchestrations.EventArchives.V2;
using EventHighway.Core.Services.Processings.EventArchives.V2;
using EventHighway.Core.Services.Processings.ListenerEventArchives.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V2
{
    public partial class EventArchiveV2OrchestrationServiceTests
    {
        private readonly Mock<IListenerEventArchiveV2ProcessingService> listenerEventArchiveV2ProcessingServiceMock;
        private readonly Mock<IEventArchiveV2ProcessingService> eventArchiveV2ProcessingServiceMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventArchiveV2OrchestrationService eventArchiveV2OrchestrationService;

        public EventArchiveV2OrchestrationServiceTests()
        {
            this.listenerEventArchiveV2ProcessingServiceMock =
                new Mock<IListenerEventArchiveV2ProcessingService>();

            this.eventArchiveV2ProcessingServiceMock =
                new Mock<IEventArchiveV2ProcessingService>();

            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.eventArchiveV2OrchestrationService = new EventArchiveV2OrchestrationService(
                listenerEventArchiveV2ProcessingService:
                    this.listenerEventArchiveV2ProcessingServiceMock.Object,
                eventArchiveV2ProcessingService: this.eventArchiveV2ProcessingServiceMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();
            someInnerException.Data.Add("ErrorCode", new List<string> { "ValidationError" });

            return new TheoryData<Xeption>
            {
                new EventArchiveV2ProcessingValidationException(
                    someMessage,
                    someInnerException),

                new EventArchiveV2ProcessingDependencyValidationException(
                    someMessage,
                    someInnerException),

                new ListenerEventArchiveV2ProcessingValidationException(
                    someMessage,
                    someInnerException),

                new ListenerEventArchiveV2ProcessingDependencyValidationException(
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
                new EventArchiveV2ProcessingDependencyException(
                    someMessage,
                    someInnerException),

                new EventArchiveV2ProcessingServiceException(
                    someMessage,
                    someInnerException),

                new ListenerEventArchiveV2ProcessingDependencyException(
                    someMessage,
                    someInnerException),

                new ListenerEventArchiveV2ProcessingServiceException(
                    someMessage,
                    someInnerException),
            };
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static IQueryable<EventArchiveV2> CreateRandomEventArchiveV2s() =>
            CreateEventArchiveV2Filler().Create(count: GetRandomNumber()).AsQueryable();

        private static EventArchiveV2 CreateRandomEventArchiveV2() =>
            CreateEventArchiveV2Filler().Create();

        private static IQueryable<ListenerEventArchiveV2> CreateRandomListenerEventArchiveV2s() =>
            CreateListenerEventArchiveV2Filler().Create(count: GetRandomNumber()).AsQueryable();

        private static Filler<EventArchiveV2> CreateEventArchiveV2Filler()
        {
            var filler = new Filler<EventArchiveV2>();

            filler.Setup()
                .OnType<DateTimeOffset>()
                    .Use(GetRandomDateTimeOffset)

                .OnType<DateTimeOffset?>()
                    .Use(GetRandomDateTimeOffset())

                .OnType<EventParticipantV2>().IgnoreIt();

            return filler;
        }

        private static Filler<ListenerEventArchiveV2> CreateListenerEventArchiveV2Filler()
        {
            var filler = new Filler<ListenerEventArchiveV2>();

            filler.Setup()
                .OnType<DateTimeOffset>()
                    .Use(GetRandomDateTimeOffset)

                .OnType<DateTimeOffset?>()
                    .Use(GetRandomDateTimeOffset());

            return filler;
        }
    }
}
