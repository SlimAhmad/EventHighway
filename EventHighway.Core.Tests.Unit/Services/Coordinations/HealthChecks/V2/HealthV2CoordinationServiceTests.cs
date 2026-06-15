// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Configurations.Healths;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V2.Exceptions;
using EventHighway.Core.Models.Services.Orchestrations.Events.V2.Exceptions;
using EventHighway.Core.Services.Coordinations.HealthChecks.V2;
using EventHighway.Core.Services.Orchestrations.EventArchives.V2;
using EventHighway.Core.Services.Orchestrations.EventListeners.V2;
using EventHighway.Core.Services.Orchestrations.Events.V2;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.HealthChecks.V2
{
    public partial class HealthV2CoordinationServiceTests
    {
        private readonly Mock<IEventV2OrchestrationService> eventV2OrchestrationServiceMock;
        private readonly Mock<IEventListenerV2OrchestrationService> eventListenerV2OrchestrationServiceMock;
        private readonly Mock<IEventArchiveV2OrchestrationService> eventArchiveV2OrchestrationServiceMock;
        private readonly Mock<IConfigurationBroker> configurationBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IHealthV2CoordinationService healthV2CoordinationService;

        public HealthV2CoordinationServiceTests()
        {
            this.eventV2OrchestrationServiceMock =
                new Mock<IEventV2OrchestrationService>();

            this.eventListenerV2OrchestrationServiceMock =
                new Mock<IEventListenerV2OrchestrationService>();

            this.eventArchiveV2OrchestrationServiceMock =
                new Mock<IEventArchiveV2OrchestrationService>();

            this.configurationBrokerMock = new Mock<IConfigurationBroker>();

            this.configurationBrokerMock
                .Setup(broker => broker.GetHealthConfiguration())
                .Returns(new HealthConfiguration());

            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.healthV2CoordinationService =
                new HealthV2CoordinationService(
                    eventV2OrchestrationService:
                        this.eventV2OrchestrationServiceMock.Object,
                    eventListenerV2OrchestrationService:
                        this.eventListenerV2OrchestrationServiceMock.Object,
                    eventArchiveV2OrchestrationService:
                        this.eventArchiveV2OrchestrationServiceMock.Object,
                    configurationBroker:
                        this.configurationBrokerMock.Object,
                    loggingBroker:
                        this.loggingBrokerMock.Object);
        }

        public static TheoryData<Xeption> ValidationExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.Data.Add("ErrorCode", new List<string> { "ValidationError" });

            return new TheoryData<Xeption>
            {
                new EventV2OrchestrationValidationException(
                    someMessage,
                    someInnerException),

                new EventV2OrchestrationDependencyValidationException(
                    someMessage,
                    someInnerException),

                new EventListenerV2OrchestrationValidationException(
                    someMessage,
                    someInnerException),

                new EventListenerV2OrchestrationDependencyValidationException(
                    someMessage,
                    someInnerException),

                new EventArchiveV2OrchestrationValidationException(
                    someMessage,
                    someInnerException),

                new EventArchiveV2OrchestrationDependencyValidationException(
                    someMessage,
                    someInnerException),
            };
        }

        public static TheoryData<Xeption> DependencyExceptions()
        {
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyError" });

            return new TheoryData<Xeption>
            {
                new EventV2OrchestrationDependencyException(
                    someMessage,
                    someInnerException),

                new EventV2OrchestrationServiceException(
                    someMessage,
                    someInnerException),

                new EventListenerV2OrchestrationDependencyException(
                    someMessage,
                    someInnerException),

                new EventListenerV2OrchestrationServiceException(
                    someMessage,
                    someInnerException),

                new EventArchiveV2OrchestrationDependencyException(
                    someMessage,
                    someInnerException),

                new EventArchiveV2OrchestrationServiceException(
                    someMessage,
                    someInnerException),
            };
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException) =>
            actualException => actualException.SameExceptionAs(expectedException);

        private static string GetRandomString() =>
            new MnemonicString().GetValue();

        private static int GetRandomPositiveNumber() =>
            new IntRange(min: 2, max: 9).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: DateTime.UnixEpoch).GetValue();

        private static IQueryable<EventAddressV2> CreateRandomEventAddressV2s(int count = 0)
        {
            int actualCount = count > 0 ? count : GetRandomPositiveNumber();

            return CreateEventAddressV2Filler()
                .Create(count: actualCount)
                    .AsQueryable();
        }

        private static IQueryable<EventListenerV2> CreateRandomEventListenerV2s(int count = 0)
        {
            int actualCount = count > 0 ? count : GetRandomPositiveNumber();

            return Enumerable.Range(0, actualCount)
                .Select(_ => new EventListenerV2
                {
                    Id = Guid.NewGuid(),
                    Name = GetRandomString(),
                    Description = GetRandomString(),
                    CreatedDate = GetRandomDateTimeOffset(),
                    UpdatedDate = GetRandomDateTimeOffset(),
                    EventAddressId = Guid.NewGuid()
                })
                .AsQueryable();
        }

        private static IQueryable<EventV2> CreateRandomEventV2s(
            int immediateCount = 0,
            int scheduledCount = 0,
            int deadCount = 0)
        {
            var events = new List<EventV2>();

            int immCount = immediateCount > 0 ? immediateCount : GetRandomPositiveNumber();
            int schCount = scheduledCount > 0 ? scheduledCount : GetRandomPositiveNumber();

            for (int i = 0; i < immCount; i++)
            {
                events.Add(new EventV2
                {
                    Id = Guid.NewGuid(),
                    Type = EventTypeV2.Immediate,
                    RemainingRetryAttempts = GetRandomPositiveNumber(),
                    Content = GetRandomString(),
                    EventName = GetRandomString(),
                    CreatedDate = GetRandomDateTimeOffset(),
                    UpdatedDate = GetRandomDateTimeOffset(),
                    EventAddressId = Guid.NewGuid()
                });
            }

            for (int i = 0; i < schCount; i++)
            {
                events.Add(new EventV2
                {
                    Id = Guid.NewGuid(),
                    Type = EventTypeV2.Scheduled,
                    RemainingRetryAttempts = GetRandomPositiveNumber(),
                    Content = GetRandomString(),
                    EventName = GetRandomString(),
                    CreatedDate = GetRandomDateTimeOffset(),
                    UpdatedDate = GetRandomDateTimeOffset(),
                    EventAddressId = Guid.NewGuid()
                });
            }

            for (int i = 0; i < deadCount; i++)
            {
                events.Add(new EventV2
                {
                    Id = Guid.NewGuid(),
                    Type = EventTypeV2.Immediate,
                    RemainingRetryAttempts = 0,
                    Content = GetRandomString(),
                    EventName = GetRandomString(),
                    CreatedDate = GetRandomDateTimeOffset(),
                    UpdatedDate = GetRandomDateTimeOffset(),
                    EventAddressId = Guid.NewGuid()
                });
            }

            return events.AsQueryable();
        }

        private static IQueryable<ListenerEventV2> CreateRandomListenerEventV2s(
            int successCount = 0,
            int pendingCount = 0,
            int errorCount = 0)
        {
            var listenerEvents = new List<ListenerEventV2>();

            int sucCount = successCount > 0 ? successCount : GetRandomPositiveNumber();
            int penCount = pendingCount;

            for (int i = 0; i < sucCount; i++)
            {
                listenerEvents.Add(new ListenerEventV2
                {
                    Id = Guid.NewGuid(),
                    Status = ListenerEventStatusV2.Success,
                    CreatedDate = GetRandomDateTimeOffset(),
                    UpdatedDate = GetRandomDateTimeOffset(),
                    EventId = Guid.NewGuid(),
                    EventAddressId = Guid.NewGuid(),
                    EventListenerId = Guid.NewGuid()
                });
            }

            for (int i = 0; i < penCount; i++)
            {
                listenerEvents.Add(new ListenerEventV2
                {
                    Id = Guid.NewGuid(),
                    Status = ListenerEventStatusV2.Pending,
                    CreatedDate = GetRandomDateTimeOffset(),
                    UpdatedDate = GetRandomDateTimeOffset(),
                    EventId = Guid.NewGuid(),
                    EventAddressId = Guid.NewGuid(),
                    EventListenerId = Guid.NewGuid()
                });
            }

            for (int i = 0; i < errorCount; i++)
            {
                listenerEvents.Add(new ListenerEventV2
                {
                    Id = Guid.NewGuid(),
                    Status = ListenerEventStatusV2.Error,
                    CreatedDate = GetRandomDateTimeOffset(),
                    UpdatedDate = GetRandomDateTimeOffset(),
                    EventId = Guid.NewGuid(),
                    EventAddressId = Guid.NewGuid(),
                    EventListenerId = Guid.NewGuid()
                });
            }

            return listenerEvents.AsQueryable();
        }

        private static IEnumerable<IEventHandler> CreateRandomEventHandlers(int count = -1)
        {
            int actualCount = count >= 0 ? count : GetRandomPositiveNumber();

            return Enumerable.Range(0, actualCount)
                .Select(_ => new Mock<IEventHandler>().Object)
                    .ToList();
        }

        private static IQueryable<EventArchiveV2> CreateRandomEventArchiveV2s(int count = 0)
        {
            int actualCount = count > 0 ? count : GetRandomPositiveNumber();

            return CreateEventArchiveV2Filler()
                .Create(count: actualCount)
                    .AsQueryable();
        }

        private static IQueryable<ListenerEventArchiveV2> CreateRandomListenerEventArchiveV2s(
            int successCount = 0,
            int errorCount = 0)
        {
            var archives = new List<ListenerEventArchiveV2>();

            int sucCount = successCount > 0 ? successCount : GetRandomPositiveNumber();

            for (int i = 0; i < sucCount; i++)
            {
                archives.Add(new ListenerEventArchiveV2
                {
                    Id = Guid.NewGuid(),
                    Status = ListenerEventArchiveStatusV2.Success,
                    CreatedDate = GetRandomDateTimeOffset(),
                    UpdatedDate = GetRandomDateTimeOffset(),
                    ArchivedDate = GetRandomDateTimeOffset(),
                    EventId = Guid.NewGuid(),
                    EventAddressId = Guid.NewGuid(),
                    EventListenerId = Guid.NewGuid()
                });
            }

            for (int i = 0; i < errorCount; i++)
            {
                archives.Add(new ListenerEventArchiveV2
                {
                    Id = Guid.NewGuid(),
                    Status = ListenerEventArchiveStatusV2.Error,
                    CreatedDate = GetRandomDateTimeOffset(),
                    UpdatedDate = GetRandomDateTimeOffset(),
                    ArchivedDate = GetRandomDateTimeOffset(),
                    EventId = Guid.NewGuid(),
                    EventAddressId = Guid.NewGuid(),
                    EventListenerId = Guid.NewGuid()
                });
            }

            return archives.AsQueryable();
        }

        private static Filler<EventAddressV2> CreateEventAddressV2Filler()
        {
            var filler = new Filler<EventAddressV2>();

            filler.Setup()
                .OnType<DateTimeOffset>()
                    .Use(GetRandomDateTimeOffset)

                .OnProperty(e => e.Events).IgnoreIt()
                .OnProperty(e => e.EventListenerV2s).IgnoreIt()
                .OnProperty(e => e.ListenerEventV2s).IgnoreIt();

            return filler;
        }

        private static Filler<EventArchiveV2> CreateEventArchiveV2Filler()
        {
            var filler = new Filler<EventArchiveV2>();

            filler.Setup()
                .OnType<DateTimeOffset>()
                    .Use(GetRandomDateTimeOffset)

                .OnType<DateTimeOffset?>()
                    .Use(GetRandomDateTimeOffset())

                .OnProperty(e => e.ListenerEventArchiveV2s).IgnoreIt();

            return filler;
        }
    }
}

