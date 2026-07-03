// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Orchestrations.RagStatuses.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.RagStatuses.V2
{
    public partial class RagStatusV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveHealthRagStatusAsync()
        {
            // given
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<IEnumerable<HealthCheckItemV2>> retrieveHealthRagStatusTask =
                this.ragStatusV2OrchestrationService
                    .RetrieveHealthRagStatusV2Async(cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveHealthRagStatusTask.AsTask);

            actualException.Should().NotBeOfType<RagStatusV2OrchestrationDependencyException>();
            actualException.Should().NotBeOfType<RagStatusV2OrchestrationServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveHealthRagStatusIfTimeoutOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.", operationCanceledException);

            var timeoutRagStatusV2OrchestrationException =
                new TimeoutRagStatusV2OrchestrationException(
                    message: "Failed rag status orchestration timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: operationCanceledException.Data);

            var expectedRagStatusV2OrchestrationDependencyException =
                new RagStatusV2OrchestrationDependencyException(
                    message: "Rag status dependency error occurred, contact support.",
                    innerException: timeoutRagStatusV2OrchestrationException);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IEnumerable<HealthCheckItemV2>> retrieveHealthRagStatusTask =
                this.ragStatusV2OrchestrationService
                    .RetrieveHealthRagStatusV2Async(randomCancellationToken);

            RagStatusV2OrchestrationDependencyException
                actualRagStatusV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<RagStatusV2OrchestrationDependencyException>(
                        retrieveHealthRagStatusTask.AsTask);

            // then
            actualRagStatusV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedRagStatusV2OrchestrationDependencyException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedRagStatusV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveHealthRagStatusIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedRagStatusV2OrchestrationDependencyException =
                new RagStatusV2OrchestrationDependencyException(
                    message: "Rag status dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<IEnumerable<HealthCheckItemV2>> retrieveHealthRagStatusTask =
                this.ragStatusV2OrchestrationService
                    .RetrieveHealthRagStatusV2Async(randomCancellationToken);

            RagStatusV2OrchestrationDependencyException
                actualRagStatusV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<RagStatusV2OrchestrationDependencyException>(
                        retrieveHealthRagStatusTask.AsTask);

            // then
            actualRagStatusV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedRagStatusV2OrchestrationDependencyException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedRagStatusV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveHealthRagStatusIfServiceErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var exception = new Exception();
            exception.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedRagStatusV2OrchestrationServiceException =
                new FailedRagStatusV2OrchestrationServiceException(
                    message: "Failed rag status service error occurred, contact support.",
                    innerException: exception,
                    data: exception.Data);

            var expectedRagStatusV2OrchestrationServiceException =
                new RagStatusV2OrchestrationServiceException(
                    message: "Rag status service error occurred, contact support.",
                    innerException: failedRagStatusV2OrchestrationServiceException);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(exception);

            // when
            ValueTask<IEnumerable<HealthCheckItemV2>> retrieveHealthRagStatusTask =
                this.ragStatusV2OrchestrationService
                    .RetrieveHealthRagStatusV2Async(randomCancellationToken);

            RagStatusV2OrchestrationServiceException
                actualRagStatusV2OrchestrationServiceException =
                    await Assert.ThrowsAsync<RagStatusV2OrchestrationServiceException>(
                        retrieveHealthRagStatusTask.AsTask);

            // then
            actualRagStatusV2OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedRagStatusV2OrchestrationServiceException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedRagStatusV2OrchestrationServiceException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
