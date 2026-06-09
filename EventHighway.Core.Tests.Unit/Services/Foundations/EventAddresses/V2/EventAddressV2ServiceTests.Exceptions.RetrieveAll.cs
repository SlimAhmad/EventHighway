// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventAddresses.V2
{
    public partial class EventAddressV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveAllIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            SqlException sqlException = CreateSqlException();
            sqlException.Data.Add("ErrorCode", new List<string> { "SqlError" });

            var failedStorageEventAddressV2Exception =
                new FailedStorageEventAddressV2Exception(
                    message: "Failed event address storage error occurred, contact support.",
                    innerException: sqlException,
                    data: sqlException.Data);

            var expectedEventAddressV2DependencyException =
                new EventAddressV2DependencyException(
                    message: "Event address dependency error occurred, contact support.",
                    innerException: failedStorageEventAddressV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventAddressV2sAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<EventAddressV2>> retrieveAllEventAddressV2sTask =
                this.eventAddressV2Service.RetrieveAllEventAddressV2sAsync();

            EventAddressV2DependencyException actualEventAddressV2DependencyException =
                await Assert.ThrowsAsync<EventAddressV2DependencyException>(
                    retrieveAllEventAddressV2sTask.AsTask);

            // then
            actualEventAddressV2DependencyException.Should()
                .BeEquivalentTo(expectedEventAddressV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventAddressV2sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveAllIfExceptionOccursAndLogItAsync()
        {
            // given
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventAddressV2ServiceException =
                new FailedEventAddressV2ServiceException(
                    message: "Failed event address service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventAddressV2ServiceException =
                new EventAddressV2ServiceException(
                    message: "Event address service error occurred, contact support.",
                    innerException: failedEventAddressV2ServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventAddressV2sAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IQueryable<EventAddressV2>> retrieveAllEventAddressV2sTask =
                this.eventAddressV2Service.RetrieveAllEventAddressV2sAsync();

            EventAddressV2ServiceException actualEventAddressV2ServiceException =
                await Assert.ThrowsAsync<EventAddressV2ServiceException>(
                    retrieveAllEventAddressV2sTask.AsTask);

            // then
            actualEventAddressV2ServiceException.Should()
                .BeEquivalentTo(expectedEventAddressV2ServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventAddressV2sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV2ServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
