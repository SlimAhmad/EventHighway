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
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Services.Foundations.ListenerEvents.V2;
using KellermanSoftware.CompareNetObjects;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEvents.V2
{
    public partial class ListenerEventV2ServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IListenerEventV2Service listenerEventV2Service;

        public ListenerEventV2ServiceTests()
        {
            this.storageBrokerMock =
                new Mock<IStorageBroker>();

            this.loggingBrokerMock =
                new Mock<ILoggingBroker>();

            this.dateTimeBrokerMock =
                new Mock<IDateTimeBroker>();

            this.listenerEventV2Service = new ListenerEventV2Service(
                storageBroker: this.storageBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static SqlException GetSqlException()
        {
            return (SqlException)RuntimeHelpers
                .GetUninitializedObject(
                    type: typeof(SqlException));
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static bool SameListenerEventV2sAs(
            List<ListenerEventV2> expectedListenerEventV2s,
            List<ListenerEventV2> actualListenerEventV2s)
        {
            var compareLogic = new CompareLogic();

            ComparisonResult comparisonResult =
                compareLogic.Compare(expectedListenerEventV2s, actualListenerEventV2s);

            return comparisonResult.AreEqual;
        }

        private static List<ListenerEventV2> CreateRandomRestoreListenerEventV2s()
        {
            DateTimeOffset createdDate = GetRandomDateTimeOffset();

            return CreateListenerEventV2Filler(dates: createdDate)
                .Create(count: GetRandomNumber())
                    .Select(listenerEventV2 =>
                    {
                        listenerEventV2.CreatedDate = createdDate;
                        listenerEventV2.UpdatedDate = createdDate.AddMinutes(GetRandomNumber());

                        return listenerEventV2;
                    })
                        .ToList();
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


        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static int GetRandomNegativeNumber() =>
            -1 * GetRandomNumber();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static ListenerEventV2 CreateRandomListenerEventV2() =>
            CreateListenerEventV2Filler(dates: GetRandomDateTimeOffset()).Create();

        private static IQueryable<ListenerEventV2> CreateRandomListenerEventV2s()
        {
            return CreateListenerEventV2Filler(
                dates: GetRandomDateTimeOffset()).Create(
                    count: GetRandomNumber())
                        .AsQueryable();
        }

        private static ListenerEventV2 CreateRandomListenerEventV2(DateTimeOffset dates) =>
            CreateListenerEventV2Filler(dates).Create();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static Filler<ListenerEventV2> CreateListenerEventV2Filler(DateTimeOffset dates)
        {
            var filler = new Filler<ListenerEventV2>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dates)

                .OnProperty(listenerEventV2 => listenerEventV2.EventV2)
                    .IgnoreIt()

                .OnProperty(listenerEventV2 => listenerEventV2.EventAddressV2)
                    .IgnoreIt()

                .OnProperty(listenerEventV2 => listenerEventV2.EventListenerV2)
                    .IgnoreIt()

                .OnType<EventV2>().IgnoreIt()
                .OnType<EventAddressV2>().IgnoreIt()
                .OnType<EventListenerV2>().IgnoreIt()
                .OnType<EventParticipantV2>().IgnoreIt();

            return filler;
        }
    }
}
