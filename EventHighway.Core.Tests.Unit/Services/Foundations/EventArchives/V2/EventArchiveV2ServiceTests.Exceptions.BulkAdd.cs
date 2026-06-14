// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventArchives.V2
{
    public partial class EventArchiveV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnBulkAddIfSqlErrorOccursAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();

            IQueryable<EventArchiveV2> randomEventArchiveV2s =
                CreateRandomEventArchiveV2s();

            List<EventArchiveV2> inputEventArchiveV2s =
                randomEventArchiveV2s.ToList();

            SqlException sqlException = CreateSqlException();
            sqlException.Data.Add("ErrorCode", new List<string> { "SqlError" });

            sqlException.Data.Add(
                key: nameof(SqlException.Number),
                value: new List<string> { "Some SQL error code" });

            var failedStorageEventArchiveV2Exception =
                new FailedStorageEventArchiveV2Exception(
                    message: "Failed event archive storage error occurred, contact support.",
                    innerException: sqlException,
                    data: sqlException.Data);

            var expectedEventArchiveV2DependencyException =
                new EventArchiveV2DependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: failedStorageEventArchiveV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertBulkEventArchiveV2sAsync(
                    It.IsAny<List<EventArchiveV2>>(),
                        It.IsAny<CancellationToken>()))
                            .ThrowsAsync(sqlException);

            // when
            ValueTask<IEnumerable<EventArchiveV2>> bulkAddEventArchiveV2sTask =
                this.eventArchiveV2Service.BulkAddEventArchiveV2sAsync(
                    inputEventArchiveV2s,
                        TestContext.Current.CancellationToken);

            // then
            EventArchiveV2DependencyException actualEventArchiveV2DependencyException =
                await Assert.ThrowsAsync<EventArchiveV2DependencyException>(
                    bulkAddEventArchiveV2sTask.AsTask);

            actualEventArchiveV2DependencyException.Should()
                .BeEquivalentTo(expectedEventArchiveV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertBulkEventArchiveV2sAsync(
                    It.IsAny<List<EventArchiveV2>>(),
                        It.IsAny<CancellationToken>()),
                            Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2DependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Exactly(inputEventArchiveV2s.Count + 1));

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnBulkAddIfExceptionOccursAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();

            IQueryable<EventArchiveV2> randomEventArchiveV2s =
                CreateRandomEventArchiveV2s();

            List<EventArchiveV2> inputEventArchiveV2s =
                randomEventArchiveV2s.ToList();

            var serviceException = new Exception();

            serviceException.Data.Add(
                "ErrorCode",
                new List<string> { "ServiceError" });

            var failedEventArchiveV2ServiceException =
                new FailedEventArchiveV2ServiceException(
                    message: "Failed event archive service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventArchiveV2ServiceException =
                new EventArchiveV2ServiceException(
                    message: "Event archive service error occurred, contact support.",
                    innerException: failedEventArchiveV2ServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertBulkEventArchiveV2sAsync(
                    It.IsAny<List<EventArchiveV2>>(),
                        It.IsAny<CancellationToken>()))
                            .ThrowsAsync(serviceException);

            // when
            ValueTask<IEnumerable<EventArchiveV2>> bulkAddEventArchiveV2sTask =
                this.eventArchiveV2Service.BulkAddEventArchiveV2sAsync(
                    inputEventArchiveV2s,
                        TestContext.Current.CancellationToken);

            // then
            EventArchiveV2ServiceException actualEventArchiveV2ServiceException =
                await Assert.ThrowsAsync<EventArchiveV2ServiceException>(
                    bulkAddEventArchiveV2sTask.AsTask);

            actualEventArchiveV2ServiceException.Should()
                .BeEquivalentTo(expectedEventArchiveV2ServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertBulkEventArchiveV2sAsync(
                    It.IsAny<List<EventArchiveV2>>(),
                        It.IsAny<CancellationToken>()),
                            Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2ServiceException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Exactly(inputEventArchiveV2s.Count + 1));

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
