// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEventArchives.V1
{
    public partial class ListenerEventV1ArchiveServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            ListenerEventArchiveV1 someListenerEventV1Archive = CreateRandomListenerEventV1Archive();
            SqlException sqlException = GetSqlException();

            var failedListenerEventV1ArchiveStorageException =
                new FailedListenerEventArchiveV1StorageException(
                    message: "Failed listener event archive storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedListenerEventV1ArchiveDependencyException =
                new ListenerEventArchiveV1DependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: failedListenerEventV1ArchiveStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<ListenerEventArchiveV1> addListenerEventV1ArchiveTask =
                this.listenerEventV1ArchiveService.AddListenerEventV1ArchiveAsync(someListenerEventV1Archive);

            ListenerEventArchiveV1DependencyException actualListenerEventV1ArchiveDependencyException =
                await Assert.ThrowsAsync<ListenerEventArchiveV1DependencyException>(
                    addListenerEventV1ArchiveTask.AsTask);

            // then
            actualListenerEventV1ArchiveDependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV1ArchiveDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ArchiveDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventV1ArchiveAsync(It.IsAny<ListenerEventArchiveV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDuplicateKeyErrorAndLogItAsync()
        {
            // given
            string randomMessage = GetRandomString();
            ListenerEventArchiveV1 someListenerEventV1Archive = CreateRandomListenerEventV1Archive();
            var duplicateKeyException = new DuplicateKeyException(randomMessage);

            var alreadyExistsListenerEventV1ArchiveException =
                new AlreadyExistsListenerEventArchiveV1Exception(
                    message: "Listener event archive with the same id already exists.",
                    innerException: duplicateKeyException);

            var expectedListenerEventV1ArchiveDependencyValidationException =
                new ListenerEventArchiveV1DependencyValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: alreadyExistsListenerEventV1ArchiveException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<ListenerEventArchiveV1> addListenerEventV1ArchiveTask =
                this.listenerEventV1ArchiveService.AddListenerEventV1ArchiveAsync(someListenerEventV1Archive);

            ListenerEventArchiveV1DependencyValidationException actualListenerEventV1ArchiveDependencyValidationException =
                await Assert.ThrowsAsync<ListenerEventArchiveV1DependencyValidationException>(
                    addListenerEventV1ArchiveTask.AsTask);

            // then
            actualListenerEventV1ArchiveDependencyValidationException.Should()
                .BeEquivalentTo(expectedListenerEventV1ArchiveDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ArchiveDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventV1ArchiveAsync(It.IsAny<ListenerEventArchiveV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateExceptionOccursAndLogItAsync()
        {
            // given
            ListenerEventArchiveV1 someListenerEventV1Archive = CreateRandomListenerEventV1Archive();
            var dbUpdateException = new DbUpdateException();

            var failedListenerEventV1ArchiveStorageException =
                new FailedListenerEventArchiveV1StorageException(
                    message: "Failed listener event archive storage error occurred, contact support.",
                    innerException: dbUpdateException);

            var expectedListenerEventV1ArchiveDependencyException =
                new ListenerEventArchiveV1DependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: failedListenerEventV1ArchiveStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<ListenerEventArchiveV1> addListenerEventV1ArchiveTask =
                this.listenerEventV1ArchiveService.AddListenerEventV1ArchiveAsync(someListenerEventV1Archive);

            ListenerEventArchiveV1DependencyException actualListenerEventV1ArchiveDependencyException =
                await Assert.ThrowsAsync<ListenerEventArchiveV1DependencyException>(
                    addListenerEventV1ArchiveTask.AsTask);

            // then
            actualListenerEventV1ArchiveDependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV1ArchiveDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ArchiveDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventV1ArchiveAsync(It.IsAny<ListenerEventArchiveV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfExceptionOccursAndLogItAsync()
        {
            // given
            ListenerEventArchiveV1 someListenerEventV1Archive = CreateRandomListenerEventV1Archive();
            var serviceException = new Exception();

            var failedListenerEventV1ArchiveServiceException =
                new FailedListenerEventArchiveV1ServiceException(
                    message: "Failed listener event archive service error occurred, contact support.",
                    innerException: serviceException);

            var expectedListenerEventV1ArchiveServiceException =
                new ListenerEventArchiveV1ServiceException(
                    message: "Listener event archive service error occurred, contact support.",
                    innerException: failedListenerEventV1ArchiveServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<ListenerEventArchiveV1> addListenerEventV1ArchiveTask =
                this.listenerEventV1ArchiveService.AddListenerEventV1ArchiveAsync(someListenerEventV1Archive);

            ListenerEventArchiveV1ServiceException actualListenerEventV1ArchiveServiceException =
                await Assert.ThrowsAsync<ListenerEventArchiveV1ServiceException>(
                    addListenerEventV1ArchiveTask.AsTask);

            // then
            actualListenerEventV1ArchiveServiceException.Should()
                .BeEquivalentTo(expectedListenerEventV1ArchiveServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ArchiveServiceException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventV1ArchiveAsync(It.IsAny<ListenerEventArchiveV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
