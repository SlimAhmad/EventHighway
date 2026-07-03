// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.HealthChecks.V2.Exceptions;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Orchestrations.DuplicateSummaries.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.HealthChecks.V2
{
    public partial class HealthDuplicateClientV2Tests
    {
        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveDuplicateDetectionSummaryIfDependencyErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyError" });

            var duplicateSummaryV2OrchestrationDependencyException =
                new DuplicateSummaryV2OrchestrationDependencyException(
                    someMessage,
                    someInnerException);

            var expectedHealthDuplicateClientV2DependencyException =
                new HealthDuplicateClientV2DependencyException(
                    message: "Health client dependency error occurred, contact support.",
                    innerException: duplicateSummaryV2OrchestrationDependencyException.InnerException as Xeption,
                    data: (duplicateSummaryV2OrchestrationDependencyException.InnerException as Xeption).Data);

            this.duplicateSummaryV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveDuplicateDetectionSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(duplicateSummaryV2OrchestrationDependencyException);

            // when
            ValueTask<DuplicateDetectionSummaryV2> retrieveTask =
                this.healthDuplicateClientV2.RetrieveDuplicateDetectionSummaryV2Async(
                    GetRandomTrafficPeriodV2(), GetRandomDateTimeOffset(), randomCancellationToken);

            HealthDuplicateClientV2DependencyException actualException =
                await Assert.ThrowsAsync<HealthDuplicateClientV2DependencyException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthDuplicateClientV2DependencyException);

            this.duplicateSummaryV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveDuplicateDetectionSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.duplicateSummaryV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveDuplicateDetectionSummaryIfServiceErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var duplicateSummaryV2OrchestrationServiceException =
                new DuplicateSummaryV2OrchestrationServiceException(
                    someMessage,
                    someInnerException);

            var expectedHealthDuplicateClientV2DependencyException =
                new HealthDuplicateClientV2DependencyException(
                    message: "Health client dependency error occurred, contact support.",
                    innerException: duplicateSummaryV2OrchestrationServiceException.InnerException as Xeption,
                    data: (duplicateSummaryV2OrchestrationServiceException.InnerException as Xeption).Data);

            this.duplicateSummaryV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveDuplicateDetectionSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(duplicateSummaryV2OrchestrationServiceException);

            // when
            ValueTask<DuplicateDetectionSummaryV2> retrieveTask =
                this.healthDuplicateClientV2.RetrieveDuplicateDetectionSummaryV2Async(
                    GetRandomTrafficPeriodV2(), GetRandomDateTimeOffset(), randomCancellationToken);

            HealthDuplicateClientV2DependencyException actualException =
                await Assert.ThrowsAsync<HealthDuplicateClientV2DependencyException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthDuplicateClientV2DependencyException);

            this.duplicateSummaryV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveDuplicateDetectionSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.duplicateSummaryV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveDuplicateDetectionSummaryIfUnexpectedErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var someXeption = new Xeption(message: GetRandomString());

            var expectedHealthDuplicateClientV2ServiceException =
                new HealthDuplicateClientV2ServiceException(
                    message: "Health client service error occurred, contact support.",
                    innerException: someXeption,
                    data: someXeption.Data);

            this.duplicateSummaryV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveDuplicateDetectionSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(someXeption);

            // when
            ValueTask<DuplicateDetectionSummaryV2> retrieveTask =
                this.healthDuplicateClientV2.RetrieveDuplicateDetectionSummaryV2Async(
                    GetRandomTrafficPeriodV2(), GetRandomDateTimeOffset(), randomCancellationToken);

            HealthDuplicateClientV2ServiceException actualException =
                await Assert.ThrowsAsync<HealthDuplicateClientV2ServiceException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthDuplicateClientV2ServiceException);

            this.duplicateSummaryV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveDuplicateDetectionSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.duplicateSummaryV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveDuplicateDetectionSummaryAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var operationCanceledException =
                new OperationCanceledException();

            this.duplicateSummaryV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveDuplicateDetectionSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<DuplicateDetectionSummaryV2> retrieveTask =
                this.healthDuplicateClientV2.RetrieveDuplicateDetectionSummaryV2Async(
                    GetRandomTrafficPeriodV2(), GetRandomDateTimeOffset(), randomCancellationToken);

            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(operationCanceledException);

            this.duplicateSummaryV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveDuplicateDetectionSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.duplicateSummaryV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
