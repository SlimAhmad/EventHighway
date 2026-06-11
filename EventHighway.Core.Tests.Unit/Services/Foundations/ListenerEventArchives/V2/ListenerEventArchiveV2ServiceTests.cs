// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Storages;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Services.Foundations.ListenerEventArchives.V2;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEventArchives.V2
{
    public partial class ListenerEventArchiveV2ServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IListenerEventArchiveV2Service listenerEventArchiveV2Service;

        public ListenerEventArchiveV2ServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.listenerEventArchiveV2Service = new ListenerEventArchiveV2Service(
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

        private static int GetRandomNegativeNumber() =>
            -1 * GetRandomNumber();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static T GetInvalidEnum<T>()
        {
            int randomNumber = GetLocalRandomNumber();

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

        private static ListenerEventArchiveV2 CreateRandomListenerEventArchiveV2(DateTimeOffset date) =>
            CreateListenerEventArchiveV2Filler(date).Create();

        private static ListenerEventArchiveV2 CreateRandomListenerEventArchiveV2() =>
            CreateListenerEventArchiveV2Filler(date: GetRandomDateTimeOffset()).Create();

        private static Filler<ListenerEventArchiveV2> CreateListenerEventArchiveV2Filler(DateTimeOffset date)
        {
            var filler = new Filler<ListenerEventArchiveV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(date);

            return filler;
        }
    }
}
