// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Coordinations.Events.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.Events.V2
{
    public partial class EventV2ClientTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventV2Id = GetRandomId();

            var expectedEventV2ClientValidationException =
                new EventV2ClientValidationException(
                    message: "Event client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption,
                    data: (validationException.InnerException as Xeption).Data);

            this.eventV2CoordinationServiceMock.Setup(service =>
                service.RetrieveEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<EventV2> retrieveEventV2ByIdTask =
                this.eventV2Client.RetrieveEventV2ByIdAsync(
                    someEventV2Id,
                    randomCancellationToken);

            EventV2ClientValidationException actualEventV2ClientValidationException =
                await Assert.ThrowsAsync<EventV2ClientValidationException>(
                    retrieveEventV2ByIdTask.AsTask);

            // then
            actualEventV2ClientValidationException.Should()
                .BeEquivalentTo(expectedEventV2ClientValidationException);

            this.eventV2CoordinationServiceMock.Verify(service =>
                service.RetrieveEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveByIdIfDependencyErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventV2Id = GetRandomId();
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var eventV2CoordinationDependencyException =
                new EventV2CoordinationDependencyException(
                    someMessage,
                    someInnerException);

            var expectedEventV2ClientDependencyException =
                new EventV2ClientDependencyException(
                    message: "Event client dependency error occurred, contact support.",
                    innerException: eventV2CoordinationDependencyException.InnerException as Xeption,
                    data: (eventV2CoordinationDependencyException.InnerException as Xeption).Data);

            this.eventV2CoordinationServiceMock.Setup(service =>
                service.RetrieveEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(eventV2CoordinationDependencyException);

            // when
            ValueTask<EventV2> retrieveEventV2ByIdTask =
                this.eventV2Client.RetrieveEventV2ByIdAsync(
                    someEventV2Id,
                    randomCancellationToken);

            EventV2ClientDependencyException actualEventV2ClientDependencyException =
                await Assert.ThrowsAsync<EventV2ClientDependencyException>(
                    retrieveEventV2ByIdTask.AsTask);

            // then
            actualEventV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedEventV2ClientDependencyException);

            this.eventV2CoordinationServiceMock.Verify(service =>
                service.RetrieveEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveByIdIfServiceErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventV2Id = GetRandomId();
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var eventV2CoordinationServiceException =
                new EventV2CoordinationServiceException(
                    someMessage,
                    someInnerException);

            var expectedEventV2ClientDependencyException =
                new EventV2ClientDependencyException(
                    message: "Event client dependency error occurred, contact support.",
                    innerException: eventV2CoordinationServiceException.InnerException as Xeption,
                    data: (eventV2CoordinationServiceException.InnerException as Xeption).Data);

            this.eventV2CoordinationServiceMock.Setup(service =>
                service.RetrieveEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(eventV2CoordinationServiceException);

            // when
            ValueTask<EventV2> retrieveEventV2ByIdTask =
                this.eventV2Client.RetrieveEventV2ByIdAsync(
                    someEventV2Id,
                    randomCancellationToken);

            EventV2ClientDependencyException actualEventV2ClientDependencyException =
                await Assert.ThrowsAsync<EventV2ClientDependencyException>(
                    retrieveEventV2ByIdTask.AsTask);

            // then
            actualEventV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedEventV2ClientDependencyException);

            this.eventV2CoordinationServiceMock.Verify(service =>
                service.RetrieveEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfUnexpectedErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventV2Id = GetRandomId();

            var someXeption = new Xeption(message: GetRandomString());

            var expectedEventV2ClientServiceException =
                new EventV2ClientServiceException(
                    message: "Event client service error occurred, contact support.",
                    innerException: someXeption,
                    data: someXeption.Data);

            this.eventV2CoordinationServiceMock.Setup(service =>
                service.RetrieveEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(someXeption);

            // when
            ValueTask<EventV2> retrieveEventV2ByIdTask =
                this.eventV2Client.RetrieveEventV2ByIdAsync(
                    someEventV2Id,
                    randomCancellationToken);

            EventV2ClientServiceException actualEventV2ClientServiceException =
                await Assert.ThrowsAsync<EventV2ClientServiceException>(
                    retrieveEventV2ByIdTask.AsTask);

            // then
            actualEventV2ClientServiceException.Should()
                .BeEquivalentTo(expectedEventV2ClientServiceException);

            this.eventV2CoordinationServiceMock.Verify(service =>
                service.RetrieveEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnRetrieveByIdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventV2Id = GetRandomId();

            var operationCanceledException =
                new OperationCanceledException();

            this.eventV2CoordinationServiceMock.Setup(service =>
                service.RetrieveEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<EventV2> retrieveEventV2ByIdTask =
                this.eventV2Client.RetrieveEventV2ByIdAsync(
                    someEventV2Id,
                    randomCancellationToken);

            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    retrieveEventV2ByIdTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(operationCanceledException);

            this.eventV2CoordinationServiceMock.Verify(service =>
                service.RetrieveEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
