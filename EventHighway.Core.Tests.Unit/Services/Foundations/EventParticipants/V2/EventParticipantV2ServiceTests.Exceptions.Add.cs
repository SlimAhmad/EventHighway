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

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventParticipants.V2
{
    public partial class EventParticipantV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventParticipantV2 someEventParticipantV2 = CreateRandomEventParticipantV2();
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

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<EventParticipantV2> addEventParticipantV2Task =
                this.eventParticipantV2Service.AddEventParticipantV2Async(
                    someEventParticipantV2, randomCancellationToken);

            EventParticipantV2DependencyException actualEventParticipantV2DependencyException =
                await Assert.ThrowsAsync<EventParticipantV2DependencyException>(
                    addEventParticipantV2Task.AsTask);

            // then
            actualEventParticipantV2DependencyException.Should()
                .BeEquivalentTo(expectedEventParticipantV2DependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventParticipantV2Async(
                    It.IsAny<EventParticipantV2>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfEventParticipantV2AlreadyExistsAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            EventParticipantV2 someEventParticipantV2 = CreateRandomEventParticipantV2();
            var duplicateKeyException = new DuplicateKeyException(someMessage);
            duplicateKeyException.Data.Add("ErrorCode", new List<string> { "DuplicateKeyError" });

            var alreadyExistsEventParticipantV2Exception =
                new AlreadyExistsEventParticipantV2Exception(
                    message: "Event participant with the same id already exists.",
                    innerException: duplicateKeyException,
                    data: duplicateKeyException.Data);

            var expectedEventParticipantV2DependencyValidationException =
                new EventParticipantV2DependencyValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: alreadyExistsEventParticipantV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<EventParticipantV2> addEventParticipantV2Task =
                this.eventParticipantV2Service.AddEventParticipantV2Async(
                    someEventParticipantV2, randomCancellationToken);

            EventParticipantV2DependencyValidationException
                actualEventParticipantV2DependencyValidationException =
                    await Assert.ThrowsAsync<EventParticipantV2DependencyValidationException>(
                        addEventParticipantV2Task.AsTask);

            // then
            actualEventParticipantV2DependencyValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantV2DependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2DependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventParticipantV2Async(
                    It.IsAny<EventParticipantV2>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
