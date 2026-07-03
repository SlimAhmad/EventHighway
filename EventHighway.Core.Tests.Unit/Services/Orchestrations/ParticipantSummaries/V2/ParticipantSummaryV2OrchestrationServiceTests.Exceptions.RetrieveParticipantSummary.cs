// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Orchestrations.ParticipantSummaries.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ParticipantSummaries.V2
{
    public partial class ParticipantSummaryV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveParticipantSummaryAsync()
        {
            // given
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<IEnumerable<ParticipantSummaryV2>> retrieveParticipantSummaryTask =
                this.participantSummaryV2OrchestrationService
                    .RetrieveParticipantSummaryV2Async(
                        TrafficPeriodV2.Day, GetRandomDateTimeOffset(), cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveParticipantSummaryTask.AsTask);

            actualException.Should().NotBeOfType<ParticipantSummaryV2OrchestrationDependencyException>();
            actualException.Should().NotBeOfType<ParticipantSummaryV2OrchestrationServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveParticipantSummaryIfTimeoutOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.", operationCanceledException);

            var timeoutParticipantSummaryV2OrchestrationException =
                new TimeoutParticipantSummaryV2OrchestrationException(
                    message: "Failed participant summary orchestration timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: operationCanceledException.Data);

            var expectedParticipantSummaryV2OrchestrationDependencyException =
                new ParticipantSummaryV2OrchestrationDependencyException(
                    message: "Participant summary dependency error occurred, contact support.",
                    innerException: timeoutParticipantSummaryV2OrchestrationException);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IEnumerable<ParticipantSummaryV2>> retrieveParticipantSummaryTask =
                this.participantSummaryV2OrchestrationService
                    .RetrieveParticipantSummaryV2Async(
                        TrafficPeriodV2.Day, GetRandomDateTimeOffset(), randomCancellationToken);

            ParticipantSummaryV2OrchestrationDependencyException
                actualParticipantSummaryV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<ParticipantSummaryV2OrchestrationDependencyException>(
                        retrieveParticipantSummaryTask.AsTask);

            // then
            actualParticipantSummaryV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedParticipantSummaryV2OrchestrationDependencyException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedParticipantSummaryV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveParticipantSummaryIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedParticipantSummaryV2OrchestrationDependencyException =
                new ParticipantSummaryV2OrchestrationDependencyException(
                    message: "Participant summary dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<IEnumerable<ParticipantSummaryV2>> retrieveParticipantSummaryTask =
                this.participantSummaryV2OrchestrationService
                    .RetrieveParticipantSummaryV2Async(
                        TrafficPeriodV2.Day, GetRandomDateTimeOffset(), randomCancellationToken);

            ParticipantSummaryV2OrchestrationDependencyException
                actualParticipantSummaryV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<ParticipantSummaryV2OrchestrationDependencyException>(
                        retrieveParticipantSummaryTask.AsTask);

            // then
            actualParticipantSummaryV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedParticipantSummaryV2OrchestrationDependencyException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedParticipantSummaryV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveParticipantSummaryIfServiceErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var exception = new Exception();
            exception.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedParticipantSummaryV2OrchestrationServiceException =
                new FailedParticipantSummaryV2OrchestrationServiceException(
                    message: "Failed participant summary service error occurred, contact support.",
                    innerException: exception,
                    data: exception.Data);

            var expectedParticipantSummaryV2OrchestrationServiceException =
                new ParticipantSummaryV2OrchestrationServiceException(
                    message: "Participant summary service error occurred, contact support.",
                    innerException: failedParticipantSummaryV2OrchestrationServiceException);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(exception);

            // when
            ValueTask<IEnumerable<ParticipantSummaryV2>> retrieveParticipantSummaryTask =
                this.participantSummaryV2OrchestrationService
                    .RetrieveParticipantSummaryV2Async(
                        TrafficPeriodV2.Day, GetRandomDateTimeOffset(), randomCancellationToken);

            ParticipantSummaryV2OrchestrationServiceException
                actualParticipantSummaryV2OrchestrationServiceException =
                    await Assert.ThrowsAsync<ParticipantSummaryV2OrchestrationServiceException>(
                        retrieveParticipantSummaryTask.AsTask);

            // then
            actualParticipantSummaryV2OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedParticipantSummaryV2OrchestrationServiceException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedParticipantSummaryV2OrchestrationServiceException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
