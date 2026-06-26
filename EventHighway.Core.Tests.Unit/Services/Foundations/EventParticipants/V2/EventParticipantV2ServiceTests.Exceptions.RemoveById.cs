// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventParticipants.V2
{
    public partial class EventParticipantV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveByIdIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventParticipantV2Id = GetRandomId();
            SqlException sqlException = CreateSqlException();
            sqlException.Data.Add("ErrorCode", new List<string> { "SqlError" });

            var failedStorageEventParticipantV2Exception =
                new FailedStorageEventParticipantV2Exception(
                    message: "Failed event participant storage error occurred, contact support.",
                    innerException: sqlException,
                    data: sqlException.Data);

            var expectedEventParticipantV2DependencyException =
                new EventParticipantV2DependencyException(
                    message: "Event participant dependency error occurred, contact support.",
                    innerException: failedStorageEventParticipantV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventParticipantV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(sqlException);

            // when
            ValueTask<EventParticipantV2> removeEventParticipantV2ByIdTask =
                this.eventParticipantV2Service.RemoveEventParticipantV2ByIdAsync(
                    someEventParticipantV2Id, randomCancellationToken);

            EventParticipantV2DependencyException actualEventParticipantV2DependencyException =
                await Assert.ThrowsAsync<EventParticipantV2DependencyException>(
                    removeEventParticipantV2ByIdTask.AsTask);

            // then
            actualEventParticipantV2DependencyException.Should()
                .BeEquivalentTo(expectedEventParticipantV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventParticipantV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveByIdIfTimeoutOccursAndLogItAsync()
        {
            // given
            Guid someEventParticipantV2Id = GetRandomId();
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutEventParticipantV2Exception =
                new TimeoutEventParticipantV2Exception(
                    message: "Failed event participant timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedEventParticipantV2DependencyException =
                new EventParticipantV2DependencyException(
                    message: "Event participant dependency error occurred, contact support.",
                    innerException: timeoutEventParticipantV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventParticipantV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<EventParticipantV2> removeEventParticipantV2ByIdTask =
                this.eventParticipantV2Service.RemoveEventParticipantV2ByIdAsync(
                    someEventParticipantV2Id, TestContext.Current.CancellationToken);

            EventParticipantV2DependencyException actualEventParticipantV2DependencyException =
                await Assert.ThrowsAsync<EventParticipantV2DependencyException>(
                    removeEventParticipantV2ByIdTask.AsTask);

            // then
            actualEventParticipantV2DependencyException.Should().BeEquivalentTo(
                expectedEventParticipantV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventParticipantV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRemoveByIdAsync()
        {
            // given
            Guid someEventParticipantV2Id = GetRandomId();
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<EventParticipantV2> removeEventParticipantV2ByIdTask =
                this.eventParticipantV2Service.RemoveEventParticipantV2ByIdAsync(
                    someEventParticipantV2Id, cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    removeEventParticipantV2ByIdTask.AsTask);

            actualException.Should().NotBeOfType<EventParticipantV2DependencyException>();
            actualException.Should().NotBeOfType<EventParticipantV2ServiceException>();
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

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveByIdIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventParticipantV2Id = GetRandomId();
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventParticipantV2ServiceException =
                new FailedEventParticipantV2ServiceException(
                    message: "Failed event participant service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventParticipantV2ServiceException =
                new EventParticipantV2ServiceException(
                    message: "Event participant service error occurred, contact support.",
                    innerException: failedEventParticipantV2ServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventParticipantV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<EventParticipantV2> removeEventParticipantV2ByIdTask =
                this.eventParticipantV2Service.RemoveEventParticipantV2ByIdAsync(
                    someEventParticipantV2Id, randomCancellationToken);

            EventParticipantV2ServiceException actualEventParticipantV2ServiceException =
                await Assert.ThrowsAsync<EventParticipantV2ServiceException>(
                    removeEventParticipantV2ByIdTask.AsTask);

            // then
            actualEventParticipantV2ServiceException.Should()
                .BeEquivalentTo(expectedEventParticipantV2ServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventParticipantV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2ServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
