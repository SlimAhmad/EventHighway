// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEventArchives.V2
{
    public partial class ListenerEventArchiveV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnBulkRemoveIfSqlErrorOccursAndLogItAsync()
        {
            // given
            IQueryable<ListenerEventArchiveV2> randomListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s();

            IEnumerable<ListenerEventArchiveV2> inputListenerEventArchiveV2s =
                randomListenerEventArchiveV2s.ToList();

            SqlException sqlException = CreateSqlException();
            sqlException.Data.Add("ErrorCode", new List<string> { "SqlError" });

            sqlException.Data.Add(
                key: nameof(SqlException.Number),
                value: new List<string> { "Some SQL error code" });

            var failedStorageListenerEventArchiveV2Exception =
                new FailedStorageListenerEventArchiveV2Exception(
                    message: "Failed listener event archive storage error occurred, contact support.",
                    innerException: sqlException,
                    data: sqlException.Data);

            var expectedListenerEventArchiveV2DependencyException =
                new ListenerEventArchiveV2DependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: failedStorageListenerEventArchiveV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.BulkDeleteListenerEventArchiveV2sAsync(
                    inputListenerEventArchiveV2s,
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(sqlException);

            // when
            ValueTask bulkAddListenerEventArchiveV2sTask =
                this.listenerEventArchiveV2Service.BulkRemoveListenerEventArchiveV2sAsync(
                    inputListenerEventArchiveV2s,
                        TestContext.Current.CancellationToken);

            ListenerEventArchiveV2DependencyException actualListenerEventArchiveV2DependencyException =
                await Assert.ThrowsAsync<ListenerEventArchiveV2DependencyException>(
                    bulkAddListenerEventArchiveV2sTask.AsTask);

            // then
            actualListenerEventArchiveV2DependencyException.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.BulkDeleteListenerEventArchiveV2sAsync(
                    inputListenerEventArchiveV2s,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnBulkRemoveIfExceptionOccursAndLogItAsync()
        {
            // given
            IQueryable<ListenerEventArchiveV2> randomListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s();

            IEnumerable<ListenerEventArchiveV2> inputListenerEventArchiveV2s =
                randomListenerEventArchiveV2s.ToList();

            var serviceException = new Exception();

            serviceException.Data.Add(
                "ErrorCode",
                new List<string> { "ServiceError" });

            var failedListenerEventArchiveV2ServiceException =
                new FailedListenerEventArchiveV2ServiceException(
                    message: "Failed listener event archive service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedListenerEventArchiveV2ServiceException =
                new ListenerEventArchiveV2ServiceException(
                    message: "Listener event archive service error occurred, contact support.",
                    innerException: failedListenerEventArchiveV2ServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.BulkDeleteListenerEventArchiveV2sAsync(
                    inputListenerEventArchiveV2s,
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask bulkAddListenerEventArchiveV2sTask =
                this.listenerEventArchiveV2Service.BulkRemoveListenerEventArchiveV2sAsync(
                    inputListenerEventArchiveV2s,
                        TestContext.Current.CancellationToken);

            ListenerEventArchiveV2ServiceException actualListenerEventArchiveV2ServiceException =
                await Assert.ThrowsAsync<ListenerEventArchiveV2ServiceException>(
                    bulkAddListenerEventArchiveV2sTask.AsTask);

            // then
            actualListenerEventArchiveV2ServiceException.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV2ServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.BulkDeleteListenerEventArchiveV2sAsync(
                    inputListenerEventArchiveV2s,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV2ServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
