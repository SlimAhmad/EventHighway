// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Processings.ListenerEvents.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEvents.V2
{
    public partial class ListenerEventV2ProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAllIfDependencyExceptionOccursAndLogItAsync(
            Xeption listenerEventV2DependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedListenerEventV2ProcessingDependencyException =
                new ListenerEventV2ProcessingDependencyException(
                    message: "Listener event dependency error occurred, contact support.",
                    innerException: listenerEventV2DependencyException.InnerException as Xeption);

            this.listenerEventV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(listenerEventV2DependencyException);

            // when
            ValueTask<IQueryable<ListenerEventV2>> retrieveAllListenerEventV2sTask =
                this.listenerEventV2ProcessingService.RetrieveAllListenerEventV2sAsync(randomCancellationToken);

            ListenerEventV2ProcessingDependencyException
                actualListenerEventV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<ListenerEventV2ProcessingDependencyException>(
                        retrieveAllListenerEventV2sTask.AsTask);

            // then
            actualListenerEventV2ProcessingDependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV2ProcessingDependencyException);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2ProcessingDependencyException))),
                        Times.Once);

            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
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

            var failedListenerEventV2ProcessingServiceException =
                new FailedListenerEventV2ProcessingServiceException(
                    message: "Failed listener event service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedListenerEventV2ProcessingServiceException =
                new ListenerEventV2ProcessingServiceException(
                    message: "Listener event service error occurred, contact support.",
                    innerException: failedListenerEventV2ProcessingServiceException);

            this.listenerEventV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IQueryable<ListenerEventV2>> retrieveAllListenerEventV2sTask =
                this.listenerEventV2ProcessingService.RetrieveAllListenerEventV2sAsync(randomCancellationToken);

            ListenerEventV2ProcessingServiceException
                actualListenerEventV2ProcessingServiceException =
                    await Assert.ThrowsAsync<ListenerEventV2ProcessingServiceException>(
                        retrieveAllListenerEventV2sTask.AsTask);

            // then
            actualListenerEventV2ProcessingServiceException.Should()
                .BeEquivalentTo(expectedListenerEventV2ProcessingServiceException);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2ProcessingServiceException))),
                        Times.Once);

            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAllIfTimeoutOccursAndLogItAsync()
        {
            // given
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutListenerEventV2ProcessingException =
                new TimeoutListenerEventV2ProcessingException(
                    message: "Failed listener event processing timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedListenerEventV2ProcessingDependencyException =
                new ListenerEventV2ProcessingDependencyException(
                    message: "Listener event dependency error occurred, contact support.",
                    innerException: timeoutListenerEventV2ProcessingException);

            this.listenerEventV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IQueryable<ListenerEventV2>> retrieveAllListenerEventV2sTask =
                this.listenerEventV2ProcessingService.RetrieveAllListenerEventV2sAsync(
                    TestContext.Current.CancellationToken);

            ListenerEventV2ProcessingDependencyException
                actualListenerEventV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<ListenerEventV2ProcessingDependencyException>(
                        retrieveAllListenerEventV2sTask.AsTask);

            // then
            actualListenerEventV2ProcessingDependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV2ProcessingDependencyException);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2ProcessingDependencyException))),
                        Times.Once);

            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveAllAsync()
        {
            // given
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<IQueryable<ListenerEventV2>> retrieveAllListenerEventV2sTask =
                this.listenerEventV2ProcessingService.RetrieveAllListenerEventV2sAsync(cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveAllListenerEventV2sTask.AsTask);

            actualException.Should().NotBeOfType<ListenerEventV2ProcessingDependencyException>();
            actualException.Should().NotBeOfType<ListenerEventV2ProcessingServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
