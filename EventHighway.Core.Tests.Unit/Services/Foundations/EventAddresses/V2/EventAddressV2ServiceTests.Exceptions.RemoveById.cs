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
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventAddresses.V2
{
    public partial class EventAddressV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveByIdIfSqlErrorOccursAndLogItAsync()
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
            ValueTask<EventAddressV2> removeEventAddressV2ByIdTask =
                this.eventAddressV2Service.RemoveEventAddressV2ByIdAsync(
                    someEventAddressV2Id, randomCancellationToken);

            EventAddressV2DependencyException actualEventAddressV2DependencyException =
                await Assert.ThrowsAsync<EventAddressV2DependencyException>(
                    removeEventAddressV2ByIdTask.AsTask);

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
        public async Task ShouldThrowDependencyValidationErrorOnRemoveByIdIfDbUpdateConcurrencyErrorAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventAddressV2Id = GetRandomId();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();
            dbUpdateConcurrencyException.Data.Add("ErrorCode", new List<string> { "DbUpdateConcurrencyError" });

            var lockedEventAddressV2Exception =
                new LockedEventAddressV2Exception(
                    message: "Event address is locked, try again.",
                    innerException: dbUpdateConcurrencyException,
                    data: dbUpdateConcurrencyException.Data);

            var expectedEventAddressV2DependencyValidationException =
                new EventAddressV2DependencyValidationException(
                    message: "Event address validation error occurred, fix the errors and try again.",
                    innerException: lockedEventAddressV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventAddressV2ByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<EventAddressV2> removeEventAddressV2ByIdTask =
                this.eventAddressV2Service.RemoveEventAddressV2ByIdAsync(
                    someEventAddressV2Id, randomCancellationToken);

            EventAddressV2DependencyValidationException actualEventAddressV2DependencyValidationException =
                await Assert.ThrowsAsync<EventAddressV2DependencyValidationException>(
                    removeEventAddressV2ByIdTask.AsTask);

            // then
            actualEventAddressV2DependencyValidationException.Should()
                .BeEquivalentTo(expectedEventAddressV2DependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV2DependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnRemoveByIdDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            Guid someEventAddressV2Id = GetRandomId();
            var dbUpdateException = new DbUpdateException();
            dbUpdateException.Data.Add("ErrorCode", new List<string> { "DatabaseUpdateError" });

            var failedStorageEventAddressV2Exception =
                new FailedStorageEventAddressV2Exception(
                    message: "Failed event address storage error occurred, contact support.",
                    innerException: dbUpdateException,
                    data: dbUpdateException.Data);

            var expectedEventAddressV2DependencyException =
                new EventAddressV2DependencyException(
                    message: "Event address dependency error occurred, contact support.",
                    innerException: failedStorageEventAddressV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventAddressV2ByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<EventAddressV2> removeEventAddressV2ByIdTask =
                this.eventAddressV2Service.RemoveEventAddressV2ByIdAsync(
                    someEventAddressV2Id, TestContext.Current.CancellationToken);

            EventAddressV2DependencyException actualEventAddressV2DependencyException =
                await Assert.ThrowsAsync<EventAddressV2DependencyException>(
                    removeEventAddressV2ByIdTask.AsTask);

            // then
            actualEventAddressV2DependencyException.Should()
                .BeEquivalentTo(expectedEventAddressV2DependencyException);

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
        public async Task ShouldThrowServiceExceptionOnRemoveByIdIfExceptionOccursAndLogItAsync()
        {
            // given
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
            ValueTask<EventAddressV2> removeEventAddressV2ByIdTask =
                this.eventAddressV2Service.RemoveEventAddressV2ByIdAsync(
                    someEventAddressV2Id, TestContext.Current.CancellationToken);

            EventAddressV2ServiceException actualEventAddressV2ServiceException =
                await Assert.ThrowsAsync<EventAddressV2ServiceException>(
                    removeEventAddressV2ByIdTask.AsTask);

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
    }
}
