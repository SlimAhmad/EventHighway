// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEvents.V2
{
    public partial class ListenerEventV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someListenerEventV2Id = GetRandomId();
            SqlException sqlException = GetSqlException();
            sqlException.Data.Add("ErrorCode", new List<string> { "SqlError" });

            var failedStorageListenerEventV2Exception =
                new FailedStorageListenerEventV2Exception(
                    message: "Failed listener event storage error occurred, contact support.",
                    innerException: sqlException,
                    data: sqlException.Data);

            var expectedListenerEventV2DependencyException =
                new ListenerEventV2DependencyException(
                    message: "Listener event dependency error occurred, contact support.",
                    innerException: failedStorageListenerEventV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<ListenerEventV2> removeListenerEventV2ByIdTask =
                this.listenerEventV2Service.RemoveListenerEventV2ByIdAsync(
                    someListenerEventV2Id,
                    randomCancellationToken);

            ListenerEventV2DependencyException actualListenerEventV2DependencyException =
                await Assert.ThrowsAsync<ListenerEventV2DependencyException>(
                    removeListenerEventV2ByIdTask.AsTask);

            // then
            actualListenerEventV2DependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2DependencyException))),
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

            Guid someListenerEventV2Id = GetRandomId();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();

            dbUpdateConcurrencyException.Data.Add(
                "ErrorCode",
                new List<string> { "DbUpdateConcurrencyError" });

            var lockedListenerEventV2Exception =
                new LockedListenerEventV2Exception(
                    message: "Listener event is locked, try again.",
                    innerException: dbUpdateConcurrencyException,
                    data: dbUpdateConcurrencyException.Data);

            var expectedListenerEventV2DependencyValidationException =
                new ListenerEventV2DependencyValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: lockedListenerEventV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    randomCancellationToken))
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<ListenerEventV2> removeListenerEventV2ByIdTask =
                this.listenerEventV2Service.RemoveListenerEventV2ByIdAsync(
                    someListenerEventV2Id,
                    randomCancellationToken);

            ListenerEventV2DependencyValidationException actualListenerEventV2DependencyValidationException =
                await Assert.ThrowsAsync<ListenerEventV2DependencyValidationException>(
                    removeListenerEventV2ByIdTask.AsTask);

            // then
            actualListenerEventV2DependencyValidationException.Should()
                .BeEquivalentTo(expectedListenerEventV2DependencyValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    randomCancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2DependencyValidationException))),
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

            Guid someListenerEventV2Id = GetRandomId();
            var dbUpdateException = new DbUpdateException();

            dbUpdateException.Data.Add(
                "ErrorCode",
                new List<string> { "DatabaseUpdateError" });

            var failedStorageListenerEventV2Exception =
                new FailedStorageListenerEventV2Exception(
                    message: "Failed listener event storage error occurred, contact support.",
                    innerException: dbUpdateException,
                    data: dbUpdateException.Data);

            var expectedListenerEventV2DependencyException =
                new ListenerEventV2DependencyException(
                    message: "Listener event dependency error occurred, contact support.",
                    innerException: failedStorageListenerEventV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    randomCancellationToken))
                    .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<ListenerEventV2> removeListenerEventV2ByIdTask =
                this.listenerEventV2Service.RemoveListenerEventV2ByIdAsync(
                    someListenerEventV2Id,
                    randomCancellationToken);

            ListenerEventV2DependencyException actualListenerEventV2DependencyException =
                await Assert.ThrowsAsync<ListenerEventV2DependencyException>(
                    removeListenerEventV2ByIdTask.AsTask);

            // then
            actualListenerEventV2DependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    randomCancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2DependencyException))),
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

            Guid someListenerEventV2Id = GetRandomId();
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedListenerEventV2ServiceException =
                new FailedListenerEventV2ServiceException(
                    message: "Failed listener event service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedListenerEventV2ServiceException =
                new ListenerEventV2ServiceException(
                    message: "Listener event service error occurred, contact support.",
                    innerException: failedListenerEventV2ServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<ListenerEventV2> removeListenerEventV2ByIdTask =
                this.listenerEventV2Service.RemoveListenerEventV2ByIdAsync(
                    someListenerEventV2Id,
                    randomCancellationToken);

            ListenerEventV2ServiceException actualListenerEventV2ServiceException =
                await Assert.ThrowsAsync<ListenerEventV2ServiceException>(
                    removeListenerEventV2ByIdTask.AsTask);

            // then
            actualListenerEventV2ServiceException.Should()
                .BeEquivalentTo(expectedListenerEventV2ServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2ServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveByIdIfTimeoutOccursAndLogItAsync()
        {
            // given
            Guid someListenerEventV2Id = GetRandomId();
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutListenerEventV2Exception =
                new TimeoutListenerEventV2Exception(
                    message: "Failed listener event timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedListenerEventV2DependencyException =
                new ListenerEventV2DependencyException(
                    message: "Listener event dependency error occurred, contact support.",
                    innerException: timeoutListenerEventV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<ListenerEventV2> removeListenerEventV2ByIdTask =
                this.listenerEventV2Service.RemoveListenerEventV2ByIdAsync(
                    someListenerEventV2Id,
                    TestContext.Current.CancellationToken);

            ListenerEventV2DependencyException actualListenerEventV2DependencyException =
                await Assert.ThrowsAsync<ListenerEventV2DependencyException>(
                    removeListenerEventV2ByIdTask.AsTask);

            // then
            actualListenerEventV2DependencyException.Should().BeEquivalentTo(
                expectedListenerEventV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRemoveByIdAsync()
        {
            // given
            Guid someListenerEventV2Id = GetRandomId();

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<ListenerEventV2> removeListenerEventV2ByIdTask =
                this.listenerEventV2Service.RemoveListenerEventV2ByIdAsync(
                    someListenerEventV2Id,
                    cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    removeListenerEventV2ByIdTask.AsTask);

            actualException.Should().NotBeOfType<ListenerEventV2DependencyException>();
            actualException.Should().NotBeOfType<ListenerEventV2ServiceException>();
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
    }
}
