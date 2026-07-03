// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Coordinations.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventFirings.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.V2
{
    public partial class EventV2CoordinationServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnSubmitIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 someEventV2 = CreateRandomEventV2();

            SetupValidateEventParticipantsSucceeds();

            var expectedEventV2CoordinationDependencyValidationException =
                new EventV2CoordinationDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.dateTimeBrokerMock.Setup(service =>
                service.GetDateTimeOffsetAsync())
                    .ThrowsAsync(validationException);

            // when
            ValueTask<EventV2> submitEventV2Task =
                this.eventV2CoordinationService.SubmitEventV2Async(
                    someEventV2,
                    randomCancellationToken);

            EventV2CoordinationDependencyValidationException
                actualEventV2CoordinationDependencyValidationException =
                    await Assert.ThrowsAsync<EventV2CoordinationDependencyValidationException>(
                        submitEventV2Task.AsTask);

            // then
            actualEventV2CoordinationDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventV2CoordinationDependencyValidationException);

            VerifyValidateEventParticipantsCalledOnce();

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2CoordinationDependencyValidationException))),
                        Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.SubmitEventV2Async(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventFiringV2OrchestrationServiceMock.Verify(service =>
                service.FireEventV2Async(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventFiringV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventParticipantV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnSubmitIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 someEventV2 = CreateRandomEventV2();

            SetupValidateEventParticipantsSucceeds();

            var expectedEventV2CoordinationDependencyException =
                new EventV2CoordinationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.dateTimeBrokerMock.Setup(service =>
                service.GetDateTimeOffsetAsync())
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<EventV2> submitEventV2Task =
                this.eventV2CoordinationService.SubmitEventV2Async(
                    someEventV2,
                    randomCancellationToken);

            EventV2CoordinationDependencyException
                actualEventV2CoordinationDependencyException =
                    await Assert.ThrowsAsync<EventV2CoordinationDependencyException>(
                        submitEventV2Task.AsTask);

            // then
            actualEventV2CoordinationDependencyException.Should()
                .BeEquivalentTo(expectedEventV2CoordinationDependencyException);

            VerifyValidateEventParticipantsCalledOnce();

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2CoordinationDependencyException))),
                        Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.SubmitEventV2Async(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventFiringV2OrchestrationServiceMock.Verify(service =>
                service.FireEventV2Async(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventFiringV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventParticipantV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnSubmitAsync()
        {
            // given
            EventV2 someEventV2 = CreateRandomEventV2();
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<EventV2> submitEventV2Task =
                this.eventV2CoordinationService.SubmitEventV2Async(
                    someEventV2,
                    cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    submitEventV2Task.AsTask);

            actualException.Should().NotBeOfType<EventV2CoordinationDependencyException>();
            actualException.Should().NotBeOfType<EventV2CoordinationServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventFiringV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventParticipantV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnSubmitIfTimeoutOccursAndLogItAsync()
        {
            // given
            EventV2 someEventV2 = CreateRandomEventV2();

            SetupValidateEventParticipantsSucceeds();
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.", operationCanceledException);

            var timeoutEventV2CoordinationException =
                new TimeoutEventV2CoordinationException(
                    message: "Failed event coordination timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: operationCanceledException.Data);

            var expectedEventV2CoordinationDependencyException =
                new EventV2CoordinationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: timeoutEventV2CoordinationException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<EventV2> submitEventV2Task =
                this.eventV2CoordinationService.SubmitEventV2Async(
                    someEventV2,
                    TestContext.Current.CancellationToken);

            EventV2CoordinationDependencyException
                actualEventV2CoordinationDependencyException =
                    await Assert.ThrowsAsync<EventV2CoordinationDependencyException>(
                        submitEventV2Task.AsTask);

            // then
            actualEventV2CoordinationDependencyException.Should()
                .BeEquivalentTo(expectedEventV2CoordinationDependencyException);

            VerifyValidateEventParticipantsCalledOnce();

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2CoordinationDependencyException))),
                        Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.SubmitEventV2Async(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventFiringV2OrchestrationServiceMock.Verify(service =>
                service.FireEventV2Async(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventFiringV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventParticipantV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnSubmitIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 someEventV2 = CreateRandomEventV2();

            SetupValidateEventParticipantsSucceeds();
            var serviceException = new Exception();

            var failedEventV2CoordinationServiceException =
                new FailedEventV2CoordinationServiceException(
                    message: "Failed event service error occurred, contact support.",
                    innerException: serviceException);

            var expectedEventV2CoordinationServiceException =
                new EventV2CoordinationServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: failedEventV2CoordinationServiceException);

            this.dateTimeBrokerMock.Setup(service =>
                service.GetDateTimeOffsetAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<EventV2> submitEventV2Task =
                this.eventV2CoordinationService.SubmitEventV2Async(
                    someEventV2,
                    randomCancellationToken);

            EventV2CoordinationServiceException
                actualEventV2CoordinationServiceException =
                    await Assert.ThrowsAsync<EventV2CoordinationServiceException>(
                        submitEventV2Task.AsTask);

            // then
            actualEventV2CoordinationServiceException.Should()
                .BeEquivalentTo(expectedEventV2CoordinationServiceException);

            VerifyValidateEventParticipantsCalledOnce();

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2CoordinationServiceException))),
                        Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.SubmitEventV2Async(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventFiringV2OrchestrationServiceMock.Verify(service =>
                service.FireEventV2Async(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventFiringV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventParticipantV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnSubmitIfFiringDependencyErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;
            inputEventV2.ScheduledDate = null;
            EventV2 inputImmediateEventV2 = inputEventV2;
            inputImmediateEventV2.Type = EventTypeV2.Immediate;
            EventV2 submittedEventV2 = inputImmediateEventV2;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption();

            var eventFiringV2OrchestrationDependencyException =
                new EventFiringV2OrchestrationDependencyException(
                    someMessage,
                    someInnerException);

            var expectedEventV2CoordinationDependencyException =
                new EventV2CoordinationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: eventFiringV2OrchestrationDependencyException.InnerException as Xeption);

            SetupValidateEventParticipantsSucceeds();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.StampContentHashAsync(
                    inputImmediateEventV2,
                    randomCancellationToken))
                        .ReturnsAsync(inputImmediateEventV2);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.IsLoopDetectedAsync(
                    inputImmediateEventV2,
                    randomCancellationToken))
                        .ReturnsAsync(false);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.SubmitEventV2Async(
                    inputImmediateEventV2,
                    randomCancellationToken))
                        .ReturnsAsync(submittedEventV2);

            this.eventFiringV2OrchestrationServiceMock.Setup(service =>
                service.FireEventV2Async(
                    submittedEventV2,
                    randomCancellationToken))
                        .ThrowsAsync(eventFiringV2OrchestrationDependencyException);

            // when
            ValueTask<EventV2> submitEventV2Task =
                this.eventV2CoordinationService.SubmitEventV2Async(
                    inputEventV2,
                    randomCancellationToken);

            EventV2CoordinationDependencyException
                actualEventV2CoordinationDependencyException =
                    await Assert.ThrowsAsync<EventV2CoordinationDependencyException>(
                        submitEventV2Task.AsTask);

            // then
            actualEventV2CoordinationDependencyException.Should()
                .BeEquivalentTo(expectedEventV2CoordinationDependencyException);

            VerifyValidateEventParticipantsCalledOnce();

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.StampContentHashAsync(
                    inputImmediateEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.IsLoopDetectedAsync(
                    inputImmediateEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.SubmitEventV2Async(
                    inputImmediateEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventFiringV2OrchestrationServiceMock.Verify(service =>
                service.FireEventV2Async(
                    submittedEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2CoordinationDependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventFiringV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventParticipantV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
