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
        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveBatchByEventIdsIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<Guid> someEventIds =
                Enumerable.Range(0, GetRandomNumber())
                    .Select(_ => Guid.NewGuid())
                        .ToList();

            int someTake = GetRandomNumber();
            var serviceException = new Exception();

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
                service.RetrieveListenerEventV2sByEventIdsAsync(someEventIds, It.IsAny<CancellationToken>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> retrieveBatchByEventIdsTask =
                this.listenerEventV2ProcessingService
                    .RetrieveBatchOfListenerEventV2sByEventIdsAsync(someEventIds, someTake, randomCancellationToken);

            ListenerEventV2ProcessingServiceException
                actualListenerEventV2ProcessingServiceException =
                    await Assert.ThrowsAsync<ListenerEventV2ProcessingServiceException>(
                        retrieveBatchByEventIdsTask.AsTask);

            // then
            actualListenerEventV2ProcessingServiceException.Should()
                .BeEquivalentTo(expectedListenerEventV2ProcessingServiceException);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.RetrieveListenerEventV2sByEventIdsAsync(someEventIds, It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2ProcessingServiceException))),
                        Times.Once);

            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveBatchByEventIdsIfDependencyErrorOccursAndLogItAsync(
            Xeption listenerEventV2DependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<Guid> someEventIds =
                Enumerable.Range(0, GetRandomNumber())
                    .Select(_ => Guid.NewGuid())
                        .ToList();

            int someTake = GetRandomNumber();

            var expectedListenerEventV2ProcessingDependencyException =
                new ListenerEventV2ProcessingDependencyException(
                    message: "Listener event dependency error occurred, contact support.",
                    innerException: listenerEventV2DependencyException.InnerException as Xeption);

            this.listenerEventV2ServiceMock.Setup(service =>
                service.RetrieveListenerEventV2sByEventIdsAsync(someEventIds, It.IsAny<CancellationToken>()))
                    .ThrowsAsync(listenerEventV2DependencyException);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> retrieveBatchByEventIdsTask =
                this.listenerEventV2ProcessingService
                    .RetrieveBatchOfListenerEventV2sByEventIdsAsync(someEventIds, someTake, randomCancellationToken);

            ListenerEventV2ProcessingDependencyException
                actualListenerEventV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<ListenerEventV2ProcessingDependencyException>(
                        retrieveBatchByEventIdsTask.AsTask);

            // then
            actualListenerEventV2ProcessingDependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV2ProcessingDependencyException);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.RetrieveListenerEventV2sByEventIdsAsync(someEventIds, It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2ProcessingDependencyException))),
                        Times.Once);

            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnRetrieveBatchByEventIdsIfValidationErrorOccursAndLogItAsync(
            Xeption listenerEventV2ValidationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<Guid> someEventIds =
                Enumerable.Range(0, GetRandomNumber())
                    .Select(_ => Guid.NewGuid())
                        .ToList();

            int someTake = GetRandomNumber();

            var expectedListenerEventV2ProcessingDependencyValidationException =
                new ListenerEventV2ProcessingDependencyValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: listenerEventV2ValidationException.InnerException as Xeption);

            this.listenerEventV2ServiceMock.Setup(service =>
                service.RetrieveListenerEventV2sByEventIdsAsync(someEventIds, It.IsAny<CancellationToken>()))
                    .ThrowsAsync(listenerEventV2ValidationException);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> retrieveBatchByEventIdsTask =
                this.listenerEventV2ProcessingService
                    .RetrieveBatchOfListenerEventV2sByEventIdsAsync(someEventIds, someTake, randomCancellationToken);

            ListenerEventV2ProcessingDependencyValidationException
                actualListenerEventV2ProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<ListenerEventV2ProcessingDependencyValidationException>(
                        retrieveBatchByEventIdsTask.AsTask);

            // then
            actualListenerEventV2ProcessingDependencyValidationException.Should()
                .BeEquivalentTo(expectedListenerEventV2ProcessingDependencyValidationException);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.RetrieveListenerEventV2sByEventIdsAsync(someEventIds, It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2ProcessingDependencyValidationException))),
                        Times.Once);

            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
