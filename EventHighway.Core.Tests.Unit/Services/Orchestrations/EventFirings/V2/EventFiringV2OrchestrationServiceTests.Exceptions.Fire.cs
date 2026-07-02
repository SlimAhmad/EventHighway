// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventFirings.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventFirings.V2
{
    public partial class EventFiringV2OrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnFireIfValidationExceptionOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 someEventV2 = CreateRandomEventV2();

            var expectedEventFiringV2OrchestrationDependencyValidationException =
                new EventFiringV2OrchestrationDependencyValidationException(
                    message: "Event firing validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventListenerV2ProcessingServiceMock.Setup(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<EventV2> fireEventV2Task =
                this.eventFiringV2OrchestrationService
                    .FireEventV2Async(
                        someEventV2,
                        randomCancellationToken);

            EventFiringV2OrchestrationDependencyValidationException
                actualEventFiringV2OrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<EventFiringV2OrchestrationDependencyValidationException>(
                        fireEventV2Task.AsTask);

            // then
            actualEventFiringV2OrchestrationDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventFiringV2OrchestrationDependencyValidationException);

            this.eventListenerV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventFiringV2OrchestrationDependencyValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnFireIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 someEventV2 = CreateRandomEventV2();

            var expectedEventFiringV2OrchestrationDependencyException =
                new EventFiringV2OrchestrationDependencyException(
                    message: "Event firing dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventListenerV2ProcessingServiceMock.Setup(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<EventV2> fireEventV2Task =
                this.eventFiringV2OrchestrationService
                    .FireEventV2Async(
                        someEventV2,
                        randomCancellationToken);

            EventFiringV2OrchestrationDependencyException
                actualEventFiringV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<EventFiringV2OrchestrationDependencyException>(
                        fireEventV2Task.AsTask);

            // then
            actualEventFiringV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedEventFiringV2OrchestrationDependencyException);

            this.eventListenerV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventFiringV2OrchestrationDependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventListenerV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
