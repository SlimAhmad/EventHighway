// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.Events.V2
{
    public partial class EventV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventV2Id = GetRandomId();
            SqlException sqlException = GetSqlException();
            sqlException.Data.Add("ErrorCode", new List<string> { "SqlError" });

            var failedStorageEventV2Exception =
                new FailedStorageEventV2Exception(
                    message: "Failed event storage error occurred, contact support.",
                    innerException: sqlException,
                    data: sqlException.Data);

            var expectedEventV2DependencyException =
                new EventV2DependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: failedStorageEventV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventV2ByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<EventV2> removeEventV2ByIdTask =
                this.eventV2Service.RemoveEventV2ByIdAsync(
                    someEventV2Id, randomCancellationToken);

            EventV2DependencyException actualEventV2DependencyException =
                await Assert.ThrowsAsync<EventV2DependencyException>(
                    removeEventV2ByIdTask.AsTask);

            // then
            actualEventV2DependencyException.Should()
                .BeEquivalentTo(expectedEventV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventV2ByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationErrorOnRemoveByIdIfDbUpdateConcurrencyErrorAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventV2Id = GetRandomId();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();
            dbUpdateConcurrencyException.Data.Add("ErrorCode", new List<string> { "DbUpdateConcurrencyError" });

            var lockedEventV2Exception =
                new LockedEventV2Exception(
                    message: "Event is locked, try again.",
                    innerException: dbUpdateConcurrencyException,
                    data: dbUpdateConcurrencyException.Data);

            var expectedEventV2DependencyValidationException =
                new EventV2DependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: lockedEventV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventV2ByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<EventV2> removeEventV2ByIdTask =
                this.eventV2Service.RemoveEventV2ByIdAsync(
                    someEventV2Id, randomCancellationToken);

            EventV2DependencyValidationException actualEventV2DependencyValidationException =
                await Assert.ThrowsAsync<EventV2DependencyValidationException>(
                    removeEventV2ByIdTask.AsTask);

            // then
            actualEventV2DependencyValidationException.Should()
                .BeEquivalentTo(expectedEventV2DependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2DependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnRemoveByIdDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventV2Id = GetRandomId();
            var dbUpdateException = new DbUpdateException();
            dbUpdateException.Data.Add("ErrorCode", new List<string> { "DatabaseUpdateError" });

            var failedStorageEventV2Exception =
                new FailedStorageEventV2Exception(
                    message: "Failed event storage error occurred, contact support.",
                    innerException: dbUpdateException,
                    data: dbUpdateException.Data);

            var expectedEventV2DependencyException =
                new EventV2DependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: failedStorageEventV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventV2ByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<EventV2> removeEventV2ByIdTask =
                this.eventV2Service.RemoveEventV2ByIdAsync(someEventV2Id, randomCancellationToken);

            EventV2DependencyException actualEventV2DependencyException =
                await Assert.ThrowsAsync<EventV2DependencyException>(
                    removeEventV2ByIdTask.AsTask);

            // then
            actualEventV2DependencyException.Should()
                .BeEquivalentTo(expectedEventV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveByIdIfTimeoutOccursAndLogItAsync()
        {
            // given
            Guid someEventV2Id = GetRandomId();
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutEventV2Exception =
                new TimeoutEventV2Exception(
                    message: "Failed event timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedEventV2DependencyException =
                new EventV2DependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: timeoutEventV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventV2ByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<EventV2> removeEventV2ByIdTask =
                this.eventV2Service.RemoveEventV2ByIdAsync(
                    someEventV2Id, TestContext.Current.CancellationToken);

            EventV2DependencyException actualEventV2DependencyException =
                await Assert.ThrowsAsync<EventV2DependencyException>(
                    removeEventV2ByIdTask.AsTask);

            // then
            actualEventV2DependencyException.Should().BeEquivalentTo(
                expectedEventV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventV2ByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRemoveByIdAsync()
        {
            // given
            Guid someEventV2Id = GetRandomId();

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<EventV2> removeEventV2ByIdTask =
                this.eventV2Service.RemoveEventV2ByIdAsync(someEventV2Id, cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    removeEventV2ByIdTask.AsTask);

            actualException.Should().NotBeOfType<EventV2DependencyException>();
            actualException.Should().NotBeOfType<EventV2ServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
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

            Guid someEventV2Id = GetRandomId();
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventV2ServiceException =
                new FailedEventV2ServiceException(
                    message: "Failed event service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventV2ServiceException =
                new EventV2ServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: failedEventV2ServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventV2ByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<EventV2> removeEventV2ByIdTask =
                this.eventV2Service.RemoveEventV2ByIdAsync(
                    someEventV2Id, randomCancellationToken);

            EventV2ServiceException actualEventV2ServiceException =
                await Assert.ThrowsAsync<EventV2ServiceException>(
                    removeEventV2ByIdTask.AsTask);

            // then
            actualEventV2ServiceException.Should()
                .BeEquivalentTo(expectedEventV2ServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventV2ByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
