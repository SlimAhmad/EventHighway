// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
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
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnBulkAddIfValidationErrorOccursAndLogItAsync(
            Xeption listenerEventArchiveV2ValidationException)
        {
            // given
            IEnumerable<ListenerEventArchiveV2> someListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s().ToList();

            var expectedListenerEventArchiveV2ProcessingDependencyValidationException =
                new ListenerEventArchiveV2ProcessingDependencyValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: listenerEventArchiveV2ValidationException.InnerException as Xeption);

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    someListenerEventArchiveV2s,
                    TestContext.Current.CancellationToken))
                        .ThrowsAsync(listenerEventArchiveV2ValidationException);

            // when
            ValueTask<IEnumerable<ListenerEventArchiveV2>> bulkAddListenerEventArchiveV2sTask =
                this.listenerEventArchiveV2ProcessingService
                    .BulkAddListenerEventArchiveV2sAsync(
                        someListenerEventArchiveV2s,
                        TestContext.Current.CancellationToken);

            ListenerEventArchiveV2ProcessingDependencyValidationException
                actualListenerEventArchiveV2ProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<ListenerEventArchiveV2ProcessingDependencyValidationException>(
                        bulkAddListenerEventArchiveV2sTask.AsTask);

            // then
            actualListenerEventArchiveV2ProcessingDependencyValidationException.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV2ProcessingDependencyValidationException);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    someListenerEventArchiveV2s,
                    TestContext.Current.CancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV2ProcessingDependencyValidationException))),
                        Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnBulkAddIfDependencyErrorOccursAndLogItAsync(
            Xeption listenerEventArchiveV2DependencyException)
        {
            // given
            IEnumerable<ListenerEventArchiveV2> someListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s().ToList();

            var expectedListenerEventArchiveV2ProcessingDependencyException =
                new ListenerEventArchiveV2ProcessingDependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: listenerEventArchiveV2DependencyException.InnerException as Xeption);

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    someListenerEventArchiveV2s,
                    TestContext.Current.CancellationToken))
                        .ThrowsAsync(listenerEventArchiveV2DependencyException);

            // when
            ValueTask<IEnumerable<ListenerEventArchiveV2>> bulkAddListenerEventArchiveV2sTask =
                this.listenerEventArchiveV2ProcessingService
                    .BulkAddListenerEventArchiveV2sAsync(
                        someListenerEventArchiveV2s,
                        TestContext.Current.CancellationToken);

            ListenerEventArchiveV2ProcessingDependencyException
                actualListenerEventArchiveV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<ListenerEventArchiveV2ProcessingDependencyException>(
                        bulkAddListenerEventArchiveV2sTask.AsTask);

            // then
            actualListenerEventArchiveV2ProcessingDependencyException.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV2ProcessingDependencyException);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    someListenerEventArchiveV2s,
                    TestContext.Current.CancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV2ProcessingDependencyException))),
                        Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnBulkAddIfExceptionOccursAndLogItAsync()
        {
            // given
            IEnumerable<ListenerEventArchiveV2> someListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s().ToList();

            var serviceException = new Exception();

            var failedListenerEventArchiveV2ProcessingServiceException =
                new FailedListenerEventArchiveV2ProcessingServiceException(
                    message: "Failed listener event archive service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedListenerEventArchiveV2ProcessingServiceException =
                new ListenerEventArchiveV2ProcessingServiceException(
                    message: "Listener event archive service error occurred, contact support.",
                    innerException: failedListenerEventArchiveV2ProcessingServiceException);

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    someListenerEventArchiveV2s,
                    TestContext.Current.CancellationToken))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<IEnumerable<ListenerEventArchiveV2>> bulkAddListenerEventArchiveV2sTask =
                this.listenerEventArchiveV2ProcessingService
                    .BulkAddListenerEventArchiveV2sAsync(
                        someListenerEventArchiveV2s,
                        TestContext.Current.CancellationToken);

            ListenerEventArchiveV2ProcessingServiceException
                actualListenerEventArchiveV2ProcessingServiceException =
                    await Assert.ThrowsAsync<ListenerEventArchiveV2ProcessingServiceException>(
                        bulkAddListenerEventArchiveV2sTask.AsTask);

            // then
            actualListenerEventArchiveV2ProcessingServiceException.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV2ProcessingServiceException);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    someListenerEventArchiveV2s,
                    TestContext.Current.CancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV2ProcessingServiceException))),
                        Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
