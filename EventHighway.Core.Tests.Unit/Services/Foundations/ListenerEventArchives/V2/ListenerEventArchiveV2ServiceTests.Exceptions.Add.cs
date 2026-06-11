// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEventArchives.V2
{
    public partial class ListenerEventArchiveV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            ListenerEventArchiveV2 someListenerEventArchiveV2 = CreateRandomListenerEventArchiveV2();
            SqlException sqlException = CreateSqlException();

            sqlException.Data.Add(
                key: nameof(SqlException.Number),
                value: new List<string> { "Some SQL error code" });

            var failedStorageListenerEventArchiveV2Exception =
                new FailedStorageListenerEventArchiveV2Exception(
                    message: "Failed listener event archive storage error occurred, contact support.",
                    innerException: sqlException,
                    data: sqlException.Data);

            var expectedListenerEventArchiveV2DependencyException =
                new ListenerEventArchiveV2DependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: failedStorageListenerEventArchiveV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<ListenerEventArchiveV2> addListenerEventArchiveV2Task =
                this.listenerEventArchiveV2Service.AddListenerEventArchiveV2Async(
                    someListenerEventArchiveV2,
                    TestContext.Current.CancellationToken);

            ListenerEventArchiveV2DependencyException actualListenerEventArchiveV2DependencyException =
                await Assert.ThrowsAsync<ListenerEventArchiveV2DependencyException>(
                    addListenerEventArchiveV2Task.AsTask);

            // then
            actualListenerEventArchiveV2DependencyException.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV2DependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventArchiveV2Async(
                    It.IsAny<ListenerEventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfListenerEventArchiveV2AlreadyExistsAndLogItAsync()
        {
            // given
            string randomMessage = GetRandomString();
            ListenerEventArchiveV2 someListenerEventArchiveV2 = CreateRandomListenerEventArchiveV2();
            var duplicateKeyException = new DuplicateKeyException(randomMessage);

            duplicateKeyException.Data.Add(
                "ErrorCode",
                new List<string> { "DuplicateKeyError" });

            var alreadyExistsListenerEventArchiveV2Exception =
                new AlreadyExistsListenerEventArchiveV2Exception(
                    message: "Listener event archive with the same id already exists.",
                    innerException: duplicateKeyException,
                    data: duplicateKeyException.Data);

            var expectedListenerEventArchiveV2DependencyValidationException =
                new ListenerEventArchiveV2DependencyValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: alreadyExistsListenerEventArchiveV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<ListenerEventArchiveV2> addListenerEventArchiveV2Task =
                this.listenerEventArchiveV2Service.AddListenerEventArchiveV2Async(
                    someListenerEventArchiveV2,
                    TestContext.Current.CancellationToken);

            ListenerEventArchiveV2DependencyValidationException
                actualListenerEventArchiveV2DependencyValidationException =
                    await Assert.ThrowsAsync<ListenerEventArchiveV2DependencyValidationException>(
                        addListenerEventArchiveV2Task.AsTask);

            // then
            actualListenerEventArchiveV2DependencyValidationException.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV2DependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV2DependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventArchiveV2Async(
                    It.IsAny<ListenerEventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfReferenceErrorOccursAndLogItAsync()
        {
            // given
            ListenerEventArchiveV2 someListenerEventArchiveV2 = CreateRandomListenerEventArchiveV2();
            string someMessage = GetRandomString();

            var foreignKeyConstraintConflictException =
                new ForeignKeyConstraintConflictException(someMessage);

            foreignKeyConstraintConflictException.Data.Add(
                "ErrorCode",
                new List<string> { "ForeignKeyConstraintConflictExceptionError" });

            var invalidReferenceListenerEventArchiveV2Exception =
                new InvalidReferenceListenerEventArchiveV2Exception(
                    message: "Invalid listener event archive reference error occurred.",
                    innerException: foreignKeyConstraintConflictException,
                    data: foreignKeyConstraintConflictException.Data);

            var expectedListenerEventArchiveV2DependencyValidationException =
                new ListenerEventArchiveV2DependencyValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidReferenceListenerEventArchiveV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(foreignKeyConstraintConflictException);

            // when
            ValueTask<ListenerEventArchiveV2> addListenerEventArchiveV2Task =
                this.listenerEventArchiveV2Service.AddListenerEventArchiveV2Async(
                    someListenerEventArchiveV2,
                    TestContext.Current.CancellationToken);

            ListenerEventArchiveV2DependencyValidationException
                actualListenerEventArchiveV2DependencyValidationException =
                    await Assert.ThrowsAsync<ListenerEventArchiveV2DependencyValidationException>(
                        addListenerEventArchiveV2Task.AsTask);

            // then
            actualListenerEventArchiveV2DependencyValidationException.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV2DependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV2DependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventArchiveV2Async(
                    It.IsAny<ListenerEventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDbUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            ListenerEventArchiveV2 someListenerEventArchiveV2 = CreateRandomListenerEventArchiveV2();
            var dbUpdateConcurrencyException = new DbUpdateConcurrencyException();

            dbUpdateConcurrencyException.Data.Add(
                "ErrorCode",
                new List<string> { "DbUpdateConcurrencyError" });

            var lockedListenerEventArchiveV2Exception =
                new LockedListenerEventArchiveV2Exception(
                    message: "Listener event archive is locked, try again.",
                    innerException: dbUpdateConcurrencyException,
                    data: dbUpdateConcurrencyException.Data);

            var expectedListenerEventArchiveV2DependencyValidationException =
                new ListenerEventArchiveV2DependencyValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: lockedListenerEventArchiveV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateConcurrencyException);

            // when
            ValueTask<ListenerEventArchiveV2> addListenerEventArchiveV2Task =
                this.listenerEventArchiveV2Service.AddListenerEventArchiveV2Async(
                    someListenerEventArchiveV2,
                    TestContext.Current.CancellationToken);

            ListenerEventArchiveV2DependencyValidationException
                actualListenerEventArchiveV2DependencyValidationException =
                    await Assert.ThrowsAsync<ListenerEventArchiveV2DependencyValidationException>(
                        addListenerEventArchiveV2Task.AsTask);

            // then
            actualListenerEventArchiveV2DependencyValidationException.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV2DependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV2DependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventArchiveV2Async(
                    It.IsAny<ListenerEventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateExceptionOccursAndLogItAsync()
        {
            // given
            ListenerEventArchiveV2 someListenerEventArchiveV2 = CreateRandomListenerEventArchiveV2();
            var dbUpdateException = new DbUpdateException();

            dbUpdateException.Data.Add(
                "ErrorCode",
                new List<string> { "DbUpdateError" });

            var failedStorageListenerEventArchiveV2Exception =
                new FailedStorageListenerEventArchiveV2Exception(
                    message: "Failed listener event archive storage error occurred, contact support.",
                    innerException: dbUpdateException,
                    data: dbUpdateException.Data);

            var expectedListenerEventArchiveV2DependencyException =
                new ListenerEventArchiveV2DependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: failedStorageListenerEventArchiveV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<ListenerEventArchiveV2> addListenerEventArchiveV2Task =
                this.listenerEventArchiveV2Service.AddListenerEventArchiveV2Async(
                    someListenerEventArchiveV2,
                    TestContext.Current.CancellationToken);

            ListenerEventArchiveV2DependencyException actualListenerEventArchiveV2DependencyException =
                await Assert.ThrowsAsync<ListenerEventArchiveV2DependencyException>(
                    addListenerEventArchiveV2Task.AsTask);

            // then
            actualListenerEventArchiveV2DependencyException.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV2DependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventArchiveV2Async(
                    It.IsAny<ListenerEventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfExceptionOccursAndLogItAsync()
        {
            // given
            ListenerEventArchiveV2 someListenerEventArchiveV2 = CreateRandomListenerEventArchiveV2();
            var serviceException = new Exception();

            serviceException.Data.Add(
                "ErrorCode",
                new List<string> { "ServiceError" });

            var failedListenerEventArchiveV2ServiceException =
                new FailedListenerEventArchiveV2ServiceException(
                    message: "Failed listener event archive service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedListenerEventArchiveV2ServiceException =
                new ListenerEventArchiveV2ServiceException(
                    message: "Listener event archive service error occurred, contact support.",
                    innerException: failedListenerEventArchiveV2ServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<ListenerEventArchiveV2> addListenerEventArchiveV2Task =
                this.listenerEventArchiveV2Service.AddListenerEventArchiveV2Async(
                    someListenerEventArchiveV2,
                    TestContext.Current.CancellationToken);

            ListenerEventArchiveV2ServiceException actualListenerEventArchiveV2ServiceException =
                await Assert.ThrowsAsync<ListenerEventArchiveV2ServiceException>(
                    addListenerEventArchiveV2Task.AsTask);

            // then
            actualListenerEventArchiveV2ServiceException.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV2ServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV2ServiceException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventArchiveV2Async(
                    It.IsAny<ListenerEventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
