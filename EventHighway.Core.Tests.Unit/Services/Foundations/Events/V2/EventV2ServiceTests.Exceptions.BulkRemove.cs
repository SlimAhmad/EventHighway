// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.Events.V2
{
    public partial class EventV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnBulkRemoveIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<EventV2> someEventV2s = CreateRandomEventV2s();
            IEnumerable<EventV2> inputEventV2s = someEventV2s;
            SqlException sqlException = GetSqlException();
            sqlException.Data.Add("ErrorCode", new List<string> { "SqlError" });

            var failedStorageEventV2Exception =
                new FailedStorageEventV2Exception(
                    message: "Failed event storage error occurred, contact support.",
                    innerException: sqlException,
                    data: sqlException.Data);

            var expectedEventV2DependencyException =
                new EventV2DependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: failedStorageEventV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.BulkDeleteEventV2sAsync(
                    inputEventV2s,
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(sqlException);

            // when
            ValueTask bulkRemoveEventV2sTask =
                this.eventV2Service.BulkRemoveEventV2sAsync(
                    inputEventV2s,
                    randomCancellationToken);

            EventV2DependencyException actualEventV2DependencyException =
                await Assert.ThrowsAsync<EventV2DependencyException>(
                    bulkRemoveEventV2sTask.AsTask);

            // then
            actualEventV2DependencyException.Should()
                .BeEquivalentTo(expectedEventV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.BulkDeleteEventV2sAsync(
                    inputEventV2s,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnBulkRemoveIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<EventV2> someEventV2s = CreateRandomEventV2s();
            IEnumerable<EventV2> inputEventV2s = someEventV2s;
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventV2ServiceException =
                new FailedEventV2ServiceException(
                    message: "Failed event service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventV2ServiceException =
                new EventV2ServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: failedEventV2ServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.BulkDeleteEventV2sAsync(
                    inputEventV2s,
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask bulkRemoveEventV2sTask =
                this.eventV2Service.BulkRemoveEventV2sAsync(
                    inputEventV2s,
                    randomCancellationToken);

            EventV2ServiceException actualEventV2ServiceException =
                await Assert.ThrowsAsync<EventV2ServiceException>(
                    bulkRemoveEventV2sTask.AsTask);

            // then
            actualEventV2ServiceException.Should()
                .BeEquivalentTo(expectedEventV2ServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.BulkDeleteEventV2sAsync(
                    inputEventV2s,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
