// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.HealthChecks.V2.Exceptions;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Orchestrations.RetrySummaries.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.HealthChecks.V2
{
    public partial class HealthRetryClientV2Tests
    {
        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveRetryHealthIfDependencyErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyError" });

            var retrySummaryV2OrchestrationDependencyException =
                new RetrySummaryV2OrchestrationDependencyException(
                    someMessage,
                    someInnerException);

            var expectedHealthRetryClientV2DependencyException =
                new HealthRetryClientV2DependencyException(
                    message: "Health client dependency error occurred, contact support.",
                    innerException: retrySummaryV2OrchestrationDependencyException.InnerException as Xeption,
                    data: (retrySummaryV2OrchestrationDependencyException.InnerException as Xeption).Data);

            this.retrySummaryV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveRetryHealthV2Async(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(retrySummaryV2OrchestrationDependencyException);

            // when
            ValueTask<RetryHealthSummaryV2> retrieveTask =
                this.healthRetryClientV2.RetrieveRetryHealthV2Async(randomCancellationToken);

            HealthRetryClientV2DependencyException actualException =
                await Assert.ThrowsAsync<HealthRetryClientV2DependencyException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthRetryClientV2DependencyException);

            this.retrySummaryV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveRetryHealthV2Async(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.retrySummaryV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveRetryHealthIfServiceErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var retrySummaryV2OrchestrationServiceException =
                new RetrySummaryV2OrchestrationServiceException(
                    someMessage,
                    someInnerException);

            var expectedHealthRetryClientV2DependencyException =
                new HealthRetryClientV2DependencyException(
                    message: "Health client dependency error occurred, contact support.",
                    innerException: retrySummaryV2OrchestrationServiceException.InnerException as Xeption,
                    data: (retrySummaryV2OrchestrationServiceException.InnerException as Xeption).Data);

            this.retrySummaryV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveRetryHealthV2Async(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(retrySummaryV2OrchestrationServiceException);

            // when
            ValueTask<RetryHealthSummaryV2> retrieveTask =
                this.healthRetryClientV2.RetrieveRetryHealthV2Async(randomCancellationToken);

            HealthRetryClientV2DependencyException actualException =
                await Assert.ThrowsAsync<HealthRetryClientV2DependencyException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthRetryClientV2DependencyException);

            this.retrySummaryV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveRetryHealthV2Async(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.retrySummaryV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveRetryHealthIfUnexpectedErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var someXeption = new Xeption(message: GetRandomString());

            var expectedHealthRetryClientV2ServiceException =
                new HealthRetryClientV2ServiceException(
                    message: "Health client service error occurred, contact support.",
                    innerException: someXeption,
                    data: someXeption.Data);

            this.retrySummaryV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveRetryHealthV2Async(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(someXeption);

            // when
            ValueTask<RetryHealthSummaryV2> retrieveTask =
                this.healthRetryClientV2.RetrieveRetryHealthV2Async(randomCancellationToken);

            HealthRetryClientV2ServiceException actualException =
                await Assert.ThrowsAsync<HealthRetryClientV2ServiceException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthRetryClientV2ServiceException);

            this.retrySummaryV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveRetryHealthV2Async(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.retrySummaryV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveRetryHealthAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var operationCanceledException =
                new OperationCanceledException();

            this.retrySummaryV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveRetryHealthV2Async(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<RetryHealthSummaryV2> retrieveTask =
                this.healthRetryClientV2.RetrieveRetryHealthV2Async(randomCancellationToken);

            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(operationCanceledException);

            this.retrySummaryV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveRetryHealthV2Async(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.retrySummaryV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
