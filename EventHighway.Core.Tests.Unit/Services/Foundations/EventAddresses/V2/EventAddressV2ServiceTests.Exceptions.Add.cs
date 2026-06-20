// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventAddresses.V2
{
    public partial class EventAddressV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventAddressV2 someEventAddressV2 = CreateRandomEventAddressV2();
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

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<EventAddressV2> addEventAddressV2Task =
                this.eventAddressV2Service.AddEventAddressV2Async(
                    someEventAddressV2, randomCancellationToken);

            EventAddressV2DependencyException actualEventAddressV2DependencyException =
                await Assert.ThrowsAsync<EventAddressV2DependencyException>(
                    addEventAddressV2Task.AsTask);

            // then
            actualEventAddressV2DependencyException.Should()
                .BeEquivalentTo(expectedEventAddressV2DependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventAddressV2Async(It.IsAny<EventAddressV2>(), It.IsAny<CancellationToken>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfEventAddressV2AlreadyExistsAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            EventAddressV2 someEventAddressV2 = CreateRandomEventAddressV2();
            var duplicateKeyException = new DuplicateKeyException(someMessage);
            duplicateKeyException.Data.Add("ErrorCode", new List<string> { "DuplicateKeyError" });

            var alreadyExistsEventAddressV2Exception =
                new AlreadyExistsEventAddressV2Exception(
                    message: "Event address with the same id already exists.",
                    innerException: duplicateKeyException,
                    data: duplicateKeyException.Data);

            var expectedEventAddressV2DependencyValidationException =
                new EventAddressV2DependencyValidationException(
                    message: "Event address validation error occurred, fix the errors and try again.",
                    innerException: alreadyExistsEventAddressV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(duplicateKeyException);

            // when
            ValueTask<EventAddressV2> addEventAddressV2Task =
                this.eventAddressV2Service.AddEventAddressV2Async(
                    someEventAddressV2, randomCancellationToken);

            EventAddressV2DependencyValidationException actualEventAddressV2DependencyValidationException =
                await Assert.ThrowsAsync<EventAddressV2DependencyValidationException>(
                    addEventAddressV2Task.AsTask);

            // then
            actualEventAddressV2DependencyValidationException.Should()
                .BeEquivalentTo(expectedEventAddressV2DependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV2DependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventAddressV2Async(It.IsAny<EventAddressV2>(), It.IsAny<CancellationToken>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDbUpdateExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventAddressV2 someEventAddressV2 = CreateRandomEventAddressV2();
            var dbUpdateException = new DbUpdateException();
            dbUpdateException.Data.Add("ErrorCode", new List<string> { "DbUpdateError" });

            var failedStorageEventAddressV2Exception =
                new FailedStorageEventAddressV2Exception(
                    message: "Failed event address storage error occurred, contact support.",
                    innerException: dbUpdateException,
                    data: dbUpdateException.Data);

            var expectedEventAddressV2DependencyException =
                new EventAddressV2DependencyException(
                    message: "Event address dependency error occurred, contact support.",
                    innerException: failedStorageEventAddressV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(dbUpdateException);

            // when
            ValueTask<EventAddressV2> addEventAddressV2Task =
                this.eventAddressV2Service.AddEventAddressV2Async(
                    someEventAddressV2, randomCancellationToken);

            EventAddressV2DependencyException actualEventAddressV2DependencyException =
                await Assert.ThrowsAsync<EventAddressV2DependencyException>(
                    addEventAddressV2Task.AsTask);

            // then
            actualEventAddressV2DependencyException.Should()
                .BeEquivalentTo(expectedEventAddressV2DependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventAddressV2Async(It.IsAny<EventAddressV2>(), It.IsAny<CancellationToken>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventAddressV2 someEventAddressV2 = CreateRandomEventAddressV2();
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

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<EventAddressV2> addEventAddressV2Task =
                this.eventAddressV2Service.AddEventAddressV2Async(
                    someEventAddressV2, randomCancellationToken);

            EventAddressV2ServiceException actualEventAddressV2ServiceException =
                await Assert.ThrowsAsync<EventAddressV2ServiceException>(
                    addEventAddressV2Task.AsTask);

            // then
            actualEventAddressV2ServiceException.Should()
                .BeEquivalentTo(expectedEventAddressV2ServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV2ServiceException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventAddressV2Async(It.IsAny<EventAddressV2>(), It.IsAny<CancellationToken>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfTimeoutOccursAndLogItAsync()
        {
            // given
            EventAddressV2 someEventAddressV2 = CreateRandomEventAddressV2();
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutEventAddressV2Exception =
                new TimeoutEventAddressV2Exception(
                    message: "Failed event address timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedEventAddressV2DependencyException =
                new EventAddressV2DependencyException(
                    message: "Event address dependency error occurred, contact support.",
                    innerException: timeoutEventAddressV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<EventAddressV2> addEventAddressV2Task =
                this.eventAddressV2Service.AddEventAddressV2Async(
                    someEventAddressV2, TestContext.Current.CancellationToken);

            EventAddressV2DependencyException actualEventAddressV2DependencyException =
                await Assert.ThrowsAsync<EventAddressV2DependencyException>(
                    addEventAddressV2Task.AsTask);

            // then
            actualEventAddressV2DependencyException.Should().BeEquivalentTo(
                expectedEventAddressV2DependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventAddressV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventAddressV2Async(It.IsAny<EventAddressV2>(), It.IsAny<CancellationToken>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnAddAsync()
        {
            // given
            EventAddressV2 someEventAddressV2 = CreateRandomEventAddressV2();
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<EventAddressV2> addEventAddressV2Task =
                this.eventAddressV2Service.AddEventAddressV2Async(
                    someEventAddressV2, cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    addEventAddressV2Task.AsTask);

            actualException.Should().NotBeOfType<EventAddressV2DependencyException>();
            actualException.Should().NotBeOfType<EventAddressV2ServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeptions.Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeptions.Xeption>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
