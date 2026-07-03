// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.HealthChecks.V2.Exceptions;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Orchestrations.RagStatuses.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.HealthChecks.V2
{
    public partial class HealthStatusClientV2Tests
    {
        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveHealthSummaryIfDependencyErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyError" });

            var ragStatusV2OrchestrationDependencyException =
                new RagStatusV2OrchestrationDependencyException(
                    someMessage,
                    someInnerException);

            var expectedHealthStatusClientV2DependencyException =
                new HealthStatusClientV2DependencyException(
                    message: "Health client dependency error occurred, contact support.",
                    innerException: ragStatusV2OrchestrationDependencyException.InnerException as Xeption,
                    data: (ragStatusV2OrchestrationDependencyException.InnerException as Xeption).Data);

            this.ragStatusV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveHealthRagStatusV2Async(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(ragStatusV2OrchestrationDependencyException);

            // when
            ValueTask<IEnumerable<HealthCheckItemV2>> retrieveTask =
                this.healthV2Client.RetrieveHealthRagStatusV2Async(randomCancellationToken);

            HealthStatusClientV2DependencyException actualException =
                await Assert.ThrowsAsync<HealthStatusClientV2DependencyException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthStatusClientV2DependencyException);

            this.ragStatusV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveHealthRagStatusV2Async(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.ragStatusV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveHealthSummaryIfServiceErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var ragStatusV2OrchestrationServiceException =
                new RagStatusV2OrchestrationServiceException(
                    someMessage,
                    someInnerException);

            var expectedHealthStatusClientV2DependencyException =
                new HealthStatusClientV2DependencyException(
                    message: "Health client dependency error occurred, contact support.",
                    innerException: ragStatusV2OrchestrationServiceException.InnerException as Xeption,
                    data: (ragStatusV2OrchestrationServiceException.InnerException as Xeption).Data);

            this.ragStatusV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveHealthRagStatusV2Async(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(ragStatusV2OrchestrationServiceException);

            // when
            ValueTask<IEnumerable<HealthCheckItemV2>> retrieveTask =
                this.healthV2Client.RetrieveHealthRagStatusV2Async(randomCancellationToken);

            HealthStatusClientV2DependencyException actualException =
                await Assert.ThrowsAsync<HealthStatusClientV2DependencyException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthStatusClientV2DependencyException);

            this.ragStatusV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveHealthRagStatusV2Async(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.ragStatusV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveHealthSummaryIfUnexpectedErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var someXeption = new Xeption(message: GetRandomString());

            var expectedHealthStatusClientV2ServiceException =
                new HealthStatusClientV2ServiceException(
                    message: "Health client service error occurred, contact support.",
                    innerException: someXeption,
                    data: someXeption.Data);

            this.ragStatusV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveHealthRagStatusV2Async(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(someXeption);

            // when
            ValueTask<IEnumerable<HealthCheckItemV2>> retrieveTask =
                this.healthV2Client.RetrieveHealthRagStatusV2Async(randomCancellationToken);

            HealthStatusClientV2ServiceException actualException =
                await Assert.ThrowsAsync<HealthStatusClientV2ServiceException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthStatusClientV2ServiceException);

            this.ragStatusV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveHealthRagStatusV2Async(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.ragStatusV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveHealthSummaryAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var operationCanceledException =
                new OperationCanceledException();

            this.ragStatusV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveHealthRagStatusV2Async(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IEnumerable<HealthCheckItemV2>> retrieveTask =
                this.healthV2Client.RetrieveHealthRagStatusV2Async(randomCancellationToken);

            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(operationCanceledException);

            this.ragStatusV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveHealthRagStatusV2Async(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.ragStatusV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
