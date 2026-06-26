// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.HealthChecks.V2.Exceptions;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.HealthChecks.V2
{
    public partial class HealthDuplicateClientV2Tests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowValidationExceptionOnRetrieveDuplicateDetectionSummaryIfValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedHealthDuplicateClientV2ValidationException =
                new HealthDuplicateClientV2ValidationException(
                    message: "Health client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption,
                    data: (validationException.InnerException as Xeption).Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveDuplicateDetectionSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<DuplicateDetectionSummaryV2> retrieveTask =
                this.healthDuplicateClientV2.RetrieveDuplicateDetectionSummaryV2Async(
                    GetRandomTrafficPeriodV2(), GetRandomDateTimeOffset(), randomCancellationToken);

            HealthDuplicateClientV2ValidationException actualException =
                await Assert.ThrowsAsync<HealthDuplicateClientV2ValidationException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthDuplicateClientV2ValidationException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveDuplicateDetectionSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveDuplicateDetectionSummaryIfDependencyErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyError" });

            var healthV2CoordinationDependencyException =
                new HealthV2CoordinationDependencyException(
                    someMessage,
                    someInnerException);

            var expectedHealthDuplicateClientV2DependencyException =
                new HealthDuplicateClientV2DependencyException(
                    message: "Health client dependency error occurred, contact support.",
                    innerException: healthV2CoordinationDependencyException.InnerException as Xeption,
                    data: (healthV2CoordinationDependencyException.InnerException as Xeption).Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveDuplicateDetectionSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(healthV2CoordinationDependencyException);

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

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveDuplicateDetectionSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
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

            var healthV2CoordinationServiceException =
                new HealthV2CoordinationServiceException(
                    someMessage,
                    someInnerException);

            var expectedHealthDuplicateClientV2DependencyException =
                new HealthDuplicateClientV2DependencyException(
                    message: "Health client dependency error occurred, contact support.",
                    innerException: healthV2CoordinationServiceException.InnerException as Xeption,
                    data: (healthV2CoordinationServiceException.InnerException as Xeption).Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveDuplicateDetectionSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(healthV2CoordinationServiceException);

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

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveDuplicateDetectionSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
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

            this.healthV2CoordinationServiceMock.Setup(service =>
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

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveDuplicateDetectionSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
