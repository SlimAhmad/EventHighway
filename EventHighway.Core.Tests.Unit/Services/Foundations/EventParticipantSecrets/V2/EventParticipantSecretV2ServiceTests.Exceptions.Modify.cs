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
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventParticipantSecrets.V2
{
    public partial class EventParticipantSecretV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            int randomDaysAgo = GetRandomNegativeNumber();
            EventParticipantSecretV2 someEventParticipantSecretV2 = CreateRandomEventParticipantSecretV2(randomDateTime);
            someEventParticipantSecretV2.CreatedDate = randomDateTime.AddDays(randomDaysAgo);

            SqlException sqlException = CreateSqlException();
            sqlException.Data.Add("ErrorCode", new List<string> { "SqlError" });

            var failedStorageEventParticipantSecretV2Exception =
                new FailedStorageEventParticipantSecretV2Exception(
                    message: "Failed event participant secret storage error occurred, contact support.",
                    innerException: sqlException,
                    data: sqlException.Data);

            var expectedEventParticipantSecretV2DependencyException =
                new EventParticipantSecretV2DependencyException(
                    message: "Event participant secret dependency error occurred, contact support.",
                    innerException: failedStorageEventParticipantSecretV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventParticipantSecretV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(sqlException);

            // when
            ValueTask<EventParticipantSecretV2> modifyEventParticipantSecretV2Task =
                this.eventParticipantSecretV2Service.ModifyEventParticipantSecretV2Async(
                    someEventParticipantSecretV2, randomCancellationToken);

            EventParticipantSecretV2DependencyException actualEventParticipantSecretV2DependencyException =
                await Assert.ThrowsAsync<EventParticipantSecretV2DependencyException>(
                    modifyEventParticipantSecretV2Task.AsTask);

            // then
            actualEventParticipantSecretV2DependencyException.Should()
                .BeEquivalentTo(expectedEventParticipantSecretV2DependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventParticipantSecretV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventParticipantSecretV2DependencyException))),
                            Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfTimeoutOccursAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            int randomDaysAgo = GetRandomNegativeNumber();
            EventParticipantSecretV2 someEventParticipantSecretV2 = CreateRandomEventParticipantSecretV2(randomDateTime);
            someEventParticipantSecretV2.CreatedDate = randomDateTime.AddDays(randomDaysAgo);
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutEventParticipantSecretV2Exception =
                new TimeoutEventParticipantSecretV2Exception(
                    message: "Failed event participant secret timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedEventParticipantSecretV2DependencyException =
                new EventParticipantSecretV2DependencyException(
                    message: "Event participant secret dependency error occurred, contact support.",
                    innerException: timeoutEventParticipantSecretV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventParticipantSecretV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<EventParticipantSecretV2> modifyEventParticipantSecretV2Task =
                this.eventParticipantSecretV2Service.ModifyEventParticipantSecretV2Async(
                    someEventParticipantSecretV2, TestContext.Current.CancellationToken);

            EventParticipantSecretV2DependencyException actualEventParticipantSecretV2DependencyException =
                await Assert.ThrowsAsync<EventParticipantSecretV2DependencyException>(
                    modifyEventParticipantSecretV2Task.AsTask);

            // then
            actualEventParticipantSecretV2DependencyException.Should().BeEquivalentTo(
                expectedEventParticipantSecretV2DependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventParticipantSecretV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventParticipantSecretV2DependencyException))),
                            Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnModifyAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            int randomDaysAgo = GetRandomNegativeNumber();
            EventParticipantSecretV2 someEventParticipantSecretV2 = CreateRandomEventParticipantSecretV2(randomDateTime);
            someEventParticipantSecretV2.CreatedDate = randomDateTime.AddDays(randomDaysAgo);

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<EventParticipantSecretV2> modifyEventParticipantSecretV2Task =
                this.eventParticipantSecretV2Service.ModifyEventParticipantSecretV2Async(
                    someEventParticipantSecretV2, cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    modifyEventParticipantSecretV2Task.AsTask);

            actualException.Should().NotBeOfType<EventParticipantSecretV2DependencyException>();
            actualException.Should().NotBeOfType<EventParticipantSecretV2ServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeptions.Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeptions.Xeption>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            int randomDaysAgo = GetRandomNegativeNumber();
            EventParticipantSecretV2 someEventParticipantSecretV2 = CreateRandomEventParticipantSecretV2(randomDateTime);
            someEventParticipantSecretV2.CreatedDate = randomDateTime.AddDays(randomDaysAgo);

            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventParticipantSecretV2ServiceException =
                new FailedEventParticipantSecretV2ServiceException(
                    message: "Failed event participant secret service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventParticipantSecretV2ServiceException =
                new EventParticipantSecretV2ServiceException(
                    message: "Event participant secret service error occurred, contact support.",
                    innerException: failedEventParticipantSecretV2ServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventParticipantSecretV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<EventParticipantSecretV2> modifyEventParticipantSecretV2Task =
                this.eventParticipantSecretV2Service.ModifyEventParticipantSecretV2Async(
                    someEventParticipantSecretV2, randomCancellationToken);

            EventParticipantSecretV2ServiceException actualEventParticipantSecretV2ServiceException =
                await Assert.ThrowsAsync<EventParticipantSecretV2ServiceException>(
                    modifyEventParticipantSecretV2Task.AsTask);

            // then
            actualEventParticipantSecretV2ServiceException.Should()
                .BeEquivalentTo(expectedEventParticipantSecretV2ServiceException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventParticipantSecretV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is<Xeption>(
                    actual => actual.SameExceptionAs(
                        expectedEventParticipantSecretV2ServiceException))),
                            Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
