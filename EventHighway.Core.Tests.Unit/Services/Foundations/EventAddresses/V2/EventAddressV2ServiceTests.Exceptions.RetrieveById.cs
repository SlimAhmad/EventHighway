// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventAddresses.V2
{
    public partial class EventAddressV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventAddressV2Id = GetRandomId();
            SqlException sqlException = CreateSqlException();
            sqlException.Data.Add("ErrorCode", new List<string> { "SqlError" });

            var failedStorageEventAddressV2Exception =
                new FailedStorageEventAddressV2Exception(
                    message: "Failed event address storage error occurred, contact support.",
                    innerException: sqlException,
                    data: sqlException.Data);

            var expectedEventAddressV2DependencyException =
                new EventAddressV2DependencyException(
                    message: "Event address dependency error occurred, contact support.",
                    innerException: failedStorageEventAddressV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventAddressV2ByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<EventAddressV2> retrieveEventAddressV2ByIdTask =
                this.eventAddressV2Service.RetrieveEventAddressV2ByIdAsync(
                    someEventAddressV2Id, randomCancellationToken);

            EventAddressV2DependencyException actualEventAddressV2DependencyException =
                await Assert.ThrowsAsync<EventAddressV2DependencyException>(
                    retrieveEventAddressV2ByIdTask.AsTask);

            // then
            actualEventAddressV2DependencyException.Should()
                .BeEquivalentTo(expectedEventAddressV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventAddressV2Id = GetRandomId();
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventAddressV2ServiceException =
                new FailedEventAddressV2ServiceException(
                    message: "Failed event address service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventAddressV2ServiceException =
                new EventAddressV2ServiceException(
                    message: "Event address service error occurred, contact support.",
                    innerException: failedEventAddressV2ServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventAddressV2ByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<EventAddressV2> retrieveEventAddressV2ByIdTask =
                this.eventAddressV2Service.RetrieveEventAddressV2ByIdAsync(
                    someEventAddressV2Id, randomCancellationToken);

            EventAddressV2ServiceException actualEventAddressV2ServiceException =
                await Assert.ThrowsAsync<EventAddressV2ServiceException>(
                    retrieveEventAddressV2ByIdTask.AsTask);

            // then
            actualEventAddressV2ServiceException.Should()
                .BeEquivalentTo(expectedEventAddressV2ServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV2ServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveByIdIfTimeoutOccursAndLogItAsync()
        {
            // given
            Guid someEventAddressV2Id = GetRandomId();
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutEventAddressV2Exception =
                new TimeoutEventAddressV2Exception(
                    message: "Failed event address timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedEventAddressV2DependencyException =
                new EventAddressV2DependencyException(
                    message: "Event address dependency error occurred, contact support.",
                    innerException: timeoutEventAddressV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventAddressV2ByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<EventAddressV2> retrieveEventAddressV2ByIdTask =
                this.eventAddressV2Service.RetrieveEventAddressV2ByIdAsync(
                    someEventAddressV2Id, TestContext.Current.CancellationToken);

            EventAddressV2DependencyException actualEventAddressV2DependencyException =
                await Assert.ThrowsAsync<EventAddressV2DependencyException>(
                    retrieveEventAddressV2ByIdTask.AsTask);

            // then
            actualEventAddressV2DependencyException.Should().BeEquivalentTo(
                expectedEventAddressV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveByIdAsync()
        {
            // given
            Guid someEventAddressV2Id = GetRandomId();
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<EventAddressV2> retrieveEventAddressV2ByIdTask =
                this.eventAddressV2Service.RetrieveEventAddressV2ByIdAsync(
                    someEventAddressV2Id, cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveEventAddressV2ByIdTask.AsTask);

            actualException.Should().NotBeOfType<EventAddressV2DependencyException>();
            actualException.Should().NotBeOfType<EventAddressV2ServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeptions.Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeptions.Xeption>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
