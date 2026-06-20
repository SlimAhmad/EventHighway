// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Processings.ListenerEventArchives.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEventArchives.V2
{
    public partial class ListenerEventArchiveV2ProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAllIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedListenerEventArchiveV2ProcessingDependencyException =
                new ListenerEventArchiveV2ProcessingDependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<IQueryable<ListenerEventArchiveV2>> retrieveAllTask =
                this.listenerEventArchiveV2ProcessingService
                    .RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken);

            ListenerEventArchiveV2ProcessingDependencyException actualException =
                await Assert.ThrowsAsync<ListenerEventArchiveV2ProcessingDependencyException>(
                    retrieveAllTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV2ProcessingDependencyException);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV2ProcessingDependencyException))),
                        Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAllIfTimeoutOccursAndLogItAsync()
        {
            // given
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutListenerEventArchiveV2ProcessingException =
                new TimeoutListenerEventArchiveV2ProcessingException(
                    message: "Failed listener event archive processing timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedListenerEventArchiveV2ProcessingDependencyException =
                new ListenerEventArchiveV2ProcessingDependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: timeoutListenerEventArchiveV2ProcessingException);

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IQueryable<ListenerEventArchiveV2>> retrieveAllListenerEventArchiveV2sTask =
                this.listenerEventArchiveV2ProcessingService
                    .RetrieveAllListenerEventArchiveV2sAsync(TestContext.Current.CancellationToken);

            ListenerEventArchiveV2ProcessingDependencyException
                actualListenerEventArchiveV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<ListenerEventArchiveV2ProcessingDependencyException>(
                        retrieveAllListenerEventArchiveV2sTask.AsTask);

            // then
            actualListenerEventArchiveV2ProcessingDependencyException.Should().BeEquivalentTo(
                expectedListenerEventArchiveV2ProcessingDependencyException);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV2ProcessingDependencyException))),
                        Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
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
            ValueTask<IQueryable<ListenerEventArchiveV2>> retrieveAllListenerEventArchiveV2sTask =
                this.listenerEventArchiveV2ProcessingService
                    .RetrieveAllListenerEventArchiveV2sAsync(cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveAllListenerEventArchiveV2sTask.AsTask);

            actualException.Should().NotBeOfType<ListenerEventArchiveV2ProcessingDependencyException>();
            actualException.Should().NotBeOfType<ListenerEventArchiveV2ProcessingServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
