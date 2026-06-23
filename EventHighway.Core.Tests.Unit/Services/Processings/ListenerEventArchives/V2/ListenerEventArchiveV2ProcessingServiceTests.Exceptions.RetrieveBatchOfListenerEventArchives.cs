// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
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
        public async Task ShouldThrowDependencyExceptionOnRetrieveBatchOfListenerEventArchivesIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int skip = GetRandomNumber();
            int take = GetRandomNumber();

            var expectedException =
                new ListenerEventArchiveV2ProcessingDependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<List<ListenerEventArchiveV2>> retrieveBatchTask =
                this.listenerEventArchiveV2ProcessingService
                    .RetrieveBatchOfListenerEventArchiveV2sAsync(
                        eventAddressId: null,
                        eventListenerIds: null,
                        startDate: null,
                        endDate: null,
                        skip: skip,
                        take: take,
                        cancellationToken: randomCancellationToken);

            ListenerEventArchiveV2ProcessingDependencyException actualException =
                await Assert.ThrowsAsync<
                    ListenerEventArchiveV2ProcessingDependencyException>(
                        retrieveBatchTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveBatchOfListenerEventArchivesIfTimeoutOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int skip = GetRandomNumber();
            int take = GetRandomNumber();

            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutListenerEventArchiveV2ProcessingException =
                new TimeoutListenerEventArchiveV2ProcessingException(
                    message: "Failed listener event archive processing timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedException =
                new ListenerEventArchiveV2ProcessingDependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: timeoutListenerEventArchiveV2ProcessingException);

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<List<ListenerEventArchiveV2>> retrieveBatchTask =
                this.listenerEventArchiveV2ProcessingService
                    .RetrieveBatchOfListenerEventArchiveV2sAsync(
                        eventAddressId: null,
                        eventListenerIds: null,
                        startDate: null,
                        endDate: null,
                        skip: skip,
                        take: take,
                        cancellationToken: randomCancellationToken);

            ListenerEventArchiveV2ProcessingDependencyException actualException =
                await Assert.ThrowsAsync<
                    ListenerEventArchiveV2ProcessingDependencyException>(
                        retrieveBatchTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(expectedException);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(expectedException))),
                    Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveBatchOfListenerEventArchivesAsync()
        {
            // given
            int skip = GetRandomNumber();
            int take = GetRandomNumber();

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<List<ListenerEventArchiveV2>> retrieveBatchTask =
                this.listenerEventArchiveV2ProcessingService
                    .RetrieveBatchOfListenerEventArchiveV2sAsync(
                        eventAddressId: null,
                        eventListenerIds: null,
                        startDate: null,
                        endDate: null,
                        skip: skip,
                        take: take,
                        cancellationToken: cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveBatchTask.AsTask);

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
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
