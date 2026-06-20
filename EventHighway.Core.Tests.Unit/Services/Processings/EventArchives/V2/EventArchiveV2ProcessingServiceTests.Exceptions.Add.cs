// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Processings.EventArchives.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventArchives.V2
{
    public partial class EventArchiveV2ProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventArchiveV2 someEventArchiveV2 = CreateRandomEventArchiveV2();

            var expectedEventArchiveV2ProcessingDependencyValidationException =
                new EventArchiveV2ProcessingDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.AddEventArchiveV2Async(
                    It.IsAny<EventArchiveV2>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<EventArchiveV2> addEventArchiveV2Task =
                this.eventArchiveV2ProcessingService.AddEventArchiveV2Async(
                    someEventArchiveV2,
                    randomCancellationToken);

            EventArchiveV2ProcessingDependencyValidationException
                actualEventArchiveV2ProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<EventArchiveV2ProcessingDependencyValidationException>(
                        addEventArchiveV2Task.AsTask);

            // then
            actualEventArchiveV2ProcessingDependencyValidationException.Should().BeEquivalentTo(
                expectedEventArchiveV2ProcessingDependencyValidationException);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.AddEventArchiveV2Async(
                    It.IsAny<EventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2ProcessingDependencyValidationException))),
                        Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnAddIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventArchiveV2 someEventArchiveV2 = CreateRandomEventArchiveV2();

            var expectedEventArchiveV2ProcessingDependencyException =
                new EventArchiveV2ProcessingDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.AddEventArchiveV2Async(
                    It.IsAny<EventArchiveV2>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<EventArchiveV2> addEventArchiveV2Task =
                this.eventArchiveV2ProcessingService.AddEventArchiveV2Async(
                    someEventArchiveV2,
                    randomCancellationToken);

            EventArchiveV2ProcessingDependencyException
                actualEventArchiveV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<EventArchiveV2ProcessingDependencyException>(
                        addEventArchiveV2Task.AsTask);

            // then
            actualEventArchiveV2ProcessingDependencyException.Should().BeEquivalentTo(
                expectedEventArchiveV2ProcessingDependencyException);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.AddEventArchiveV2Async(
                    It.IsAny<EventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2ProcessingDependencyException))),
                        Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventArchiveV2 someEventArchiveV2 = CreateRandomEventArchiveV2();
            var serviceException = new Exception();

            serviceException.Data.Add(
                key: GetRandomString(),
                value: new List<string> { GetRandomString() });

            var failedEventArchiveV2ProcessingServiceException =
                new FailedEventArchiveV2ProcessingServiceException(
                    message: "Failed event archive service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventArchiveV2ProcessingServiceException =
                new EventArchiveV2ProcessingServiceException(
                    message: "Event archive service error occurred, contact support.",
                    innerException: failedEventArchiveV2ProcessingServiceException);

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.AddEventArchiveV2Async(
                    It.IsAny<EventArchiveV2>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<EventArchiveV2> addEventArchiveV2Task =
                this.eventArchiveV2ProcessingService.AddEventArchiveV2Async(
                    someEventArchiveV2,
                    randomCancellationToken);

            EventArchiveV2ProcessingServiceException
                actualEventArchiveV2ProcessingServiceException =
                    await Assert.ThrowsAsync<EventArchiveV2ProcessingServiceException>(
                        addEventArchiveV2Task.AsTask);

            // then
            actualEventArchiveV2ProcessingServiceException.Should().BeEquivalentTo(
                expectedEventArchiveV2ProcessingServiceException);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.AddEventArchiveV2Async(
                    It.IsAny<EventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2ProcessingServiceException))),
                        Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfTimeoutOccursAndLogItAsync()
        {
            // given
            EventArchiveV2 someEventArchiveV2 = CreateRandomEventArchiveV2();

            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutEventArchiveV2ProcessingException =
                new TimeoutEventArchiveV2ProcessingException(
                    message: "Failed event archive processing timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedEventArchiveV2ProcessingDependencyException =
                new EventArchiveV2ProcessingDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: timeoutEventArchiveV2ProcessingException);

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.AddEventArchiveV2Async(
                    It.IsAny<EventArchiveV2>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<EventArchiveV2> addEventArchiveV2Task =
                this.eventArchiveV2ProcessingService.AddEventArchiveV2Async(
                    someEventArchiveV2,
                    TestContext.Current.CancellationToken);

            EventArchiveV2ProcessingDependencyException
                actualEventArchiveV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<EventArchiveV2ProcessingDependencyException>(
                        addEventArchiveV2Task.AsTask);

            // then
            actualEventArchiveV2ProcessingDependencyException.Should().BeEquivalentTo(
                expectedEventArchiveV2ProcessingDependencyException);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.AddEventArchiveV2Async(
                    It.IsAny<EventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2ProcessingDependencyException))),
                        Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnAddAsync()
        {
            // given
            EventArchiveV2 someEventArchiveV2 = CreateRandomEventArchiveV2();

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<EventArchiveV2> addEventArchiveV2Task =
                this.eventArchiveV2ProcessingService.AddEventArchiveV2Async(
                    someEventArchiveV2,
                    cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    addEventArchiveV2Task.AsTask);

            actualException.Should().NotBeOfType<EventArchiveV2ProcessingDependencyException>();
            actualException.Should().NotBeOfType<EventArchiveV2ProcessingServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
