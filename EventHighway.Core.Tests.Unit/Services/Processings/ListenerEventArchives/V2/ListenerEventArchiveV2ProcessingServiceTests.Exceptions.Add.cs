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
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            ListenerEventArchiveV2 someListenerEventArchiveV2 = CreateRandomListenerEventArchiveV2();

            var expectedListenerEventArchiveV2ProcessingDependencyValidationException =
                new ListenerEventArchiveV2ProcessingDependencyValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.AddListenerEventArchiveV2Async(
                    It.IsAny<ListenerEventArchiveV2>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<ListenerEventArchiveV2> addListenerEventArchiveV2Task =
                this.listenerEventArchiveV2ProcessingService.AddListenerEventArchiveV2Async(
                    someListenerEventArchiveV2,
                    randomCancellationToken);

            ListenerEventArchiveV2ProcessingDependencyValidationException
                actualListenerEventArchiveV2ProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<ListenerEventArchiveV2ProcessingDependencyValidationException>(
                        addListenerEventArchiveV2Task.AsTask);

            // then
            actualListenerEventArchiveV2ProcessingDependencyValidationException.Should().BeEquivalentTo(
                expectedListenerEventArchiveV2ProcessingDependencyValidationException);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.AddListenerEventArchiveV2Async(
                    It.IsAny<ListenerEventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV2ProcessingDependencyValidationException))),
                        Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnAddIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken cancellationToken =
                TestContext.Current.CancellationToken;

            ListenerEventArchiveV2 someListenerEventArchiveV2 = CreateRandomListenerEventArchiveV2();

            var expectedListenerEventArchiveV2ProcessingDependencyException =
                new ListenerEventArchiveV2ProcessingDependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.AddListenerEventArchiveV2Async(
                    It.IsAny<ListenerEventArchiveV2>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<ListenerEventArchiveV2> addListenerEventArchiveV2Task =
                this.listenerEventArchiveV2ProcessingService.AddListenerEventArchiveV2Async(
                    someListenerEventArchiveV2,
                    cancellationToken);

            ListenerEventArchiveV2ProcessingDependencyException
                actualListenerEventArchiveV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<ListenerEventArchiveV2ProcessingDependencyException>(
                        addListenerEventArchiveV2Task.AsTask);

            // then
            actualListenerEventArchiveV2ProcessingDependencyException.Should().BeEquivalentTo(
                expectedListenerEventArchiveV2ProcessingDependencyException);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.AddListenerEventArchiveV2Async(
                    It.IsAny<ListenerEventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV2ProcessingDependencyException))),
                        Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken cancellationToken =
                TestContext.Current.CancellationToken;

            ListenerEventArchiveV2 someListenerEventArchiveV2 = CreateRandomListenerEventArchiveV2();
            var serviceException = new Exception();

            serviceException.Data.Add(
                key: GetRandomString(),
                value: new List<string> { GetRandomString() });

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
                service.AddListenerEventArchiveV2Async(
                    It.IsAny<ListenerEventArchiveV2>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<ListenerEventArchiveV2> addListenerEventArchiveV2Task =
                this.listenerEventArchiveV2ProcessingService.AddListenerEventArchiveV2Async(
                    someListenerEventArchiveV2,
                    cancellationToken);

            ListenerEventArchiveV2ProcessingServiceException
                actualListenerEventArchiveV2ProcessingServiceException =
                    await Assert.ThrowsAsync<ListenerEventArchiveV2ProcessingServiceException>(
                        addListenerEventArchiveV2Task.AsTask);

            // then
            actualListenerEventArchiveV2ProcessingServiceException.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV2ProcessingServiceException);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.AddListenerEventArchiveV2Async(
                    It.IsAny<ListenerEventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV2ProcessingServiceException))),
                        Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
