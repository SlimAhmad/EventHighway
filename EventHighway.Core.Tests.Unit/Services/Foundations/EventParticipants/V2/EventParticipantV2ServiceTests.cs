// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Services.Foundations.EventParticipants.V2;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventParticipants.V2
{
    public partial class EventParticipantV2ServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventParticipantV2Service eventParticipantV2Service;

        public EventParticipantV2ServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.eventParticipantV2Service = new EventParticipantV2Service(
                storageBroker: this.storageBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static SqlException CreateSqlException() =>
            (SqlException)RuntimeHelpers.GetUninitializedObject(type: typeof(SqlException));

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static string GetRandomStringWithLengthOf(int length) =>
            new MnemonicString(wordCount: 1, wordMinLength: length, wordMaxLength: length).GetValue();

        private static int GetRandomNegativeNumber() =>
            -1 * GetRandomNumber();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static EventParticipantV2 CreateRandomEventParticipantV2(DateTimeOffset dates) =>
            CreateEventParticipantV2Filler(dates).Create();

        private static EventParticipantV2 CreateRandomEventParticipantV2() =>
            CreateEventParticipantV2Filler(dates: GetRandomDateTimeOffset()).Create();

        private static IQueryable<EventParticipantV2> CreateRandomEventParticipantV2s()
        {
            return CreateEventParticipantV2Filler(dates: GetRandomDateTimeOffset())
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static Filler<EventParticipantV2> CreateEventParticipantV2Filler(DateTimeOffset dates)
        {
            var filler = new Filler<EventParticipantV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates)

                .OnProperty(eventParticipantV2 => eventParticipantV2.IsActive)
                    .Use(true)

                .OnProperty(eventParticipantV2 => eventParticipantV2.ActiveFrom)
                    .IgnoreIt()

                .OnProperty(eventParticipantV2 => eventParticipantV2.ActiveTo)
                    .IgnoreIt()

                .OnProperty(eventParticipantV2 => eventParticipantV2.EventV2s)
                    .IgnoreIt()

                .OnProperty(eventParticipantV2 => eventParticipantV2.EventArchiveV2s)
                    .IgnoreIt()

                .OnProperty(eventParticipantV2 => eventParticipantV2.EventListenerV2s)
                    .IgnoreIt()

                .OnProperty(eventParticipantV2 => eventParticipantV2.ListenerEventV2s)
                    .IgnoreIt()

                .OnProperty(eventParticipantV2 => eventParticipantV2.ListenerEventArchiveV2s)
                    .IgnoreIt()

                .OnProperty(eventParticipantV2 => eventParticipantV2.EventParticipantSecretV2s)
                    .IgnoreIt();

            return filler;
        }
    }
}
