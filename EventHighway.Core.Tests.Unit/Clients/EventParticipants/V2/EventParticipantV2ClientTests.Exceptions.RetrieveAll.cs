// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventParticipants.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.EventParticipants.V2
{
    public partial class EventParticipantV2ClientTests
    {
        [Fact]
        public async Task
            ShouldThrowValidationExceptionOnRetrieveAllIfValidationExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var someInnerException = new Xeption(message: GetRandomString());

            var eventParticipantV2ValidationException =
                new EventParticipantV2ValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: someInnerException);

            var expectedEventParticipantV2ClientValidationException =
                new EventParticipantV2ClientValidationException(
                    message: "Event participant client validation error occurred, fix the errors and try again.",
                    innerException: someInnerException,
                    data: someInnerException.Data);

            this.eventParticipantV2ServiceMock.Setup(service =>
                service.RetrieveAllEventParticipantV2sAsync(randomCancellationToken))
                    .ThrowsAsync(eventParticipantV2ValidationException);

            // when
            ValueTask<IEnumerable<EventParticipantV2>> retrieveAllEventParticipantV2sTask =
                this.eventParticipantV2Client
                    .RetrieveAllEventParticipantV2sAsync(randomCancellationToken);

            EventParticipantV2ClientValidationException
                actualEventParticipantV2ClientValidationException =
                    await Assert.ThrowsAsync<EventParticipantV2ClientValidationException>(
                        retrieveAllEventParticipantV2sTask.AsTask);

            // then
            actualEventParticipantV2ClientValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantV2ClientValidationException);

            this.eventParticipantV2ServiceMock.Verify(service =>
                service.RetrieveAllEventParticipantV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowValidationExceptionOnRetrieveAllIfDependencyValidationExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var someInnerException = new Xeption(message: GetRandomString());

            var eventParticipantV2DependencyValidationException =
                new EventParticipantV2DependencyValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: someInnerException);

            var expectedEventParticipantV2ClientValidationException =
                new EventParticipantV2ClientValidationException(
                    message: "Event participant client validation error occurred, fix the errors and try again.",
                    innerException: someInnerException,
                    data: someInnerException.Data);

            this.eventParticipantV2ServiceMock.Setup(service =>
                service.RetrieveAllEventParticipantV2sAsync(randomCancellationToken))
                    .ThrowsAsync(eventParticipantV2DependencyValidationException);

            // when
            ValueTask<IEnumerable<EventParticipantV2>> retrieveAllEventParticipantV2sTask =
                this.eventParticipantV2Client
                    .RetrieveAllEventParticipantV2sAsync(randomCancellationToken);

            EventParticipantV2ClientValidationException
                actualEventParticipantV2ClientValidationException =
                    await Assert.ThrowsAsync<EventParticipantV2ClientValidationException>(
                        retrieveAllEventParticipantV2sTask.AsTask);

            // then
            actualEventParticipantV2ClientValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantV2ClientValidationException);

            this.eventParticipantV2ServiceMock.Verify(service =>
                service.RetrieveAllEventParticipantV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowDependencyExceptionOnRetrieveAllIfDependencyExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var someInnerException = new Xeption(message: GetRandomString());

            var eventParticipantV2DependencyException =
                new EventParticipantV2DependencyException(
                    message: "Event participant dependency error occurred, contact support.",
                    innerException: someInnerException);

            var expectedEventParticipantV2ClientDependencyException =
                new EventParticipantV2ClientDependencyException(
                    message: "Event participant client dependency error occurred, contact support.",
                    innerException: someInnerException,
                    data: someInnerException.Data);

            this.eventParticipantV2ServiceMock.Setup(service =>
                service.RetrieveAllEventParticipantV2sAsync(randomCancellationToken))
                    .ThrowsAsync(eventParticipantV2DependencyException);

            // when
            ValueTask<IEnumerable<EventParticipantV2>> retrieveAllEventParticipantV2sTask =
                this.eventParticipantV2Client
                    .RetrieveAllEventParticipantV2sAsync(randomCancellationToken);

            EventParticipantV2ClientDependencyException
                actualEventParticipantV2ClientDependencyException =
                    await Assert.ThrowsAsync<EventParticipantV2ClientDependencyException>(
                        retrieveAllEventParticipantV2sTask.AsTask);

            // then
            actualEventParticipantV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedEventParticipantV2ClientDependencyException);

            this.eventParticipantV2ServiceMock.Verify(service =>
                service.RetrieveAllEventParticipantV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowDependencyExceptionOnRetrieveAllIfServiceExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var someInnerException = new Xeption(message: GetRandomString());

            var eventParticipantV2ServiceException =
                new EventParticipantV2ServiceException(
                    message: "Event participant service error occurred, contact support.",
                    innerException: someInnerException);

            var expectedEventParticipantV2ClientDependencyException =
                new EventParticipantV2ClientDependencyException(
                    message: "Event participant client dependency error occurred, contact support.",
                    innerException: someInnerException,
                    data: someInnerException.Data);

            this.eventParticipantV2ServiceMock.Setup(service =>
                service.RetrieveAllEventParticipantV2sAsync(randomCancellationToken))
                    .ThrowsAsync(eventParticipantV2ServiceException);

            // when
            ValueTask<IEnumerable<EventParticipantV2>> retrieveAllEventParticipantV2sTask =
                this.eventParticipantV2Client
                    .RetrieveAllEventParticipantV2sAsync(randomCancellationToken);

            EventParticipantV2ClientDependencyException
                actualEventParticipantV2ClientDependencyException =
                    await Assert.ThrowsAsync<EventParticipantV2ClientDependencyException>(
                        retrieveAllEventParticipantV2sTask.AsTask);

            // then
            actualEventParticipantV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedEventParticipantV2ClientDependencyException);

            this.eventParticipantV2ServiceMock.Verify(service =>
                service.RetrieveAllEventParticipantV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowServiceExceptionOnRetrieveAllIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var someException = new Exception(message: GetRandomString());

            var expectedEventParticipantV2ClientServiceException =
                new EventParticipantV2ClientServiceException(
                    message: "Event participant client service error occurred, contact support.",
                    innerException: someException as Xeption,
                    data: someException.Data);

            this.eventParticipantV2ServiceMock.Setup(service =>
                service.RetrieveAllEventParticipantV2sAsync(randomCancellationToken))
                    .ThrowsAsync(someException);

            // when
            ValueTask<IEnumerable<EventParticipantV2>> retrieveAllEventParticipantV2sTask =
                this.eventParticipantV2Client
                    .RetrieveAllEventParticipantV2sAsync(randomCancellationToken);

            EventParticipantV2ClientServiceException
                actualEventParticipantV2ClientServiceException =
                    await Assert.ThrowsAsync<EventParticipantV2ClientServiceException>(
                        retrieveAllEventParticipantV2sTask.AsTask);

            // then
            actualEventParticipantV2ClientServiceException.Should()
                .BeEquivalentTo(expectedEventParticipantV2ClientServiceException);

            this.eventParticipantV2ServiceMock.Verify(service =>
                service.RetrieveAllEventParticipantV2sAsync(randomCancellationToken),
                    Times.Once);

            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
