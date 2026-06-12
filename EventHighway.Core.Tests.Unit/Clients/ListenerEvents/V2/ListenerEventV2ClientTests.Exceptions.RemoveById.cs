// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someListenerEventV2Id = GetRandomId();

            var expectedListenerEventV2ClientValidationException =
                new ListenerEventV2ClientValidationException(
                    message: "Listener event client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption,
                    data: (validationException.InnerException as Xeption).Data);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RemoveListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<ListenerEventV2> removeListenerEventV2ByIdTask =
                this.listenerEventV2Client.RemoveListenerEventV2ByIdAsync(
                    someListenerEventV2Id,
                    randomCancellationToken);

            ListenerEventV2ClientValidationException actualListenerEventV2ClientValidationException =
                await Assert.ThrowsAsync<ListenerEventV2ClientValidationException>(
                    removeListenerEventV2ByIdTask.AsTask);

            // then
            actualListenerEventV2ClientValidationException.Should()
                .BeEquivalentTo(expectedListenerEventV2ClientValidationException);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RemoveListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveByIdIfDependencyErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someListenerEventV2Id = GetRandomId();
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
                service.RemoveListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(eventListenerV2OrchestrationDependencyException);

            // when
            ValueTask<ListenerEventV2> removeListenerEventV2ByIdTask =
                this.listenerEventV2Client.RemoveListenerEventV2ByIdAsync(
                    someListenerEventV2Id,
                    randomCancellationToken);

            ListenerEventV2ClientDependencyException actualListenerEventV2ClientDependencyException =
                await Assert.ThrowsAsync<ListenerEventV2ClientDependencyException>(
                    removeListenerEventV2ByIdTask.AsTask);

            // then
            actualListenerEventV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV2ClientDependencyException);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RemoveListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveByIdIfServiceErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someListenerEventV2Id = GetRandomId();
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
                service.RemoveListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(eventListenerV2OrchestrationServiceException);

            // when
            ValueTask<ListenerEventV2> removeListenerEventV2ByIdTask =
                this.listenerEventV2Client.RemoveListenerEventV2ByIdAsync(
                    someListenerEventV2Id,
                    randomCancellationToken);

            ListenerEventV2ClientDependencyException actualListenerEventV2ClientDependencyException =
                await Assert.ThrowsAsync<ListenerEventV2ClientDependencyException>(
                    removeListenerEventV2ByIdTask.AsTask);

            // then
            actualListenerEventV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedListenerEventV2ClientDependencyException);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RemoveListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
