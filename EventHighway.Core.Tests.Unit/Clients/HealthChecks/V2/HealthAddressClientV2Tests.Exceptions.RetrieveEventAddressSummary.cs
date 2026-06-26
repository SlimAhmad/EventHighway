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
    public partial class HealthAddressClientV2Tests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowValidationExceptionOnRetrieveEventAddressSummaryIfValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedHealthAddressClientV2ValidationException =
                new HealthAddressClientV2ValidationException(
                    message: "Health client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption,
                    data: (validationException.InnerException as Xeption).Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveEventAddressSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<IEnumerable<EventAddressSummaryV2>> retrieveTask =
                this.healthAddressClientV2.RetrieveEventAddressSummaryV2Async(
                    GetRandomTrafficPeriodV2(), GetRandomDateTimeOffset(), randomCancellationToken);

            HealthAddressClientV2ValidationException actualException =
                await Assert.ThrowsAsync<HealthAddressClientV2ValidationException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthAddressClientV2ValidationException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveEventAddressSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveEventAddressSummaryIfDependencyErrorOccursAsync()
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

            var expectedHealthAddressClientV2DependencyException =
                new HealthAddressClientV2DependencyException(
                    message: "Health client dependency error occurred, contact support.",
                    innerException: healthV2CoordinationDependencyException.InnerException as Xeption,
                    data: (healthV2CoordinationDependencyException.InnerException as Xeption).Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveEventAddressSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(healthV2CoordinationDependencyException);

            // when
            ValueTask<IEnumerable<EventAddressSummaryV2>> retrieveTask =
                this.healthAddressClientV2.RetrieveEventAddressSummaryV2Async(
                    GetRandomTrafficPeriodV2(), GetRandomDateTimeOffset(), randomCancellationToken);

            HealthAddressClientV2DependencyException actualException =
                await Assert.ThrowsAsync<HealthAddressClientV2DependencyException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthAddressClientV2DependencyException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveEventAddressSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveEventAddressSummaryIfServiceErrorOccursAsync()
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

            var expectedHealthAddressClientV2DependencyException =
                new HealthAddressClientV2DependencyException(
                    message: "Health client dependency error occurred, contact support.",
                    innerException: healthV2CoordinationServiceException.InnerException as Xeption,
                    data: (healthV2CoordinationServiceException.InnerException as Xeption).Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveEventAddressSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(healthV2CoordinationServiceException);

            // when
            ValueTask<IEnumerable<EventAddressSummaryV2>> retrieveTask =
                this.healthAddressClientV2.RetrieveEventAddressSummaryV2Async(
                    GetRandomTrafficPeriodV2(), GetRandomDateTimeOffset(), randomCancellationToken);

            HealthAddressClientV2DependencyException actualException =
                await Assert.ThrowsAsync<HealthAddressClientV2DependencyException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthAddressClientV2DependencyException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveEventAddressSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveEventAddressSummaryIfUnexpectedErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var someXeption = new Xeption(message: GetRandomString());

            var expectedHealthAddressClientV2ServiceException =
                new HealthAddressClientV2ServiceException(
                    message: "Health client service error occurred, contact support.",
                    innerException: someXeption,
                    data: someXeption.Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveEventAddressSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(someXeption);

            // when
            ValueTask<IEnumerable<EventAddressSummaryV2>> retrieveTask =
                this.healthAddressClientV2.RetrieveEventAddressSummaryV2Async(
                    GetRandomTrafficPeriodV2(), GetRandomDateTimeOffset(), randomCancellationToken);

            HealthAddressClientV2ServiceException actualException =
                await Assert.ThrowsAsync<HealthAddressClientV2ServiceException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthAddressClientV2ServiceException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveEventAddressSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
