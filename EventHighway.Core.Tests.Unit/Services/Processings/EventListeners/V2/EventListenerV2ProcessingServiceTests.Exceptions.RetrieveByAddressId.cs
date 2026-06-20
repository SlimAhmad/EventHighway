// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Processings.EventListeners.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventListeners.V2
{
    public partial class EventListenerV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveByEventAddressIdIfTimeoutOccursAndLogItAsync()
        {
            // given
            Guid someEventAddressId = GetRandomId();
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutEventListenerV2ProcessingException =
                new TimeoutEventListenerV2ProcessingException(
                    message: "Failed event listener processing timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedEventListenerV2ProcessingDependencyException =
                new EventListenerV2ProcessingDependencyException(
                    message: "Event listener dependency error occurred, contact support.",
                    innerException: timeoutEventListenerV2ProcessingException);

            this.eventListenerV2ServiceMock.Setup(service =>
                service.RetrieveAllEventListenerV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IQueryable<EventListenerV2>> retrieveEventListenerV2sByEventAddressIdTask =
                this.eventListenerV2ProcessingService.RetrieveEventListenerV2sByEventAddressIdAsync(
                    someEventAddressId,
                    TestContext.Current.CancellationToken);

            EventListenerV2ProcessingDependencyException actualEventListenerV2ProcessingDependencyException =
                await Assert.ThrowsAsync<EventListenerV2ProcessingDependencyException>(
                    retrieveEventListenerV2sByEventAddressIdTask.AsTask);

            // then
            actualEventListenerV2ProcessingDependencyException.Should().BeEquivalentTo(
                expectedEventListenerV2ProcessingDependencyException);

            this.eventListenerV2ServiceMock.Verify(service =>
                service.RetrieveAllEventListenerV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2ProcessingDependencyException))),
                        Times.Once);

            this.eventListenerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveByEventAddressIdIfDependencyErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventAddressId = GetRandomId();

            var expectedEventListenerV2ProcessingDependencyException =
                new EventListenerV2ProcessingDependencyException(
                    message: "Event listener dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventListenerV2ServiceMock.Setup(service =>
                service.RetrieveAllEventListenerV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<IQueryable<EventListenerV2>> retrieveEventListenerV2sByEventAddressIdTask =
                this.eventListenerV2ProcessingService.RetrieveEventListenerV2sByEventAddressIdAsync(
                    someEventAddressId,
                    randomCancellationToken);

            EventListenerV2ProcessingDependencyException actualEventListenerV2ProcessingDependencyException =
                await Assert.ThrowsAsync<EventListenerV2ProcessingDependencyException>(
                    retrieveEventListenerV2sByEventAddressIdTask.AsTask);

            // then
            actualEventListenerV2ProcessingDependencyException.Should()
                .BeEquivalentTo(expectedEventListenerV2ProcessingDependencyException);

            this.eventListenerV2ServiceMock.Verify(service =>
                service.RetrieveAllEventListenerV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2ProcessingDependencyException))),
                        Times.Once);

            this.eventListenerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnOnRetrieveByEventAddressIdIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventAddressId = GetRandomId();
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventListenerV2ProcessingServiceException =
                new FailedEventListenerV2ProcessingServiceException(
                    message: "Failed event listener service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventListenerV2ProcessingServiceException =
                new EventListenerV2ProcessingServiceException(
                    message: "Event listener service error occurred, contact support.",
                    innerException: failedEventListenerV2ProcessingServiceException);

            this.eventListenerV2ServiceMock.Setup(service =>
                service.RetrieveAllEventListenerV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IQueryable<EventListenerV2>> retrieveEventListenerV2sByEventAddressIdTask =
                this.eventListenerV2ProcessingService.RetrieveEventListenerV2sByEventAddressIdAsync(
                    someEventAddressId,
                    randomCancellationToken);

            EventListenerV2ProcessingServiceException actualEventListenerV2ProcessingServiceException =
                await Assert.ThrowsAsync<EventListenerV2ProcessingServiceException>(
                    retrieveEventListenerV2sByEventAddressIdTask.AsTask);

            // then
            actualEventListenerV2ProcessingServiceException.Should()
                .BeEquivalentTo(expectedEventListenerV2ProcessingServiceException);

            this.eventListenerV2ServiceMock.Verify(service =>
                service.RetrieveAllEventListenerV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventListenerV2ProcessingServiceException))),
                        Times.Once);

            this.eventListenerV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
