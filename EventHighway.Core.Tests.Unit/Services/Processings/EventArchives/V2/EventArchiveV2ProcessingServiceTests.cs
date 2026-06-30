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
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2.Exceptions;
using EventHighway.Core.Services.Foundations.EventArchives.V2;
using EventHighway.Core.Services.Processings.EventArchives.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventArchives.V2
{
    public partial class EventArchiveV2ProcessingServiceTests
    {
        private readonly Mock<IEventArchiveV2Service> eventArchiveV2ServiceMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventArchiveV2ProcessingService eventArchiveV2ProcessingService;

        public EventArchiveV2ProcessingServiceTests()
        {
            this.eventArchiveV2ServiceMock = new Mock<IEventArchiveV2Service>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.eventArchiveV2ProcessingService =
                new EventArchiveV2ProcessingService(
                    eventArchiveV2Service: this.eventArchiveV2ServiceMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> DependencyValidationExceptions()
        {
            string someMessage = GetRandomString();

            var someInnerException = new Xeption();

            someInnerException.Data.Add("ErrorCode", new List<string> { "ValidationError" });

            return new TheoryData<Xeption>
            {
                new EventArchiveV2ValidationException(
                    someMessage,
                    someInnerException),

                new EventArchiveV2DependencyValidationException(
                    someMessage,
                    someInnerException)
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            string someMessage = GetRandomString();

            var someInnerException = new Xeption();

            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyError" });

            return new TheoryData<Xeption>
            {
                new EventArchiveV2DependencyException(
                    someMessage,
                    someInnerException),

                new EventArchiveV2ServiceException(
                    someMessage,
                    someInnerException)
            };
        }

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static IQueryable<EventArchiveV2> CreateRandomEventArchiveV2s() =>
            CreateEventArchiveV2Filler().Create(count: GetRandomNumber()).AsQueryable();

        private static EventArchiveV2 CreateRandomEventArchiveV2() =>
            CreateEventArchiveV2Filler().Create();

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static Filler<EventArchiveV2> CreateEventArchiveV2Filler()
        {
            var filler = new Filler<EventArchiveV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(GetRandomDateTimeOffset)
                .OnType<DateTimeOffset?>().Use(GetRandomDateTimeOffset())

                .OnProperty(eventArchiveV2 => eventArchiveV2.ListenerEventArchiveV2s)
                    .IgnoreIt()

                .OnProperty(eventArchiveV2 => eventArchiveV2.EventAddressV2)
                    .IgnoreIt()

                .OnType<EventParticipantV2>().IgnoreIt();

            return filler;
        }
    }
}
