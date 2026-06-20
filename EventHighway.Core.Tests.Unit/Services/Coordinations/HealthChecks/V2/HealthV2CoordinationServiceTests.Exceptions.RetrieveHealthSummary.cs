// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.HealthChecks.V2
{
    public partial class HealthV2CoordinationServiceTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnRetrieveHealthSummaryIfValidationExceptionOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedHealthV2CoordinationDependencyValidationException =
                new HealthV2CoordinationDependencyValidationException(
                    message: "Health validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask<IEnumerable<HealthCheckItemV2>> retrieveTask =
                this.healthV2CoordinationService
                    .RetrieveHealthSummaryV2Async(randomCancellationToken);

            HealthV2CoordinationDependencyValidationException actualException =
                await Assert.ThrowsAsync<HealthV2CoordinationDependencyValidationException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthV2CoordinationDependencyValidationException);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHealthV2CoordinationDependencyValidationException))),
                        Times.Once);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveHealthSummaryIfTimeoutOccursAndLogItAsync()
        {
            // given
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutHealthV2CoordinationException =
                new TimeoutHealthV2CoordinationException(
                    message: "Failed health coordination timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedHealthV2CoordinationDependencyException =
                new HealthV2CoordinationDependencyException(
                    message: "Health dependency error occurred, contact support.",
                    innerException: timeoutHealthV2CoordinationException);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IEnumerable<HealthCheckItemV2>> retrieveHealthSummaryTask =
                this.healthV2CoordinationService
                    .RetrieveHealthSummaryV2Async(TestContext.Current.CancellationToken);

            HealthV2CoordinationDependencyException actualHealthV2CoordinationDependencyException =
                await Assert.ThrowsAsync<HealthV2CoordinationDependencyException>(
                    retrieveHealthSummaryTask.AsTask);

            // then
            actualHealthV2CoordinationDependencyException.Should().BeEquivalentTo(
                expectedHealthV2CoordinationDependencyException);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHealthV2CoordinationDependencyException))),
                        Times.Once);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveHealthSummaryIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedHealthV2CoordinationDependencyException =
                new HealthV2CoordinationDependencyException(
                    message: "Health dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<IEnumerable<HealthCheckItemV2>> retrieveTask =
                this.healthV2CoordinationService
                    .RetrieveHealthSummaryV2Async(randomCancellationToken);

            HealthV2CoordinationDependencyException actualException =
                await Assert.ThrowsAsync<HealthV2CoordinationDependencyException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthV2CoordinationDependencyException);

            this.eventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedHealthV2CoordinationDependencyException))),
                        Times.Once);

            this.eventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
