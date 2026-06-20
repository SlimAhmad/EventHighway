// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventArchives.V2
{
    public partial class EventArchiveV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventArchiveV2Id = GetRandomId();
            SqlException sqlException = CreateSqlException();

            sqlException.Data.Add(
                key: nameof(SqlException.Number),
                value: new List<string> { "Some SQL error code" });

            var failedStorageEventArchiveV2Exception =
                new FailedStorageEventArchiveV2Exception(
                    message: "Failed event archive storage error occurred, contact support.",
                    innerException: sqlException,
                    data: sqlException.Data);

            var expectedEventArchiveV2DependencyException =
                new EventArchiveV2DependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: failedStorageEventArchiveV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventArchiveV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(sqlException);

            // when
            ValueTask<EventArchiveV2> removeEventArchiveV2ByIdTask =
                this.eventArchiveV2Service.RemoveEventArchiveV2ByIdAsync(
                    someEventArchiveV2Id,
                    randomCancellationToken);

            EventArchiveV2DependencyException actualEventArchiveV2DependencyException =
                await Assert.ThrowsAsync<EventArchiveV2DependencyException>(
                    removeEventArchiveV2ByIdTask.AsTask);

            // then
            actualEventArchiveV2DependencyException.Should()
                .BeEquivalentTo(expectedEventArchiveV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventArchiveV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2DependencyException))),
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

            Guid someEventArchiveV2Id = GetRandomId();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();

            dbUpdateConcurrencyException.Data.Add(
                "ErrorCode",
                new List<string> { "DbUpdateConcurrencyError" });

            var lockedEventArchiveV2Exception =
                new LockedEventArchiveV2Exception(
                    message: "Event archive is locked, try again.",
                    innerException: dbUpdateConcurrencyException,
                    data: dbUpdateConcurrencyException.Data);

            var expectedEventArchiveV2DependencyValidationException =
                new EventArchiveV2DependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: lockedEventArchiveV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventArchiveV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<EventArchiveV2> removeEventArchiveV2ByIdTask =
                this.eventArchiveV2Service.RemoveEventArchiveV2ByIdAsync(
                    someEventArchiveV2Id,
                    randomCancellationToken);

            EventArchiveV2DependencyValidationException actualEventArchiveV2DependencyValidationException =
                await Assert.ThrowsAsync<EventArchiveV2DependencyValidationException>(
                    removeEventArchiveV2ByIdTask.AsTask);

            // then
            actualEventArchiveV2DependencyValidationException.Should()
                .BeEquivalentTo(expectedEventArchiveV2DependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventArchiveV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2DependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnRemoveByIdIfDbUpdateExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventArchiveV2Id = GetRandomId();
            var dbUpdateException = new DbUpdateException();

            dbUpdateException.Data.Add(
                "ErrorCode",
                new List<string> { "DatabaseUpdateError" });

            var failedStorageEventArchiveV2Exception =
                new FailedStorageEventArchiveV2Exception(
                    message: "Failed event archive storage error occurred, contact support.",
                    innerException: dbUpdateException,
                    data: dbUpdateException.Data);

            var expectedEventArchiveV2DependencyException =
                new EventArchiveV2DependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: failedStorageEventArchiveV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventArchiveV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<EventArchiveV2> removeEventArchiveV2ByIdTask =
                this.eventArchiveV2Service.RemoveEventArchiveV2ByIdAsync(
                    someEventArchiveV2Id,
                    randomCancellationToken);

            EventArchiveV2DependencyException actualEventArchiveV2DependencyException =
                await Assert.ThrowsAsync<EventArchiveV2DependencyException>(
                    removeEventArchiveV2ByIdTask.AsTask);

            // then
            actualEventArchiveV2DependencyException.Should()
                .BeEquivalentTo(expectedEventArchiveV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventArchiveV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2DependencyException))),
                        Times.Once);

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

            Guid someEventArchiveV2Id = GetRandomId();
            var serviceException = new Exception();

            serviceException.Data.Add(
                "ErrorCode",
                new List<string> { "ServiceError" });

            var failedEventArchiveV2ServiceException =
                new FailedEventArchiveV2ServiceException(
                    message: "Failed event archive service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventArchiveV2ServiceException =
                new EventArchiveV2ServiceException(
                    message: "Event archive service error occurred, contact support.",
                    innerException: failedEventArchiveV2ServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventArchiveV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<EventArchiveV2> removeEventArchiveV2ByIdTask =
                this.eventArchiveV2Service.RemoveEventArchiveV2ByIdAsync(
                    someEventArchiveV2Id,
                    randomCancellationToken);

            EventArchiveV2ServiceException actualEventArchiveV2ServiceException =
                await Assert.ThrowsAsync<EventArchiveV2ServiceException>(
                    removeEventArchiveV2ByIdTask.AsTask);

            // then
            actualEventArchiveV2ServiceException.Should()
                .BeEquivalentTo(expectedEventArchiveV2ServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventArchiveV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2ServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveByIdIfTimeoutOccursAndLogItAsync()
        {
            // given
            Guid someEventArchiveV2Id = GetRandomId();
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutEventArchiveV2Exception =
                new TimeoutEventArchiveV2Exception(
                    message: "Failed event archive timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedEventArchiveV2DependencyException =
                new EventArchiveV2DependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: timeoutEventArchiveV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventArchiveV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<EventArchiveV2> removeEventArchiveV2ByIdTask =
                this.eventArchiveV2Service.RemoveEventArchiveV2ByIdAsync(
                    someEventArchiveV2Id, TestContext.Current.CancellationToken);

            EventArchiveV2DependencyException actualEventArchiveV2DependencyException =
                await Assert.ThrowsAsync<EventArchiveV2DependencyException>(
                    removeEventArchiveV2ByIdTask.AsTask);

            // then
            actualEventArchiveV2DependencyException.Should().BeEquivalentTo(
                expectedEventArchiveV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventArchiveV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
