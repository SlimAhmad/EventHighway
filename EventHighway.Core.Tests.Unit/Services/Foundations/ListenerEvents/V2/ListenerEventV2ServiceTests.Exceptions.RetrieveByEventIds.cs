// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEvents.V2
{
    public partial class ListenerEventV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByEventIdsIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            List<Guid> someEventIds =
                Enumerable.Range(0, GetRandomNumber())
                    .Select(_ => Guid.NewGuid())
                    .ToList();

            SqlException sqlException = GetSqlException();

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
                broker.SelectAllListenerEventV2sAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<ListenerEventV2>> retrieveByEventIdsTask =
                this.listenerEventV2Service
                    .RetrieveListenerEventV2sByEventIdsAsync(someEventIds);

            ListenerEventV2DependencyException actualListenerEventV2DependencyException =
                await Assert.ThrowsAsync<ListenerEventV2DependencyException>(
                    retrieveByEventIdsTask.AsTask);

            // then
            actualListenerEventV2DependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllListenerEventV2sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
