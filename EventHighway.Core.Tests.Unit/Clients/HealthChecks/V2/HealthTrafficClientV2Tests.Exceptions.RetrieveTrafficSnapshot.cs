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
    public partial class HealthTrafficClientV2Tests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowValidationExceptionOnRetrieveTrafficSnapshotIfValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedHealthTrafficClientV2ValidationException =
                new HealthTrafficClientV2ValidationException(
                    message: "Health client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption,
                    data: (validationException.InnerException as Xeption).Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveTrafficSnapshotV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<TrafficSnapshotV2> retrieveTask =
                this.healthTrafficClientV2.RetrieveTrafficSnapshotV2Async(
                    GetRandomTrafficPeriodV2(), GetRandomDateTimeOffset(), randomCancellationToken);

            HealthTrafficClientV2ValidationException actualException =
                await Assert.ThrowsAsync<HealthTrafficClientV2ValidationException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthTrafficClientV2ValidationException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveTrafficSnapshotV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveTrafficSnapshotIfDependencyErrorOccursAsync()
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

            var expectedHealthTrafficClientV2DependencyException =
                new HealthTrafficClientV2DependencyException(
                    message: "Health client dependency error occurred, contact support.",
                    innerException: healthV2CoordinationDependencyException.InnerException as Xeption,
                    data: (healthV2CoordinationDependencyException.InnerException as Xeption).Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveTrafficSnapshotV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(healthV2CoordinationDependencyException);

            // when
            ValueTask<TrafficSnapshotV2> retrieveTask =
                this.healthTrafficClientV2.RetrieveTrafficSnapshotV2Async(
                    GetRandomTrafficPeriodV2(), GetRandomDateTimeOffset(), randomCancellationToken);

            HealthTrafficClientV2DependencyException actualException =
                await Assert.ThrowsAsync<HealthTrafficClientV2DependencyException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthTrafficClientV2DependencyException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveTrafficSnapshotV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveTrafficSnapshotIfServiceErrorOccursAsync()
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

            var expectedHealthTrafficClientV2DependencyException =
                new HealthTrafficClientV2DependencyException(
                    message: "Health client dependency error occurred, contact support.",
                    innerException: healthV2CoordinationServiceException.InnerException as Xeption,
                    data: (healthV2CoordinationServiceException.InnerException as Xeption).Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveTrafficSnapshotV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(healthV2CoordinationServiceException);

            // when
            ValueTask<TrafficSnapshotV2> retrieveTask =
                this.healthTrafficClientV2.RetrieveTrafficSnapshotV2Async(
                    GetRandomTrafficPeriodV2(), GetRandomDateTimeOffset(), randomCancellationToken);

            HealthTrafficClientV2DependencyException actualException =
                await Assert.ThrowsAsync<HealthTrafficClientV2DependencyException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthTrafficClientV2DependencyException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveTrafficSnapshotV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveTrafficSnapshotIfUnexpectedErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var someXeption = new Xeption(message: GetRandomString());

            var expectedHealthTrafficClientV2ServiceException =
                new HealthTrafficClientV2ServiceException(
                    message: "Health client service error occurred, contact support.",
                    innerException: someXeption,
                    data: someXeption.Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveTrafficSnapshotV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(someXeption);

            // when
            ValueTask<TrafficSnapshotV2> retrieveTask =
                this.healthTrafficClientV2.RetrieveTrafficSnapshotV2Async(
                    GetRandomTrafficPeriodV2(), GetRandomDateTimeOffset(), randomCancellationToken);

            HealthTrafficClientV2ServiceException actualException =
                await Assert.ThrowsAsync<HealthTrafficClientV2ServiceException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthTrafficClientV2ServiceException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveTrafficSnapshotV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveTrafficSnapshotAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var operationCanceledException =
                new OperationCanceledException();

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveTrafficSnapshotV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<TrafficSnapshotV2> retrieveTask =
                this.healthTrafficClientV2.RetrieveTrafficSnapshotV2Async(
                    GetRandomTrafficPeriodV2(), GetRandomDateTimeOffset(), randomCancellationToken);

            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(operationCanceledException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveTrafficSnapshotV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
