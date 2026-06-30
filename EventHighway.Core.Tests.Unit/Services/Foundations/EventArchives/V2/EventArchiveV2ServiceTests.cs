// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Services.Foundations.EventArchives.V2;
using KellermanSoftware.CompareNetObjects;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventArchives.V2
{
    public partial class EventArchiveV2ServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IEventArchiveV2Service eventArchiveV2Service;

        public EventArchiveV2ServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.eventArchiveV2Service = new EventArchiveV2Service(
                storageBroker: this.storageBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static bool SameEventArchiveV2sAs(
            List<EventArchiveV2> expectedEventArchiveV2s,
            List<EventArchiveV2> actualEventArchiveV2s)
        {
            var compareLogic = new CompareLogic();

            ComparisonResult comparisonResult =
                compareLogic.Compare(expectedEventArchiveV2s, actualEventArchiveV2s);

            return comparisonResult.AreEqual;
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static SqlException CreateSqlException() =>
            (SqlException)RuntimeHelpers.GetUninitializedObject(type: typeof(SqlException));

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomNegativeNumber() =>
            -1 * GetRandomNumber();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static EventArchiveV2 CreateRandomEventArchiveV2(DateTimeOffset date) =>
            CreateEventArchiveV2Filler(date).Create();

        private static EventArchiveV2 CreateRandomEventArchiveV2() =>
            CreateEventArchiveV2Filler(date: GetRandomDateTimeOffset()).Create();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static IQueryable<EventArchiveV2> CreateRandomEventArchiveV2s()
        {
            return CreateEventArchiveV2Filler(date: GetRandomDateTimeOffset())
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static T GetInvalidEnum<T>()
        {
            int randomNumber =
                GetLocalRandomNumber();

            while (Enum.IsDefined(typeof(T), randomNumber) is true)
            {
                randomNumber = GetLocalRandomNumber();
            }

            return (T)(object)randomNumber;

            static int GetLocalRandomNumber()
            {
                return new IntRange(
                    min: int.MinValue,
                    max: int.MaxValue)
                        .GetValue();
            }
        }

        private static Filler<EventArchiveV2> CreateEventArchiveV2Filler(
            DateTimeOffset date)
        {
            var filler = new Filler<EventArchiveV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(date)

                .OnType<DateTimeOffset?>().Use(
                    GetRandomDateTimeOffset())

                .OnProperty(eventArchiveV2 => eventArchiveV2.EventAddressV2)
                    .IgnoreIt()

                .OnProperty(eventArchiveV2 => eventArchiveV2.ListenerEventArchiveV2s)
                    .IgnoreIt()

                .OnProperty(eventArchiveV2 => eventArchiveV2.EventAddressV2)
                    .IgnoreIt()

                .OnType<EventParticipantV2>().IgnoreIt();

            return filler;
        }
    }
}
