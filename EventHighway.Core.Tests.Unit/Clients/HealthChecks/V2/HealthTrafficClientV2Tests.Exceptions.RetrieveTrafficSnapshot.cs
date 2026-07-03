// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.HealthChecks.V2.Exceptions;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Processings.Traffics.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.HealthChecks.V2
{
    public partial class HealthTrafficClientV2Tests
    {
        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveTrafficSnapshotIfDependencyErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyError" });

            var trafficV2ProcessingDependencyException =
                new TrafficV2ProcessingDependencyException(
                    someMessage,
                    someInnerException);

            var expectedHealthTrafficClientV2DependencyException =
                new HealthTrafficClientV2DependencyException(
                    message: "Health client dependency error occurred, contact support.",
                    innerException: trafficV2ProcessingDependencyException.InnerException as Xeption,
                    data: (trafficV2ProcessingDependencyException.InnerException as Xeption).Data);

            this.trafficV2ProcessingServiceMock.Setup(service =>
                service.RetrieveTrafficSnapshotV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(trafficV2ProcessingDependencyException);

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

            this.trafficV2ProcessingServiceMock.Verify(service =>
                service.RetrieveTrafficSnapshotV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.trafficV2ProcessingServiceMock.VerifyNoOtherCalls();
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

            var trafficV2ProcessingServiceException =
                new TrafficV2ProcessingServiceException(
                    someMessage,
                    someInnerException);

            var expectedHealthTrafficClientV2DependencyException =
                new HealthTrafficClientV2DependencyException(
                    message: "Health client dependency error occurred, contact support.",
                    innerException: trafficV2ProcessingServiceException.InnerException as Xeption,
                    data: (trafficV2ProcessingServiceException.InnerException as Xeption).Data);

            this.trafficV2ProcessingServiceMock.Setup(service =>
                service.RetrieveTrafficSnapshotV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(trafficV2ProcessingServiceException);

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

            this.trafficV2ProcessingServiceMock.Verify(service =>
                service.RetrieveTrafficSnapshotV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.trafficV2ProcessingServiceMock.VerifyNoOtherCalls();
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

            this.trafficV2ProcessingServiceMock.Setup(service =>
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

            this.trafficV2ProcessingServiceMock.Verify(service =>
                service.RetrieveTrafficSnapshotV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.trafficV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveTrafficSnapshotAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var operationCanceledException =
                new OperationCanceledException();

            this.trafficV2ProcessingServiceMock.Setup(service =>
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

            this.trafficV2ProcessingServiceMock.Verify(service =>
                service.RetrieveTrafficSnapshotV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.trafficV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
