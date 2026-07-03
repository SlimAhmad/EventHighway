// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Services.Orchestrations.AddressSummaries.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.AddressSummaries.V2
{
    public partial class AddressSummaryV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveEventAddressSummaryAsync()
        {
            // given
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<IEnumerable<EventAddressSummaryV2>> retrieveEventAddressSummaryTask =
                this.addressSummaryV2OrchestrationService
                    .RetrieveEventAddressSummaryV2Async(
                        TrafficPeriodV2.Day, GetRandomDateTimeOffset(), cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveEventAddressSummaryTask.AsTask);

            actualException.Should().NotBeOfType<AddressSummaryV2OrchestrationDependencyException>();
            actualException.Should().NotBeOfType<AddressSummaryV2OrchestrationServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveEventAddressSummaryIfTimeoutOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutAddressSummaryV2OrchestrationException =
                new TimeoutAddressSummaryV2OrchestrationException(
                    message: "Failed address summary orchestration timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedAddressSummaryV2OrchestrationDependencyException =
                new AddressSummaryV2OrchestrationDependencyException(
                    message: "Address summary dependency error occurred, contact support.",
                    innerException: timeoutAddressSummaryV2OrchestrationException);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IEnumerable<EventAddressSummaryV2>> retrieveEventAddressSummaryTask =
                this.addressSummaryV2OrchestrationService
                    .RetrieveEventAddressSummaryV2Async(
                        TrafficPeriodV2.Day, GetRandomDateTimeOffset(), randomCancellationToken);

            AddressSummaryV2OrchestrationDependencyException
                actualAddressSummaryV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<AddressSummaryV2OrchestrationDependencyException>(
                        retrieveEventAddressSummaryTask.AsTask);

            // then
            actualAddressSummaryV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedAddressSummaryV2OrchestrationDependencyException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAddressSummaryV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveEventAddressSummaryIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedAddressSummaryV2OrchestrationDependencyException =
                new AddressSummaryV2OrchestrationDependencyException(
                    message: "Address summary dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<IEnumerable<EventAddressSummaryV2>> retrieveEventAddressSummaryTask =
                this.addressSummaryV2OrchestrationService
                    .RetrieveEventAddressSummaryV2Async(
                        TrafficPeriodV2.Day, GetRandomDateTimeOffset(), randomCancellationToken);

            AddressSummaryV2OrchestrationDependencyException
                actualAddressSummaryV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<AddressSummaryV2OrchestrationDependencyException>(
                        retrieveEventAddressSummaryTask.AsTask);

            // then
            actualAddressSummaryV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedAddressSummaryV2OrchestrationDependencyException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAddressSummaryV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveEventAddressSummaryIfServiceErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var exception = new Exception();
            exception.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedAddressSummaryV2OrchestrationServiceException =
                new FailedAddressSummaryV2OrchestrationServiceException(
                    message: "Failed address summary service error occurred, contact support.",
                    innerException: exception,
                    data: exception.Data);

            var expectedAddressSummaryV2OrchestrationServiceException =
                new AddressSummaryV2OrchestrationServiceException(
                    message: "Address summary service error occurred, contact support.",
                    innerException: failedAddressSummaryV2OrchestrationServiceException);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(exception);

            // when
            ValueTask<IEnumerable<EventAddressSummaryV2>> retrieveEventAddressSummaryTask =
                this.addressSummaryV2OrchestrationService
                    .RetrieveEventAddressSummaryV2Async(
                        TrafficPeriodV2.Day, GetRandomDateTimeOffset(), randomCancellationToken);

            AddressSummaryV2OrchestrationServiceException
                actualAddressSummaryV2OrchestrationServiceException =
                    await Assert.ThrowsAsync<AddressSummaryV2OrchestrationServiceException>(
                        retrieveEventAddressSummaryTask.AsTask);

            // then
            actualAddressSummaryV2OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedAddressSummaryV2OrchestrationServiceException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.RetrieveAllEventAddressV2sWithEventListenerV2sAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedAddressSummaryV2OrchestrationServiceException))),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
