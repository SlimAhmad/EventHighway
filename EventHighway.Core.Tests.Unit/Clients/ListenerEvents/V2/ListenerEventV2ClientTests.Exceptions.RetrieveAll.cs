// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.ListenerEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.ListenerEvents.V2
{
    public partial class ListenerEventV2ClientTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowValidationExceptionOnRetrieveAllIfValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedListenerEventV2ClientValidationException =
                new ListenerEventV2ClientValidationException(
                    message: "Listener event client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption,
                    data: (validationException.InnerException as Xeption).Data);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask<IQueryable<ListenerEventV2>> retrieveAllListenerEventV2sTask =
                this.listenerEventV2Client.RetrieveAllListenerEventV2sAsync(randomCancellationToken);

            ListenerEventV2ClientValidationException actualListenerEventV2ClientValidationException =
                await Assert.ThrowsAsync<ListenerEventV2ClientValidationException>(
                    retrieveAllListenerEventV2sTask.AsTask);

            // then
            actualListenerEventV2ClientValidationException.Should()
                .BeEquivalentTo(expectedListenerEventV2ClientValidationException);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAllIfDependencyErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var eventListenerV2OrchestrationDependencyException =
                new EventListenerV2OrchestrationDependencyException(
                    someMessage,
                    someInnerException);

            var expectedListenerEventV2ClientDependencyException =
                new ListenerEventV2ClientDependencyException(
                    message: "Listener event client dependency error occurred, contact support.",

                    innerException: eventListenerV2OrchestrationDependencyException
                        .InnerException as Xeption,

                    data: (eventListenerV2OrchestrationDependencyException
                        .InnerException as Xeption).Data);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(eventListenerV2OrchestrationDependencyException);

            // when
            ValueTask<IQueryable<ListenerEventV2>> retrieveAllListenerEventV2sTask =
                this.listenerEventV2Client.RetrieveAllListenerEventV2sAsync(randomCancellationToken);

            ListenerEventV2ClientDependencyException actualListenerEventV2ClientDependencyException =
                await Assert.ThrowsAsync<ListenerEventV2ClientDependencyException>(
                    retrieveAllListenerEventV2sTask.AsTask);

            // then
            actualListenerEventV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV2ClientDependencyException);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAllIfServiceErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var eventListenerV2OrchestrationServiceException =
                new EventListenerV2OrchestrationServiceException(
                    someMessage,
                    someInnerException);

            var expectedListenerEventV2ClientDependencyException =
                new ListenerEventV2ClientDependencyException(
                    message: "Listener event client dependency error occurred, contact support.",

                    innerException: eventListenerV2OrchestrationServiceException
                        .InnerException as Xeption,

                    data: (eventListenerV2OrchestrationServiceException
                        .InnerException as Xeption).Data);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(eventListenerV2OrchestrationServiceException);

            // when
            ValueTask<IQueryable<ListenerEventV2>> retrieveAllListenerEventV2sTask =
                this.listenerEventV2Client.RetrieveAllListenerEventV2sAsync(randomCancellationToken);

            ListenerEventV2ClientDependencyException actualListenerEventV2ClientDependencyException =
                await Assert.ThrowsAsync<ListenerEventV2ClientDependencyException>(
                    retrieveAllListenerEventV2sTask.AsTask);

            // then
            actualListenerEventV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV2ClientDependencyException);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveAllIfUnexpectedErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var someXeption = new Xeption(message: GetRandomString());

            var expectedListenerEventV2ClientServiceException =
                new ListenerEventV2ClientServiceException(
                    message: "Listener event client service error occurred, contact support.",
                    innerException: someXeption,
                    data: someXeption.Data);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(someXeption);

            // when
            ValueTask<IQueryable<ListenerEventV2>> retrieveAllListenerEventV2sTask =
                this.listenerEventV2Client.RetrieveAllListenerEventV2sAsync(randomCancellationToken);

            ListenerEventV2ClientServiceException actualListenerEventV2ClientServiceException =
                await Assert.ThrowsAsync<ListenerEventV2ClientServiceException>(
                    retrieveAllListenerEventV2sTask.AsTask);

            // then
            actualListenerEventV2ClientServiceException.Should()
                .BeEquivalentTo(expectedListenerEventV2ClientServiceException);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveAllAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var operationCanceledException =
                new OperationCanceledException();

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<IQueryable<ListenerEventV2>> retrieveAllListenerEventV2sTask =
                this.listenerEventV2Client.RetrieveAllListenerEventV2sAsync(randomCancellationToken);

            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveAllListenerEventV2sTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(operationCanceledException);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllListenerEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
