// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
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
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnBulkRestoreIfValidationExceptionOccursAndLogItAsync(
            Xeption dependencyValidationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEnumerable<ListenerEventV2> someListenerEventV2s = CreateRandomListenerEventV2s();

            var expectedException =
                new ListenerEventV2ProcessingDependencyValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: dependencyValidationException.InnerException as Xeption);

            this.listenerEventV2ServiceMock.Setup(service =>
                service.BulkRestoreListenerEventV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventV2>>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyValidationException);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> bulkRestoreTask =
                this.listenerEventV2ProcessingService.BulkRestoreListenerEventV2sAsync(
                    someListenerEventV2s,
                    randomCancellationToken);

            ListenerEventV2ProcessingDependencyValidationException actualException =
                await Assert.ThrowsAsync<ListenerEventV2ProcessingDependencyValidationException>(
                    bulkRestoreTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.BulkRestoreListenerEventV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventV2>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnBulkRestoreIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEnumerable<ListenerEventV2> someListenerEventV2s = CreateRandomListenerEventV2s();

            var expectedException =
                new ListenerEventV2ProcessingDependencyException(
                    message: "Listener event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.listenerEventV2ServiceMock.Setup(service =>
                service.BulkRestoreListenerEventV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventV2>>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> bulkRestoreTask =
                this.listenerEventV2ProcessingService.BulkRestoreListenerEventV2sAsync(
                    someListenerEventV2s,
                    randomCancellationToken);

            ListenerEventV2ProcessingDependencyException actualException =
                await Assert.ThrowsAsync<ListenerEventV2ProcessingDependencyException>(
                    bulkRestoreTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.BulkRestoreListenerEventV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventV2>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnBulkRestoreIfTimeoutOccursAndLogItAsync()
        {
            // given
            IEnumerable<ListenerEventV2> someListenerEventV2s = CreateRandomListenerEventV2s();
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutListenerEventV2ProcessingException =
                new TimeoutListenerEventV2ProcessingException(
                    message: "Failed listener event processing timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedException =
                new ListenerEventV2ProcessingDependencyException(
                    message: "Listener event dependency error occurred, contact support.",
                    innerException: timeoutListenerEventV2ProcessingException);

            this.listenerEventV2ServiceMock.Setup(service =>
                service.BulkRestoreListenerEventV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventV2>>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> bulkRestoreTask =
                this.listenerEventV2ProcessingService.BulkRestoreListenerEventV2sAsync(
                    someListenerEventV2s, TestContext.Current.CancellationToken);

            ListenerEventV2ProcessingDependencyException actualException =
                await Assert.ThrowsAsync<ListenerEventV2ProcessingDependencyException>(
                    bulkRestoreTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.BulkRestoreListenerEventV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventV2>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnBulkRestoreIfServiceErrorOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEnumerable<ListenerEventV2> someListenerEventV2s = CreateRandomListenerEventV2s();
            var serviceException = new Exception();

            var failedListenerEventV2ProcessingServiceException =
                new FailedListenerEventV2ProcessingServiceException(
                    message: "Failed listener event service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedException =
                new ListenerEventV2ProcessingServiceException(
                    message: "Listener event service error occurred, contact support.",
                    innerException: failedListenerEventV2ProcessingServiceException);

            this.listenerEventV2ServiceMock.Setup(service =>
                service.BulkRestoreListenerEventV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventV2>>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> bulkRestoreTask =
                this.listenerEventV2ProcessingService.BulkRestoreListenerEventV2sAsync(
                    someListenerEventV2s,
                    randomCancellationToken);

            ListenerEventV2ProcessingServiceException actualException =
                await Assert.ThrowsAsync<ListenerEventV2ProcessingServiceException>(
                    bulkRestoreTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.BulkRestoreListenerEventV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventV2>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnBulkRestoreAsync()
        {
            // given
            IEnumerable<ListenerEventV2> someListenerEventV2s = CreateRandomListenerEventV2s();

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<IEnumerable<ListenerEventV2>> bulkRestoreTask =
                this.listenerEventV2ProcessingService.BulkRestoreListenerEventV2sAsync(
                    someListenerEventV2s, cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    bulkRestoreTask.AsTask);

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
