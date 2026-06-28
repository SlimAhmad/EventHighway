// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.ReplayingEvents.V2
{
    public partial class ReplayingEventV2CoordinationServiceTests
    {
        [Fact]
        public async Task ShouldTargetedReplayEventArchiveV2ForSpecifiedListenersAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid eventV2Id = GetRandomId();
            Guid? eventAddressId = GetRandomId();

            List<Guid> eventListenerIds =
                new List<Guid> { GetRandomId(), GetRandomId() };

            EventArchiveV2 eventArchiveV2 = CreateRandomEventArchiveV2s().First();
            eventArchiveV2.Id = eventV2Id;
            eventArchiveV2.EventAddressId = eventAddressId.Value;
            eventArchiveV2.Status = EventArchiveStatusV2.Active;

            List<EventArchiveV2> retrievedEventArchiveV2s =
                new List<EventArchiveV2> { eventArchiveV2 };

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveEventArchiveV2sByIdsAsync(
                    It.Is<IEnumerable<Guid>>(ids => ids.Single() == eventV2Id),
                    randomCancellationToken))
                        .ReturnsAsync(retrievedEventArchiveV2s);

            this.restoringEventV2OrchestrationServiceMock.Setup(service =>
                service.RestoreAsync(
                    It.IsAny<IEnumerable<EventArchiveV2>>(),
                    It.IsAny<IEnumerable<ListenerEventArchiveV2>>(),
                    randomCancellationToken))
                        .Returns(ValueTask.CompletedTask);

            this.restoringEventV2OrchestrationServiceMock.Setup(service =>
                service.GenerateReplayForListenersAsync(
                    It.IsAny<IEnumerable<EventArchiveV2>>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    randomCancellationToken))
                        .Returns(ValueTask.CompletedTask);

            // when
            await this.replayingEventV2CoordinationService.ReplayEventArchiveV2sAsync(
                eventV2Id, eventAddressId, eventListenerIds, false, randomCancellationToken);

            // then
            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventArchiveV2sByIdsAsync(
                    It.Is<IEnumerable<Guid>>(ids => ids.Single() == eventV2Id),
                    randomCancellationToken),
                        Times.Once);

            this.restoringEventV2OrchestrationServiceMock.Verify(service =>
                service.RestoreAsync(
                    It.Is<IEnumerable<EventArchiveV2>>(archives =>
                        archives.Single() == eventArchiveV2),
                    It.Is<IEnumerable<ListenerEventArchiveV2>>(listenerArchives =>
                        listenerArchives.Any() == false),
                    randomCancellationToken),
                        Times.Once);

            this.restoringEventV2OrchestrationServiceMock.Verify(service =>
                service.GenerateReplayForListenersAsync(
                    It.Is<IEnumerable<EventArchiveV2>>(archives =>
                        archives.Single() == eventArchiveV2),
                    eventListenerIds,
                    randomCancellationToken),
                        Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.restoringEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldNotTargetedReplayQuarantinedItemWhenNotAllowedAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid eventV2Id = GetRandomId();
            Guid? eventAddressId = GetRandomId();
            List<Guid> eventListenerIds = new List<Guid> { GetRandomId() };

            EventArchiveV2 quarantinedEventArchiveV2 = CreateRandomEventArchiveV2s().First();
            quarantinedEventArchiveV2.Id = eventV2Id;
            quarantinedEventArchiveV2.EventAddressId = eventAddressId.Value;
            quarantinedEventArchiveV2.Status = EventArchiveStatusV2.Quarantined;

            List<EventArchiveV2> retrievedEventArchiveV2s =
                new List<EventArchiveV2> { quarantinedEventArchiveV2 };

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveEventArchiveV2sByIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(),
                    randomCancellationToken))
                        .ReturnsAsync(retrievedEventArchiveV2s);

            // when
            await this.replayingEventV2CoordinationService.ReplayEventArchiveV2sAsync(
                eventV2Id, eventAddressId, eventListenerIds, false, randomCancellationToken);

            // then
            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventArchiveV2sByIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(),
                    randomCancellationToken),
                        Times.Once);

            this.restoringEventV2OrchestrationServiceMock.Verify(service =>
                service.RestoreAsync(
                    It.IsAny<IEnumerable<EventArchiveV2>>(),
                    It.IsAny<IEnumerable<ListenerEventArchiveV2>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.restoringEventV2OrchestrationServiceMock.Verify(service =>
                service.GenerateReplayForListenersAsync(
                    It.IsAny<IEnumerable<EventArchiveV2>>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.restoringEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldTargetedReplayQuarantinedItemWhenAllowedAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid eventV2Id = GetRandomId();
            Guid? eventAddressId = GetRandomId();
            List<Guid> eventListenerIds = new List<Guid> { GetRandomId() };

            EventArchiveV2 quarantinedEventArchiveV2 = CreateRandomEventArchiveV2s().First();
            quarantinedEventArchiveV2.Id = eventV2Id;
            quarantinedEventArchiveV2.EventAddressId = eventAddressId.Value;
            quarantinedEventArchiveV2.Status = EventArchiveStatusV2.Quarantined;

            List<EventArchiveV2> retrievedEventArchiveV2s =
                new List<EventArchiveV2> { quarantinedEventArchiveV2 };

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveEventArchiveV2sByIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(),
                    randomCancellationToken))
                        .ReturnsAsync(retrievedEventArchiveV2s);

            this.restoringEventV2OrchestrationServiceMock.Setup(service =>
                service.RestoreAsync(
                    It.IsAny<IEnumerable<EventArchiveV2>>(),
                    It.IsAny<IEnumerable<ListenerEventArchiveV2>>(),
                    randomCancellationToken))
                        .Returns(ValueTask.CompletedTask);

            this.restoringEventV2OrchestrationServiceMock.Setup(service =>
                service.GenerateReplayForListenersAsync(
                    It.IsAny<IEnumerable<EventArchiveV2>>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    randomCancellationToken))
                        .Returns(ValueTask.CompletedTask);

            // when
            await this.replayingEventV2CoordinationService.ReplayEventArchiveV2sAsync(
                eventV2Id, eventAddressId, eventListenerIds, true, randomCancellationToken);

            // then
            this.restoringEventV2OrchestrationServiceMock.Verify(service =>
                service.RestoreAsync(
                    It.Is<IEnumerable<EventArchiveV2>>(archives =>
                        archives.Single() == quarantinedEventArchiveV2),
                    It.Is<IEnumerable<ListenerEventArchiveV2>>(listenerArchives =>
                        listenerArchives.Any() == false),
                    randomCancellationToken),
                        Times.Once);

            this.restoringEventV2OrchestrationServiceMock.Verify(service =>
                service.GenerateReplayForListenersAsync(
                    It.Is<IEnumerable<EventArchiveV2>>(archives =>
                        archives.Single() == quarantinedEventArchiveV2),
                    eventListenerIds,
                    randomCancellationToken),
                        Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventArchiveV2sByIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(),
                    randomCancellationToken),
                        Times.Once);

            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.restoringEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldNotTargetedReplayWhenEventArchiveV2NotFoundAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid eventV2Id = GetRandomId();
            Guid? eventAddressId = GetRandomId();
            List<Guid> eventListenerIds = new List<Guid> { GetRandomId() };

            this.eventArchiveV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveEventArchiveV2sByIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(),
                    randomCancellationToken))
                        .ReturnsAsync(new List<EventArchiveV2>());

            // when
            await this.replayingEventV2CoordinationService.ReplayEventArchiveV2sAsync(
                eventV2Id, eventAddressId, eventListenerIds, false, randomCancellationToken);

            // then
            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventArchiveV2sByIdsAsync(
                    It.IsAny<IEnumerable<Guid>>(),
                    randomCancellationToken),
                        Times.Once);

            this.restoringEventV2OrchestrationServiceMock.Verify(service =>
                service.RestoreAsync(
                    It.IsAny<IEnumerable<EventArchiveV2>>(),
                    It.IsAny<IEnumerable<ListenerEventArchiveV2>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.restoringEventV2OrchestrationServiceMock.Verify(service =>
                service.GenerateReplayForListenersAsync(
                    It.IsAny<IEnumerable<EventArchiveV2>>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.restoringEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
