// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Brokers.Times;
using EventHighway.Core.Models.Coordinations.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Orchestrations.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V1;
using EventHighway.Core.Services.Coordinations.ArchivingEvents.V2;
using EventHighway.Core.Services.Orchestrations.ArchivingEvents.V2;
using EventHighway.Core.Services.Orchestrations.EventArchives.V1;
using KellermanSoftware.CompareNetObjects;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.ArchivingEvents.V2
{
    public partial class ArchivingEventV2CoordinationServiceTests
    {
        private readonly Mock<IArchivingEvent2OrchestrationService> archivingEvent2OrchestrationServiceMock;
        private readonly Mock<IEventArchiveV1OrchestrationService> eventArchiveV1OrchestrationServiceMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly ICompareLogic compareLogic;
        private readonly IArchivingEventV2CoordinationService archivingEventV1CoordinationService;

        public ArchivingEventV2CoordinationServiceTests()
        {
            this.archivingEvent2OrchestrationServiceMock =
                new Mock<IArchivingEvent2OrchestrationService>(
                    behavior: MockBehavior.Strict);

            this.eventArchiveV1OrchestrationServiceMock =
                new Mock<IEventArchiveV1OrchestrationService>(
                    behavior: MockBehavior.Strict);

            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>(
                behavior: MockBehavior.Strict);

            this.loggingBrokerMock = new Mock<ILoggingBroker>();
            var compareConfiguration = new ComparisonConfig();
            this.compareLogic = new CompareLogic(compareConfiguration);

            this.archivingEventV1CoordinationService =
                new ArchivingEventV2CoordinationService(
                    archivingEvent2OrchestrationService:
                        this.archivingEvent2OrchestrationServiceMock.Object,
                    eventArchiveV1OrchestrationService:
                        this.eventArchiveV1OrchestrationServiceMock.Object,
                    dateTimeBroker: this.dateTimeBrokerMock.Object,
                    loggingBroker: this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> ArchivingEvent2ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();
            someInnerException.Data.Add("ErrorCode", new List<string> { "ValidationError" });

            return new TheoryData<Xeption>
            {
                new ArchivingEvent2OrchestrationValidationException(
                    someMessage,
                    someInnerException),

                new ArchivingEvent2OrchestrationDependencyValidationException(
                    someMessage,
                    someInnerException),
            };
        }

        public static TheoryData<Xeption> ArchivingEvent2DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();
            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyError" });

            return new TheoryData<Xeption>
            {
                new ArchivingEvent2OrchestrationDependencyException(
                    someMessage,
                    someInnerException),

                new ArchivingEvent2OrchestrationServiceException(
                    someMessage,
                    someInnerException),
            };
        }

        public static TheoryData<Xeption> EventArchiveV1ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();
            someInnerException.Data.Add("ErrorCode", new List<string> { "ValidationError" });

            return new TheoryData<Xeption>
            {
                new EventArchiveV1OrchestrationValidationException(
                    someMessage,
                    someInnerException),

                new EventArchiveV1OrchestrationDependencyValidationException(
                    someMessage,
                    someInnerException),
            };
        }

        public static TheoryData<Xeption> EventArchiveV1DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption();
            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyError" });

            return new TheoryData<Xeption>
            {
                new EventArchiveV1OrchestrationDependencyException(
                    someMessage,
                    someInnerException),

                new EventArchiveV1OrchestrationServiceException(
                    someMessage,
                    someInnerException),
            };
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static Guid GetRandomId() =>
            Guid.NewGuid();

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static T GetRandomEnum<T>()
        {
            int randomNumber =
                new IntRange(
                    min: 0,

                    max: Enum.GetValues(
                        enumType: typeof(T)).Length)
                            .GetValue();

            return (T)(object)randomNumber;
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private Expression<Func<EventV2, bool>> SameEventV2As(EventV2 expectedEventV2)
        {
            return actualEventV2 =>
                this.compareLogic.Compare(
                    expectedEventV2,
                    actualEventV2)
                        .AreEqual;
        }

        private Expression<Func<EventArchiveV1, bool>> SameEventArchiveV1As(
            EventArchiveV1 expectedEventArchiveV1)
        {
            return actualEventArchiveV1 =>
                this.compareLogic.Compare(
                    expectedEventArchiveV1,
                    actualEventArchiveV1)
                        .AreEqual;
        }

        private static List<dynamic> CreateRandomEventV2sProperties()
        {
            int randomCount = GetRandomNumber();

            return Enumerable.Range(start: 0, count: randomCount)
                .Select(item => CreateRandomEventV2Properties())
                    .ToList();
        }

        private static List<dynamic> CreateRandomListenerEventV2sProperties()
        {
            int randomCount = GetRandomNumber();

            return Enumerable.Range(start: 0, count: randomCount)
                .Select(item => CreateRandomListenerEventV2Properties())
                    .ToList();
        }

        private static dynamic CreateRandomEventV2Properties()
        {
            Guid randomId = GetRandomId();
            string randomContent = GetRandomString();
            string randomEventName = GetRandomString();

            EventTypeV2 randomType =
                GetRandomEnum<EventTypeV2>();

            DateTimeOffset randomCreatedDate =
                GetRandomDateTimeOffset();

            DateTimeOffset randomUpdatedDate =
                GetRandomDateTimeOffset();

            DateTimeOffset randomScheduledDate =
                GetRandomDateTimeOffset();

            Guid randomEventAddressId = GetRandomId();

            return new
            {
                Id = randomId,
                Content = randomContent,
                EventName = randomEventName,
                Type = randomType,
                CreatedDate = randomCreatedDate,
                UpdatedDate = randomUpdatedDate,
                ScheduledDate = randomScheduledDate,
                EventAddressId = randomEventAddressId
            };
        }

        private static dynamic CreateRandomListenerEventV2Properties()
        {
            Guid randomId = GetRandomId();

            ListenerEventStatusV2 randomStatus =
                GetRandomEnum<ListenerEventStatusV2>();

            string randomResponse = GetRandomString();
            string randomResponseCode = GetRandomString();
            string randomResponseMessage = GetRandomString();

            DateTimeOffset randomCreatedDate =
                GetRandomDateTimeOffset();

            DateTimeOffset randomUpdatedDate =
                GetRandomDateTimeOffset();

            Guid randomEventId = GetRandomId();
            Guid randomEventAddressId = GetRandomId();
            Guid randomEventListenerId = GetRandomId();

            return new
            {
                Id = randomId,
                Status = randomStatus,
                Response = randomResponse,
                ResponseCode = randomResponseCode,
                ResponseMessage = randomResponseMessage,
                CreatedDate = randomCreatedDate,
                UpdatedDate = randomUpdatedDate,
                EventId = randomEventId,
                EventAddressId = randomEventAddressId,
                EventListenerId = randomEventListenerId
            };
        }

        private static IQueryable<EventV2> CreateRandomEventV2s()
        {
            return CreateEventV2Filler()
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static Filler<EventV2> CreateEventV2Filler()
        {
            var filler = new Filler<EventV2>();

            filler.Setup()
                .OnType<DateTimeOffset>()
                    .Use(GetRandomDateTimeOffset)

                .OnType<DateTimeOffset?>()
                    .Use(GetRandomDateTimeOffset())

                .OnType<EventAddressV2>()
                    .IgnoreIt()

                .OnType<EventListenerV2>()
                    .IgnoreIt()

                .OnProperty(eventV2 =>
                    eventV2.EventAddressV2).IgnoreIt()

                .OnProperty(eventV2 =>
                    eventV2.ListenerEventV2s).IgnoreIt();

            return filler;
        }
    }
}
