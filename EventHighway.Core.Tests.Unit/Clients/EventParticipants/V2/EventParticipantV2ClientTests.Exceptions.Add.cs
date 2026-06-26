// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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
        public async Task ShouldThrowValidationExceptionOnAddIfValidationExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventParticipantV2 someEventParticipantV2 = CreateRandomEventParticipantV2();
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
                service.AddEventParticipantV2Async(
                    someEventParticipantV2,
                    randomCancellationToken))
                        .ThrowsAsync(eventParticipantV2ValidationException);

            // when
            ValueTask<EventParticipantV2> addEventParticipantV2Task =
                this.eventParticipantV2Client.AddEventParticipantV2Async(
                    someEventParticipantV2,
                    randomCancellationToken);

            EventParticipantV2ClientValidationException
                actualEventParticipantV2ClientValidationException =
                    await Assert.ThrowsAsync<EventParticipantV2ClientValidationException>(
                        addEventParticipantV2Task.AsTask);

            // then
            actualEventParticipantV2ClientValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantV2ClientValidationException);

            this.eventParticipantV2ServiceMock.Verify(service =>
                service.AddEventParticipantV2Async(
                    someEventParticipantV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowValidationExceptionOnAddIfDependencyValidationExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventParticipantV2 someEventParticipantV2 = CreateRandomEventParticipantV2();
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
                service.AddEventParticipantV2Async(
                    someEventParticipantV2,
                    randomCancellationToken))
                        .ThrowsAsync(eventParticipantV2DependencyValidationException);

            // when
            ValueTask<EventParticipantV2> addEventParticipantV2Task =
                this.eventParticipantV2Client.AddEventParticipantV2Async(
                    someEventParticipantV2,
                    randomCancellationToken);

            EventParticipantV2ClientValidationException
                actualEventParticipantV2ClientValidationException =
                    await Assert.ThrowsAsync<EventParticipantV2ClientValidationException>(
                        addEventParticipantV2Task.AsTask);

            // then
            actualEventParticipantV2ClientValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantV2ClientValidationException);

            this.eventParticipantV2ServiceMock.Verify(service =>
                service.AddEventParticipantV2Async(
                    someEventParticipantV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventParticipantV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
