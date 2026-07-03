// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.HealthChecks.V2.Exceptions;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Orchestrations.AddressSummaries.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.HealthChecks.V2
{
    public partial class HealthAddressClientV2Tests
    {
        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveEventAddressSummaryIfDependencyErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyError" });

            var addressSummaryV2OrchestrationDependencyException =
                new AddressSummaryV2OrchestrationDependencyException(
                    someMessage,
                    someInnerException);

            var expectedHealthAddressClientV2DependencyException =
                new HealthAddressClientV2DependencyException(
                    message: "Health client dependency error occurred, contact support.",
                    innerException: addressSummaryV2OrchestrationDependencyException.InnerException as Xeption,
                    data: (addressSummaryV2OrchestrationDependencyException.InnerException as Xeption).Data);

            this.addressSummaryV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveEventAddressSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(addressSummaryV2OrchestrationDependencyException);

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

            this.addressSummaryV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventAddressSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.addressSummaryV2OrchestrationServiceMock.VerifyNoOtherCalls();
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

            var addressSummaryV2OrchestrationServiceException =
                new AddressSummaryV2OrchestrationServiceException(
                    someMessage,
                    someInnerException);

            var expectedHealthAddressClientV2DependencyException =
                new HealthAddressClientV2DependencyException(
                    message: "Health client dependency error occurred, contact support.",
                    innerException: addressSummaryV2OrchestrationServiceException.InnerException as Xeption,
                    data: (addressSummaryV2OrchestrationServiceException.InnerException as Xeption).Data);

            this.addressSummaryV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveEventAddressSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(addressSummaryV2OrchestrationServiceException);

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

            this.addressSummaryV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventAddressSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.addressSummaryV2OrchestrationServiceMock.VerifyNoOtherCalls();
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

            this.addressSummaryV2OrchestrationServiceMock.Setup(service =>
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

            this.addressSummaryV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventAddressSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.addressSummaryV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveEventAddressSummaryAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var operationCanceledException =
                new OperationCanceledException();

            this.addressSummaryV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveEventAddressSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IEnumerable<EventAddressSummaryV2>> retrieveTask =
                this.healthAddressClientV2.RetrieveEventAddressSummaryV2Async(
                    GetRandomTrafficPeriodV2(), GetRandomDateTimeOffset(), randomCancellationToken);

            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(operationCanceledException);

            this.addressSummaryV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventAddressSummaryV2Async(
                    It.IsAny<TrafficPeriodV2>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.addressSummaryV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
