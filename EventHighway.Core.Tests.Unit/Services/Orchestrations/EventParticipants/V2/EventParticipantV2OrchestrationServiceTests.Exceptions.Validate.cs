// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventParticipants.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventParticipants.V2
{
    public partial class EventParticipantV2OrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnValidateIfValidationExceptionOccursAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;
            inputEventV2.EventParticipantV2Id = GetRandomId();

            var expectedEventParticipantV2OrchestrationDependencyValidationException =
                new EventParticipantV2OrchestrationDependencyValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            this.eventParticipantV2ServiceMock.Setup(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask validateTask =
                this.eventParticipantV2OrchestrationService
                    .ValidateEventParticipantsAsync(
                        inputEventV2,
                        TestContext.Current.CancellationToken);

            EventParticipantV2OrchestrationDependencyValidationException
                actualException =
                    await Assert.ThrowsAsync<EventParticipantV2OrchestrationDependencyValidationException>(
                        validateTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedEventParticipantV2OrchestrationDependencyValidationException);

            this.eventParticipantV2ServiceMock.Verify(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2OrchestrationDependencyValidationException))),
                        Times.Once);

            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnValidateIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;
            inputEventV2.EventParticipantV2Id = GetRandomId();

            var expectedEventParticipantV2OrchestrationDependencyException =
                new EventParticipantV2OrchestrationDependencyException(
                    message: "Event participant dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventParticipantV2ServiceMock.Setup(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask validateTask =
                this.eventParticipantV2OrchestrationService
                    .ValidateEventParticipantsAsync(
                        inputEventV2,
                        TestContext.Current.CancellationToken);

            EventParticipantV2OrchestrationDependencyException
                actualException =
                    await Assert.ThrowsAsync<EventParticipantV2OrchestrationDependencyException>(
                        validateTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedEventParticipantV2OrchestrationDependencyException);

            this.eventParticipantV2ServiceMock.Verify(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnValidateIfServiceErrorOccursAndLogItAsync()
        {
            // given
            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;
            inputEventV2.EventParticipantV2Id = GetRandomId();

            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventParticipantV2OrchestrationServiceException =
                new FailedEventParticipantV2OrchestrationServiceException(
                    message: "Failed event participant service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventParticipantV2OrchestrationServiceException =
                new EventParticipantV2OrchestrationServiceException(
                    message: "Event participant service error occurred, contact support.",
                    innerException: failedEventParticipantV2OrchestrationServiceException);

            this.eventParticipantV2ServiceMock.Setup(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask validateTask =
                this.eventParticipantV2OrchestrationService
                    .ValidateEventParticipantsAsync(
                        inputEventV2,
                        TestContext.Current.CancellationToken);

            EventParticipantV2OrchestrationServiceException
                actualException =
                    await Assert.ThrowsAsync<EventParticipantV2OrchestrationServiceException>(
                        validateTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedEventParticipantV2OrchestrationServiceException);

            this.eventParticipantV2ServiceMock.Verify(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2OrchestrationServiceException))),
                        Times.Once);

            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnValidateIfTimeoutOccursAndLogItAsync()
        {
            // given
            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;
            inputEventV2.EventParticipantV2Id = GetRandomId();

            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutEventParticipantV2OrchestrationException =
                new TimeoutEventParticipantV2OrchestrationException(
                    message: "Failed event participant orchestration timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedEventParticipantV2OrchestrationDependencyException =
                new EventParticipantV2OrchestrationDependencyException(
                    message: "Event participant dependency error occurred, contact support.",
                    innerException: timeoutEventParticipantV2OrchestrationException);

            this.eventParticipantV2ServiceMock.Setup(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask validateTask =
                this.eventParticipantV2OrchestrationService
                    .ValidateEventParticipantsAsync(
                        inputEventV2,
                        TestContext.Current.CancellationToken);

            EventParticipantV2OrchestrationDependencyException
                actualException =
                    await Assert.ThrowsAsync<EventParticipantV2OrchestrationDependencyException>(
                        validateTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedEventParticipantV2OrchestrationDependencyException);

            this.eventParticipantV2ServiceMock.Verify(service =>
                service.RetrieveEventParticipantV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnValidateAsync()
        {
            // given
            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;
            inputEventV2.EventParticipantV2Id = GetRandomId();

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask validateTask =
                this.eventParticipantV2OrchestrationService
                    .ValidateEventParticipantsAsync(
                        inputEventV2,
                        cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    validateTask.AsTask);

            actualException.Should().NotBeOfType<EventParticipantV2OrchestrationDependencyException>();
            actualException.Should().NotBeOfType<EventParticipantV2OrchestrationServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
            this.eventParticipantSecretV2ServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
