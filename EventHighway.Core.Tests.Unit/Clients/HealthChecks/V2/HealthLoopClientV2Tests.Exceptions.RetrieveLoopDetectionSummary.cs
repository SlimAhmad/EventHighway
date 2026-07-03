// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.HealthChecks.V2.Exceptions;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Orchestrations.LoopDetections.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.HealthChecks.V2
{
    public partial class HealthLoopClientV2Tests
    {
        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveLoopDetectionSummaryIfDependencyErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyError" });

            var loopDetectionV2OrchestrationDependencyException =
                new LoopDetectionV2OrchestrationDependencyException(
                    someMessage,
                    someInnerException);

            var expectedHealthLoopClientV2DependencyException =
                new HealthLoopClientV2DependencyException(
                    message: "Health client dependency error occurred, contact support.",
                    innerException: loopDetectionV2OrchestrationDependencyException.InnerException as Xeption,
                    data: (loopDetectionV2OrchestrationDependencyException.InnerException as Xeption).Data);

            this.loopDetectionV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveLoopDetectionSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(loopDetectionV2OrchestrationDependencyException);

            // when
            ValueTask<LoopDetectionSummaryV2> retrieveTask =
                this.healthLoopClientV2.RetrieveLoopDetectionSummaryV2Async(
                    GetRandomTrafficPeriodV2(), GetRandomDateTimeOffset(), randomCancellationToken);

            HealthLoopClientV2DependencyException actualException =
                await Assert.ThrowsAsync<HealthLoopClientV2DependencyException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthLoopClientV2DependencyException);

            this.loopDetectionV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveLoopDetectionSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loopDetectionV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveLoopDetectionSummaryIfServiceErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var loopDetectionV2OrchestrationServiceException =
                new LoopDetectionV2OrchestrationServiceException(
                    someMessage,
                    someInnerException);

            var expectedHealthLoopClientV2DependencyException =
                new HealthLoopClientV2DependencyException(
                    message: "Health client dependency error occurred, contact support.",
                    innerException: loopDetectionV2OrchestrationServiceException.InnerException as Xeption,
                    data: (loopDetectionV2OrchestrationServiceException.InnerException as Xeption).Data);

            this.loopDetectionV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveLoopDetectionSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(loopDetectionV2OrchestrationServiceException);

            // when
            ValueTask<LoopDetectionSummaryV2> retrieveTask =
                this.healthLoopClientV2.RetrieveLoopDetectionSummaryV2Async(
                    GetRandomTrafficPeriodV2(), GetRandomDateTimeOffset(), randomCancellationToken);

            HealthLoopClientV2DependencyException actualException =
                await Assert.ThrowsAsync<HealthLoopClientV2DependencyException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthLoopClientV2DependencyException);

            this.loopDetectionV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveLoopDetectionSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loopDetectionV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveLoopDetectionSummaryIfUnexpectedErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var someXeption = new Xeption(message: GetRandomString());

            var expectedHealthLoopClientV2ServiceException =
                new HealthLoopClientV2ServiceException(
                    message: "Health client service error occurred, contact support.",
                    innerException: someXeption,
                    data: someXeption.Data);

            this.loopDetectionV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveLoopDetectionSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(someXeption);

            // when
            ValueTask<LoopDetectionSummaryV2> retrieveTask =
                this.healthLoopClientV2.RetrieveLoopDetectionSummaryV2Async(
                    GetRandomTrafficPeriodV2(), GetRandomDateTimeOffset(), randomCancellationToken);

            HealthLoopClientV2ServiceException actualException =
                await Assert.ThrowsAsync<HealthLoopClientV2ServiceException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthLoopClientV2ServiceException);

            this.loopDetectionV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveLoopDetectionSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loopDetectionV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveLoopDetectionSummaryAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var operationCanceledException =
                new OperationCanceledException();

            this.loopDetectionV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveLoopDetectionSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<LoopDetectionSummaryV2> retrieveTask =
                this.healthLoopClientV2.RetrieveLoopDetectionSummaryV2Async(
                    GetRandomTrafficPeriodV2(), GetRandomDateTimeOffset(), randomCancellationToken);

            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(operationCanceledException);

            this.loopDetectionV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveLoopDetectionSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loopDetectionV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
