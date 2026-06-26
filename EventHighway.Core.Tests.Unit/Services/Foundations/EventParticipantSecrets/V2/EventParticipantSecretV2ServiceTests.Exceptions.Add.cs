// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventParticipantSecrets.V2
{
    public partial class EventParticipantSecretV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventParticipantSecretV2 someEventParticipantSecretV2 =
                CreateRandomEventParticipantSecretV2();

            SqlException sqlException = CreateSqlException();
            sqlException.Data.Add("ErrorCode", new List<string> { "SqlError" });

            var failedStorageEventParticipantSecretV2Exception =
                new FailedStorageEventParticipantSecretV2Exception(
                    message: "Failed event participant secret storage error occurred, contact support.",
                    innerException: sqlException,
                    data: sqlException.Data);

            var expectedEventParticipantSecretV2DependencyException =
                new EventParticipantSecretV2DependencyException(
                    message: "Event participant secret dependency error occurred, contact support.",
                    innerException: failedStorageEventParticipantSecretV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<EventParticipantSecretV2> addEventParticipantSecretV2Task =
                this.eventParticipantSecretV2Service.AddEventParticipantSecretV2Async(
                    someEventParticipantSecretV2, randomCancellationToken);

            EventParticipantSecretV2DependencyException actualEventParticipantSecretV2DependencyException =
                await Assert.ThrowsAsync<EventParticipantSecretV2DependencyException>(
                    addEventParticipantSecretV2Task.AsTask);

            // then
            actualEventParticipantSecretV2DependencyException.Should()
                .BeEquivalentTo(expectedEventParticipantSecretV2DependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventParticipantSecretV2DependencyException))),
                            Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventParticipantSecretV2Async(
                    It.IsAny<EventParticipantSecretV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfEventParticipantSecretV2AlreadyExistsAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            EventParticipantSecretV2 someEventParticipantSecretV2 =
                CreateRandomEventParticipantSecretV2();

            var duplicateKeyException = new DuplicateKeyException(someMessage);
            duplicateKeyException.Data.Add("ErrorCode", new List<string> { "DuplicateKeyError" });

            var alreadyExistsEventParticipantSecretV2Exception =
                new AlreadyExistsEventParticipantSecretV2Exception(
                    message: "Event participant secret with the same id already exists.",
                    innerException: duplicateKeyException,
                    data: duplicateKeyException.Data);

            var expectedEventParticipantSecretV2DependencyValidationException =
                new EventParticipantSecretV2DependencyValidationException(
                    message: "Event participant secret validation error occurred, fix the errors and try again.",
                    innerException: alreadyExistsEventParticipantSecretV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<EventParticipantSecretV2> addEventParticipantSecretV2Task =
                this.eventParticipantSecretV2Service.AddEventParticipantSecretV2Async(
                    someEventParticipantSecretV2, randomCancellationToken);

            EventParticipantSecretV2DependencyValidationException
                actualEventParticipantSecretV2DependencyValidationException =
                    await Assert.ThrowsAsync<EventParticipantSecretV2DependencyValidationException>(
                        addEventParticipantSecretV2Task.AsTask);

            // then
            actualEventParticipantSecretV2DependencyValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantSecretV2DependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventParticipantSecretV2DependencyValidationException))),
                            Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventParticipantSecretV2Async(
                    It.IsAny<EventParticipantSecretV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
