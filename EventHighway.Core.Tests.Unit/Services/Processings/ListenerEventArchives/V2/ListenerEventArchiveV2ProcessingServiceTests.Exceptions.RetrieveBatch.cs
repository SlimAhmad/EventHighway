// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Configurations.BatchProcessings;
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
        public async Task ShouldThrowDependencyExceptionOnRetrieveBatchIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            var batchConfiguration = new BatchConfiguration
            {
                BatchSizeForBulkProcessing = 10
            };

            var expectedException =
                new ListenerEventArchiveV2ProcessingDependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync())
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<List<ListenerEventArchiveV2>> retrieveBatchTask =
                this.listenerEventArchiveV2ProcessingService
                    .RetrieveNextPurgeBatchOfArchivedEventV2sAsync(
                        DateTimeOffset.UtcNow,
                        CancellationToken.None);

            ListenerEventArchiveV2ProcessingDependencyException actualException =
                await Assert.ThrowsAsync<
                    ListenerEventArchiveV2ProcessingDependencyException>(
                        retrieveBatchTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveBatchIfTimeoutOccursAndLogItAsync()
        {
            // given
            var batchConfiguration = new BatchConfiguration
            {
                BatchSizeForBulkProcessing = 10
            };

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

            this.configurationBrokerMock.Setup(broker =>
                broker.GetBatchConfiguration())
                    .Returns(batchConfiguration);

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<List<ListenerEventArchiveV2>> retrieveNextPurgeBatchTask =
                this.listenerEventArchiveV2ProcessingService
                    .RetrieveNextPurgeBatchOfArchivedEventV2sAsync(
                        DateTimeOffset.UtcNow,
                        TestContext.Current.CancellationToken);

            ListenerEventArchiveV2ProcessingDependencyException
                actualListenerEventArchiveV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<ListenerEventArchiveV2ProcessingDependencyException>(
                        retrieveNextPurgeBatchTask.AsTask);

            // then
            actualListenerEventArchiveV2ProcessingDependencyException.Should().BeEquivalentTo(
                expectedListenerEventArchiveV2ProcessingDependencyException);

            this.configurationBrokerMock.Verify(broker =>
                broker.GetBatchConfiguration(),
                    Times.Once);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV2ProcessingDependencyException))),
                        Times.Once);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveBatchAsync()
        {
            // given
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<List<ListenerEventArchiveV2>> retrieveNextPurgeBatchTask =
                this.listenerEventArchiveV2ProcessingService
                    .RetrieveNextPurgeBatchOfArchivedEventV2sAsync(
                        DateTimeOffset.UtcNow,
                        cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveNextPurgeBatchTask.AsTask);

            actualException.Should().NotBeOfType<ListenerEventArchiveV2ProcessingDependencyException>();
            actualException.Should().NotBeOfType<ListenerEventArchiveV2ProcessingServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
