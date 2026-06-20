// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventListeners.V2
{
    public partial class EventListenerV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            SqlException sqlException = GetSqlException();
            sqlException.Data.Add("ErrorCode", new List<string> { "SqlError" });

            var failedStorageEventListenerV2Exception =
                new FailedStorageEventListenerV2Exception(
                    message: "Failed event listener storage error occurred, contact support.",
                    innerException: sqlException,
                    data: sqlException.Data);

            var expectedEventListenerV2DependencyException =
                new EventListenerV2DependencyException(
                    message: "Event listener dependency error occurred, contact support.",
                    innerException: failedStorageEventListenerV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventListenerV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<EventListenerV2>> retrieveAllEventListenerV2sTask =
                this.eventListenerV2Service.RetrieveAllEventListenerV2sAsync(randomCancellationToken);

            EventListenerV2DependencyException actualEventListenerV2DependencyException =
                await Assert.ThrowsAsync<EventListenerV2DependencyException>(
                    retrieveAllEventListenerV2sTask.AsTask);

            // then
            actualEventListenerV2DependencyException.Should()
                .BeEquivalentTo(expectedEventListenerV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventListenerV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveAllIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventListenerV2ServiceException =
                new FailedEventListenerV2ServiceException(
                    message: "Failed event listener service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventListenerV2ServiceException =
                new EventListenerV2ServiceException(
                    message: "Event listener service error occurred, contact support.",
                    innerException: failedEventListenerV2ServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventListenerV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IQueryable<EventListenerV2>> retrieveAllEventListenerV2sTask =
                this.eventListenerV2Service.RetrieveAllEventListenerV2sAsync(randomCancellationToken);

            EventListenerV2ServiceException actualEventListenerV2ServiceException =
                await Assert.ThrowsAsync<EventListenerV2ServiceException>(
                    retrieveAllEventListenerV2sTask.AsTask);

            // then
            actualEventListenerV2ServiceException.Should()
                .BeEquivalentTo(expectedEventListenerV2ServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventListenerV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2ServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
