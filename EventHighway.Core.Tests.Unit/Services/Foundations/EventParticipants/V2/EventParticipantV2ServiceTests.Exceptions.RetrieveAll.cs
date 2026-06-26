// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

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

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventParticipantV2sAsync(randomCancellationToken))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<EventParticipantV2>> retrieveAllEventParticipantV2sTask =
                this.eventParticipantV2Service
                    .RetrieveAllEventParticipantV2sAsync(randomCancellationToken);

            EventParticipantV2DependencyException actualEventParticipantV2DependencyException =
                await Assert.ThrowsAsync<EventParticipantV2DependencyException>(
                    retrieveAllEventParticipantV2sTask.AsTask);

            // then
            actualEventParticipantV2DependencyException.Should()
                .BeEquivalentTo(expectedEventParticipantV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventParticipantV2sAsync(randomCancellationToken),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
