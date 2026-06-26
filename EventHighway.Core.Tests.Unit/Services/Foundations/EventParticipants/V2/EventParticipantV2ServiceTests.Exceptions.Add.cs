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
    }
}
