// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2.Exceptions;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEvents.V2
{
    public partial class ListenerEventV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByEventListenerV2IdIfSqlExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventListenerV2Id = GetRandomId();
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
                broker.SelectAllListenerEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<IQueryable<ListenerEventV2>> retrieveByEventListenerV2IdTask =
                this.listenerEventV2Service.RetrieveListenerEventV2sByEventListenerV2IdAsync(
                    someEventListenerV2Id,
                    randomCancellationToken);

            ListenerEventV2DependencyException actualListenerEventV2DependencyException =
                await Assert.ThrowsAsync<ListenerEventV2DependencyException>(
                    retrieveByEventListenerV2IdTask.AsTask);

            // then
            actualListenerEventV2DependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllListenerEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByEventListenerV2IdIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventListenerV2Id = GetRandomId();
            var serviceException = new Exception();

            var failedListenerEventV2ServiceException =
                new FailedListenerEventV2ServiceException(
                    message: "Failed listener event service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedListenerEventV2ServiceException =
                new ListenerEventV2ServiceException(
                    message: "Listener event service error occurred, contact support.",
                    innerException: failedListenerEventV2ServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllListenerEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IQueryable<ListenerEventV2>> retrieveByEventListenerV2IdTask =
                this.listenerEventV2Service.RetrieveListenerEventV2sByEventListenerV2IdAsync(
                    someEventListenerV2Id,
                    randomCancellationToken);

            ListenerEventV2ServiceException actualListenerEventV2ServiceException =
                await Assert.ThrowsAsync<ListenerEventV2ServiceException>(
                    retrieveByEventListenerV2IdTask.AsTask);

            // then
            actualListenerEventV2ServiceException.Should()
                .BeEquivalentTo(expectedListenerEventV2ServiceException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllListenerEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2ServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveByEventListenerV2IdIfTimeoutOccursAndLogItAsync()
        {
            // given
            Guid someEventListenerV2Id = GetRandomId();
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutListenerEventV2Exception =
                new TimeoutListenerEventV2Exception(
                    message: "Failed listener event timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedListenerEventV2DependencyException =
                new ListenerEventV2DependencyException(
                    message: "Listener event dependency error occurred, contact support.",
                    innerException: timeoutListenerEventV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllListenerEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IQueryable<ListenerEventV2>> retrieveByEventListenerV2IdTask =
                this.listenerEventV2Service.RetrieveListenerEventV2sByEventListenerV2IdAsync(
                    someEventListenerV2Id,
                    TestContext.Current.CancellationToken);

            ListenerEventV2DependencyException actualListenerEventV2DependencyException =
                await Assert.ThrowsAsync<ListenerEventV2DependencyException>(
                    retrieveByEventListenerV2IdTask.AsTask);

            // then
            actualListenerEventV2DependencyException.Should().BeEquivalentTo(
                expectedListenerEventV2DependencyException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllListenerEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2DependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveByEventListenerV2IdAsync()
        {
            // given
            Guid someEventListenerV2Id = GetRandomId();
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<IQueryable<ListenerEventV2>> retrieveByEventListenerV2IdTask =
                this.listenerEventV2Service.RetrieveListenerEventV2sByEventListenerV2IdAsync(
                    someEventListenerV2Id,
                    cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveByEventListenerV2IdTask.AsTask);

            actualException.Should().NotBeOfType<ListenerEventV2DependencyException>();
            actualException.Should().NotBeOfType<ListenerEventV2ServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
