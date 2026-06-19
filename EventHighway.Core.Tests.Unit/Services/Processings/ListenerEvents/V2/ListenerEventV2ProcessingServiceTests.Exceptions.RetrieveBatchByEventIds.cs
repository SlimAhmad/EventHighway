// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task ShouldThrowDependencyExceptionOnRetrieveBatchByEventIdsIfDependencyErrorOccursAndLogItAsync(
            Xeption listenerEventV2DependencyException)
        {
            // given
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
                service.RetrieveListenerEventV2sByEventIdsAsync(someEventIds))
                    .ThrowsAsync(listenerEventV2DependencyException);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> retrieveBatchByEventIdsTask =
                this.listenerEventV2ProcessingService
                    .RetrieveBatchOfListenerEventV2sByEventIdsAsync(someEventIds, someTake);

            ListenerEventV2ProcessingDependencyException
                actualListenerEventV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<ListenerEventV2ProcessingDependencyException>(
                        retrieveBatchByEventIdsTask.AsTask);

            // then
            actualListenerEventV2ProcessingDependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV2ProcessingDependencyException);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.RetrieveListenerEventV2sByEventIdsAsync(someEventIds),
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
                service.RetrieveListenerEventV2sByEventIdsAsync(someEventIds))
                    .ThrowsAsync(listenerEventV2ValidationException);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> retrieveBatchByEventIdsTask =
                this.listenerEventV2ProcessingService
                    .RetrieveBatchOfListenerEventV2sByEventIdsAsync(someEventIds, someTake);

            ListenerEventV2ProcessingDependencyValidationException
                actualListenerEventV2ProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<ListenerEventV2ProcessingDependencyValidationException>(
                        retrieveBatchByEventIdsTask.AsTask);

            // then
            actualListenerEventV2ProcessingDependencyValidationException.Should()
                .BeEquivalentTo(expectedListenerEventV2ProcessingDependencyValidationException);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.RetrieveListenerEventV2sByEventIdsAsync(someEventIds),
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
