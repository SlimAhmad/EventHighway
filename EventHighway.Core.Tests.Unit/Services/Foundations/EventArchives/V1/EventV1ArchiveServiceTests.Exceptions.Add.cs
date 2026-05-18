// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventArchives.V1
{
    public partial class EventV1ArchiveServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            EventArchiveV1 someEventV1Archive = CreateRandomEventV1Archive();
            SqlException sqlException = CreateSqlException();

            var failedEventV1ArchiveStorageException =
                new FailedEventArchiveV1StorageException(
                    message: "Failed event archive storage error occurred, contact support.",
                    innerException: sqlException);

            var expectedEventV1ArchiveDependencyException =
                new EventArchiveV1DependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: failedEventV1ArchiveStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<EventArchiveV1> addEventV1ArchiveTask =
                this.eventV1ArchiveService.AddEventArchiveV1Async(someEventV1Archive);

            EventArchiveV1DependencyException actualEventV1ArchiveDependencyException =
                await Assert.ThrowsAsync<EventArchiveV1DependencyException>(
                    addEventV1ArchiveTask.AsTask);

            // then
            actualEventV1ArchiveDependencyException.Should()
                .BeEquivalentTo(expectedEventV1ArchiveDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventV1ArchiveAsync(It.IsAny<EventArchiveV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfEventV1ArchiveAlreadyExistsAndLogItAsync()
        {
            // given
            string randomMessage = GetRandomString();
            EventArchiveV1 someEventV1Archive = CreateRandomEventV1Archive();
            var duplicateKeyException = new DuplicateKeyException(randomMessage);

            var alreadyExistsEventV1ArchiveException =
                new AlreadyExistsEventV1ArchiveException(
                    message: "Event archive with the same id already exists.",
                    innerException: duplicateKeyException);

            var expectedEventV1ArchiveDependencyValidationException =
                new EventArchiveV1DependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: alreadyExistsEventV1ArchiveException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<EventArchiveV1> addEventV1ArchiveTask =
                this.eventV1ArchiveService.AddEventArchiveV1Async(someEventV1Archive);

            EventArchiveV1DependencyValidationException actualEventV1ArchiveDependencyValidationException =
                await Assert.ThrowsAsync<EventArchiveV1DependencyValidationException>(
                    addEventV1ArchiveTask.AsTask);

            // then
            actualEventV1ArchiveDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventV1ArchiveDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventV1ArchiveAsync(
                    It.IsAny<EventArchiveV1>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfReferenceErrorOccursAndLogItAsync()
        {
            // given
            EventArchiveV1 someEventV1Archive = CreateRandomEventV1Archive();
            string someMessage = GetRandomString();

            var foreignKeyConstraintConflictException =
                new ForeignKeyConstraintConflictException(someMessage);

            var invalidEventV1ArchiveReferenceException =
                new InvalidEventArchiveV1ReferenceException(
                    message: "Invalid event archive reference error occurred.",
                    innerException: foreignKeyConstraintConflictException);

            var expectedEventV1ArchiveDependencyValidationException =
                new EventArchiveV1DependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV1ArchiveReferenceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(foreignKeyConstraintConflictException);

            // when
            ValueTask<EventArchiveV1> addEventV1ArchiveTask =
                this.eventV1ArchiveService.AddEventArchiveV1Async(someEventV1Archive);

            EventArchiveV1DependencyValidationException actualEventV1ArchiveDependencyValidationException =
                await Assert.ThrowsAsync<EventArchiveV1DependencyValidationException>(
                    addEventV1ArchiveTask.AsTask);

            // then
            actualEventV1ArchiveDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventV1ArchiveDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventV1ArchiveAsync(
                    It.IsAny<EventArchiveV1>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateExceptionOccursAndLogItAsync()
        {
            // given
            EventArchiveV1 someEventV1Archive = CreateRandomEventV1Archive();
            var dbUpdateException = new DbUpdateException();

            var failedEventV1ArchiveStorageException =
                new FailedEventArchiveV1StorageException(
                    message: "Failed event archive storage error occurred, contact support.",
                    innerException: dbUpdateException);

            var expectedEventV1ArchiveDependencyException =
                new EventArchiveV1DependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: failedEventV1ArchiveStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<EventArchiveV1> addEventV1ArchiveTask =
                this.eventV1ArchiveService.AddEventArchiveV1Async(someEventV1Archive);

            EventArchiveV1DependencyException actualEventV1ArchiveDependencyException =
                await Assert.ThrowsAsync<EventArchiveV1DependencyException>(
                    addEventV1ArchiveTask.AsTask);

            // then
            actualEventV1ArchiveDependencyException.Should()
                .BeEquivalentTo(expectedEventV1ArchiveDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventV1ArchiveAsync(
                    It.IsAny<EventArchiveV1>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfExceptionOccursAndLogItAsync()
        {
            // given
            EventArchiveV1 someEventV1Archive = CreateRandomEventV1Archive();
            var serviceException = new Exception();

            var failedEventV1ArchiveServiceException =
                new FailedEventArchiveV1ServiceException(
                    message: "Failed event archive service error occurred, contact support.",
                    innerException: serviceException);

            var expectedEventV1ArchiveServiceException =
                new EventArchiveV1ServiceException(
                    message: "Event archive service error occurred, contact support.",
                    innerException: failedEventV1ArchiveServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<EventArchiveV1> addEventV1ArchiveTask =
                this.eventV1ArchiveService.AddEventArchiveV1Async(someEventV1Archive);

            EventArchiveV1ServiceException actualEventV1ArchiveServiceException =
                await Assert.ThrowsAsync<EventArchiveV1ServiceException>(
                    addEventV1ArchiveTask.AsTask);

            // then
            actualEventV1ArchiveServiceException.Should()
                .BeEquivalentTo(expectedEventV1ArchiveServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveServiceException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventV1ArchiveAsync(
                    It.IsAny<EventArchiveV1>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
