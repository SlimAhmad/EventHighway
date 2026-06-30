// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.Core.Brokers.Configurations;
using EventHighway.Core.Brokers.Loggings;
using EventHighway.Core.Models.Configurations.Healths;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
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
            int deadCount = 0,
            int quarantinedCount = 0)
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
                    Status = EventStatusV2.Active,
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
                    Status = EventStatusV2.Active,
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
                    Status = EventStatusV2.Active,
                    RemainingRetryAttempts = 0,
                    Content = GetRandomString(),
                    EventName = GetRandomString(),
                    CreatedDate = GetRandomDateTimeOffset(),
                    UpdatedDate = GetRandomDateTimeOffset(),
                    EventAddressId = Guid.NewGuid()
                });
            }

            for (int i = 0; i < quarantinedCount; i++)
            {
                events.Add(new EventV2
                {
                    Id = Guid.NewGuid(),
                    Type = EventTypeV2.Immediate,
                    Status = EventStatusV2.Quarantined,
                    RemainingRetryAttempts = GetRandomPositiveNumber(),
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
            int errorCount = 0,
            int replayCount = 0)
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

            for (int i = 0; i < replayCount; i++)
            {
                listenerEvents.Add(new ListenerEventV2
                {
                    Id = Guid.NewGuid(),
                    Status = ListenerEventStatusV2.Replay,
                    CreatedDate = GetRandomDateTimeOffset(),
                    UpdatedDate = GetRandomDateTimeOffset(),
                    EventId = Guid.NewGuid(),
                    EventAddressId = Guid.NewGuid(),
                    EventListenerId = Guid.NewGuid()
                });
            }

            return listenerEvents.AsQueryable();
        }

        private static EventParticipantV2 CreateEventParticipantV2(
            Guid id,
            string name,
            string contactEmail,
            string contactPhone,
            bool isActive)
        {
            return new EventParticipantV2
            {
                Id = id,
                Name = name,
                Description = GetRandomString(),
                ContactEmail = contactEmail,
                ContactPhone = contactPhone,
                IsActive = isActive,
                CreatedDate = GetRandomDateTimeOffset(),
                UpdatedDate = GetRandomDateTimeOffset()
            };
        }

        private static EventV2 CreateEventV2ForParticipant(
            Guid id,
            Guid eventAddressId,
            Guid? participantId,
            EventParticipantV2 participant,
            DateTimeOffset createdDate,
            string contentHash,
            EventStatusV2 status)
        {
            return new EventV2
            {
                Id = id,
                Type = EventTypeV2.Immediate,
                Status = status,
                RemainingRetryAttempts = GetRandomPositiveNumber(),
                ContentHash = contentHash,
                Content = GetRandomString(),
                EventName = GetRandomString(),
                CreatedDate = createdDate,
                UpdatedDate = createdDate,
                EventAddressId = eventAddressId,
                ParticipantId = participantId,
                Participant = participant
            };
        }

        private static EventListenerV2 CreateEventListenerV2ForParticipant(
            Guid id,
            Guid eventAddressId,
            Guid? participantId,
            EventParticipantV2 participant)
        {
            return new EventListenerV2
            {
                Id = id,
                Name = GetRandomString(),
                Description = GetRandomString(),
                CreatedDate = GetRandomDateTimeOffset(),
                UpdatedDate = GetRandomDateTimeOffset(),
                EventAddressId = eventAddressId,
                ParticipantId = participantId,
                Participant = participant
            };
        }

        private static ListenerEventV2 CreateListenerEventV2ForEventAndListener(
            Guid eventId,
            Guid eventListenerId,
            Guid eventAddressId,
            DateTimeOffset createdDate,
            ListenerEventStatusV2 status)
        {
            return new ListenerEventV2
            {
                Id = Guid.NewGuid(),
                Status = status,
                CreatedDate = createdDate,
                UpdatedDate = createdDate,
                EventId = eventId,
                EventAddressId = eventAddressId,
                EventListenerId = eventListenerId
            };
        }

        private static EventAddressV2 CreateEventAddressV2(
            Guid id,
            string name,
            string description)
        {
            return new EventAddressV2
            {
                Id = id,
                Name = name,
                Description = description,
                CreatedDate = GetRandomDateTimeOffset(),
                UpdatedDate = GetRandomDateTimeOffset()
            };
        }

        private static EventV2 CreateEventV2ForAddress(
            Guid eventAddressId,
            DateTimeOffset createdDate,
            string contentHash,
            int remainingRetryAttempts,
            EventStatusV2 status)
        {
            return new EventV2
            {
                Id = Guid.NewGuid(),
                Type = EventTypeV2.Immediate,
                Status = status,
                RemainingRetryAttempts = remainingRetryAttempts,
                ContentHash = contentHash,
                Content = GetRandomString(),
                EventName = GetRandomString(),
                CreatedDate = createdDate,
                UpdatedDate = createdDate,
                EventAddressId = eventAddressId
            };
        }

        private static ListenerEventV2 CreateListenerEventV2ForAddress(
            Guid eventAddressId,
            DateTimeOffset createdDate,
            ListenerEventStatusV2 status)
        {
            return new ListenerEventV2
            {
                Id = Guid.NewGuid(),
                Status = status,
                CreatedDate = createdDate,
                UpdatedDate = createdDate,
                EventId = Guid.NewGuid(),
                EventAddressId = eventAddressId,
                EventListenerId = Guid.NewGuid()
            };
        }

        private static EventListenerV2 CreateEventListenerV2ForAddress(Guid eventAddressId)
        {
            return new EventListenerV2
            {
                Id = Guid.NewGuid(),
                Name = GetRandomString(),
                Description = GetRandomString(),
                CreatedDate = GetRandomDateTimeOffset(),
                UpdatedDate = GetRandomDateTimeOffset(),
                EventAddressId = eventAddressId
            };
        }

        private static EventArchiveV2 CreateEventArchiveV2ForAddress(
            Guid eventAddressId,
            DateTimeOffset archivedDate)
        {
            return new EventArchiveV2
            {
                Id = Guid.NewGuid(),
                Status = EventArchiveStatusV2.Active,
                RemainingRetryAttempts = GetRandomPositiveNumber(),
                Content = GetRandomString(),
                EventName = GetRandomString(),
                CreatedDate = archivedDate,
                UpdatedDate = archivedDate,
                ArchivedDate = archivedDate,
                EventAddressId = eventAddressId
            };
        }

        private static EventArchiveV2 CreateEventArchiveV2ForAddress(
            Guid eventAddressId,
            DateTimeOffset archivedDate,
            EventArchiveStatusV2 status)
        {
            return new EventArchiveV2
            {
                Id = Guid.NewGuid(),
                Status = status,
                RemainingRetryAttempts = GetRandomPositiveNumber(),
                Content = GetRandomString(),
                EventName = GetRandomString(),
                CreatedDate = archivedDate,
                UpdatedDate = archivedDate,
                ArchivedDate = archivedDate,
                EventAddressId = eventAddressId
            };
        }

        private static ListenerEventArchiveV2 CreateListenerEventArchiveV2ForAddress(
            Guid eventAddressId,
            DateTimeOffset archivedDate)
        {
            return new ListenerEventArchiveV2
            {
                Id = Guid.NewGuid(),
                Status = ListenerEventArchiveStatusV2.Success,
                CreatedDate = archivedDate,
                UpdatedDate = archivedDate,
                ArchivedDate = archivedDate,
                EventId = Guid.NewGuid(),
                EventAddressId = eventAddressId,
                EventListenerId = Guid.NewGuid()
            };
        }

        private static EventV2 CreateEventV2WithDate(
            DateTimeOffset createdDate,
            EventTypeV2 type)
        {
            return new EventV2
            {
                Id = Guid.NewGuid(),
                Type = type,
                Status = EventStatusV2.Active,
                RemainingRetryAttempts = GetRandomPositiveNumber(),
                Content = GetRandomString(),
                EventName = GetRandomString(),
                CreatedDate = createdDate,
                UpdatedDate = createdDate,
                EventAddressId = Guid.NewGuid()
            };
        }

        private static ListenerEventV2 CreateListenerEventV2WithDate(
            DateTimeOffset createdDate,
            ListenerEventStatusV2 status)
        {
            return new ListenerEventV2
            {
                Id = Guid.NewGuid(),
                Status = status,
                CreatedDate = createdDate,
                UpdatedDate = createdDate,
                EventId = Guid.NewGuid(),
                EventAddressId = Guid.NewGuid(),
                EventListenerId = Guid.NewGuid()
            };
        }

        private static IEnumerable<IEventHandler> CreateRandomEventHandlers(int count = -1)
        {
            int actualCount = count >= 0 ? count : GetRandomPositiveNumber();

            return Enumerable.Range(0, actualCount)
                .Select(_ => new Mock<IEventHandler>().Object)
                    .ToList();
        }

        private static IQueryable<EventArchiveV2> CreateRandomEventArchiveV2s(
            int count = 0,
            int quarantinedCount = 0,
            int deadCount = 0)
        {
            var archives = new List<EventArchiveV2>();
            int activeCount = count > 0 ? count : GetRandomPositiveNumber();

            for (int i = 0; i < activeCount; i++)
            {
                archives.Add(new EventArchiveV2
                {
                    Id = Guid.NewGuid(),
                    Status = EventArchiveStatusV2.Active,
                    RemainingRetryAttempts = GetRandomPositiveNumber(),
                    Content = GetRandomString(),
                    EventName = GetRandomString(),
                    CreatedDate = GetRandomDateTimeOffset(),
                    UpdatedDate = GetRandomDateTimeOffset(),
                    ArchivedDate = GetRandomDateTimeOffset(),
                    EventAddressId = Guid.NewGuid()
                });
            }

            for (int i = 0; i < quarantinedCount; i++)
            {
                archives.Add(new EventArchiveV2
                {
                    Id = Guid.NewGuid(),
                    Status = EventArchiveStatusV2.Quarantined,
                    RemainingRetryAttempts = GetRandomPositiveNumber(),
                    Content = GetRandomString(),
                    EventName = GetRandomString(),
                    CreatedDate = GetRandomDateTimeOffset(),
                    UpdatedDate = GetRandomDateTimeOffset(),
                    ArchivedDate = GetRandomDateTimeOffset(),
                    EventAddressId = Guid.NewGuid()
                });
            }

            for (int i = 0; i < deadCount; i++)
            {
                archives.Add(new EventArchiveV2
                {
                    Id = Guid.NewGuid(),
                    Status = EventArchiveStatusV2.Active,
                    RemainingRetryAttempts = 0,
                    Content = GetRandomString(),
                    EventName = GetRandomString(),
                    CreatedDate = GetRandomDateTimeOffset(),
                    UpdatedDate = GetRandomDateTimeOffset(),
                    ArchivedDate = GetRandomDateTimeOffset(),
                    EventAddressId = Guid.NewGuid()
                });
            }

            return archives.AsQueryable();
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

        private void SetupHealthOrchestrationMocks(
            CancellationToken cancellationToken,
            IQueryable<EventV2> events,
            IQueryable<EventAddressV2> addresses,
            IQueryable<EventListenerV2> listeners,
            IQueryable<ListenerEventV2> listenerEvents,
            IEnumerable<IEventHandler> handlers,
            IQueryable<EventArchiveV2> archives,
            IQueryable<ListenerEventArchiveV2> listenerArchives)
        {
            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(cancellationToken))
                    .ReturnsAsync(events);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sAsync(cancellationToken))
                    .ReturnsAsync(addresses);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventListenerV2sAsync(cancellationToken))
                    .ReturnsAsync(listeners);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(cancellationToken))
                    .ReturnsAsync(listenerEvents);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventHandlerV2sAsync(cancellationToken))
                    .ReturnsAsync(handlers);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sAsync(cancellationToken))
                    .ReturnsAsync(archives);

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(cancellationToken))
                    .ReturnsAsync(listenerArchives);
        }

        private void VerifyHealthOrchestrationMocksOnce(CancellationToken cancellationToken)
        {
            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(cancellationToken), Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sAsync(cancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventListenerV2sAsync(cancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(cancellationToken), Times.Once);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventHandlerV2sAsync(cancellationToken), Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sAsync(cancellationToken), Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(cancellationToken), Times.Once);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetHealthConfiguration(), Times.Once);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        private static Filler<EventAddressV2> CreateEventAddressV2Filler()
        {
            var filler = new Filler<EventAddressV2>();

            filler.Setup()
                .OnType<DateTimeOffset>()
                    .Use(GetRandomDateTimeOffset)

                .OnProperty(e => e.Events).IgnoreIt()
                .OnProperty(e => e.EventListenerV2s).IgnoreIt()
                .OnProperty(e => e.ListenerEventV2s).IgnoreIt()
                .OnProperty(e => e.EventArchiveV2s).IgnoreIt();

            return filler;
        }

    }
}

