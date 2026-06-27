// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.ReplayingEvents.V2.Exceptions;
using EventHighway.Core.Models.Coordinations.ReplayingEvents.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.ReplayingEvents.V2
{
    public partial class ReplayingEventV2ClientTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowValidationExceptionOnProcessReplayedIfValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedReplayingEventV2ClientValidationException =
                new ReplayingEventV2ClientValidationException(
                    message: "Replaying event client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption,
                    data: (validationException.InnerException as Xeption).Data);

            this.replayingEventV2CoordinationServiceMock.Setup(service =>
                service.ProcessReplayedListenerEventV2sAsync(randomCancellationToken))
                    .ThrowsAsync(validationException);

            // when
            ValueTask processReplayedTask =
                this.replayingEventV2Client
                    .ProcessReplayedListenerEventV2sAsync(randomCancellationToken);

            ReplayingEventV2ClientValidationException actualException =
                await Assert.ThrowsAsync<ReplayingEventV2ClientValidationException>(
                    processReplayedTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedReplayingEventV2ClientValidationException);

            this.replayingEventV2CoordinationServiceMock.Verify(service =>
                service.ProcessReplayedListenerEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.replayingEventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnProcessReplayedIfDependencyErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var replayingEventV2CoordinationDependencyException =
                new ReplayingEventV2CoordinationDependencyException(
                    someMessage, someInnerException);

            var expectedReplayingEventV2ClientDependencyException =
                new ReplayingEventV2ClientDependencyException(
                    message: "Replaying event client dependency error occurred, contact support.",
                    innerException: replayingEventV2CoordinationDependencyException.InnerException as Xeption,
                    data: (replayingEventV2CoordinationDependencyException.InnerException as Xeption).Data);

            this.replayingEventV2CoordinationServiceMock.Setup(service =>
                service.ProcessReplayedListenerEventV2sAsync(randomCancellationToken))
                    .ThrowsAsync(replayingEventV2CoordinationDependencyException);

            // when
            ValueTask processReplayedTask =
                this.replayingEventV2Client
                    .ProcessReplayedListenerEventV2sAsync(randomCancellationToken);

            ReplayingEventV2ClientDependencyException actualException =
                await Assert.ThrowsAsync<ReplayingEventV2ClientDependencyException>(
                    processReplayedTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedReplayingEventV2ClientDependencyException);

            this.replayingEventV2CoordinationServiceMock.Verify(service =>
                service.ProcessReplayedListenerEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.replayingEventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnProcessReplayedIfServiceErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var replayingEventV2CoordinationServiceException =
                new ReplayingEventV2CoordinationServiceException(
                    someMessage, someInnerException);

            var expectedReplayingEventV2ClientDependencyException =
                new ReplayingEventV2ClientDependencyException(
                    message: "Replaying event client dependency error occurred, contact support.",
                    innerException: replayingEventV2CoordinationServiceException.InnerException as Xeption,
                    data: (replayingEventV2CoordinationServiceException.InnerException as Xeption).Data);

            this.replayingEventV2CoordinationServiceMock.Setup(service =>
                service.ProcessReplayedListenerEventV2sAsync(randomCancellationToken))
                    .ThrowsAsync(replayingEventV2CoordinationServiceException);

            // when
            ValueTask processReplayedTask =
                this.replayingEventV2Client
                    .ProcessReplayedListenerEventV2sAsync(randomCancellationToken);

            ReplayingEventV2ClientDependencyException actualException =
                await Assert.ThrowsAsync<ReplayingEventV2ClientDependencyException>(
                    processReplayedTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedReplayingEventV2ClientDependencyException);

            this.replayingEventV2CoordinationServiceMock.Verify(service =>
                service.ProcessReplayedListenerEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.replayingEventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnProcessReplayedIfUnexpectedErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var someXeption = new Xeption(message: GetRandomString());

            var expectedReplayingEventV2ClientServiceException =
                new ReplayingEventV2ClientServiceException(
                    message: "Replaying event client service error occurred, contact support.",
                    innerException: someXeption,
                    data: someXeption.Data);

            this.replayingEventV2CoordinationServiceMock.Setup(service =>
                service.ProcessReplayedListenerEventV2sAsync(randomCancellationToken))
                    .ThrowsAsync(someXeption);

            // when
            ValueTask processReplayedTask =
                this.replayingEventV2Client
                    .ProcessReplayedListenerEventV2sAsync(randomCancellationToken);

            ReplayingEventV2ClientServiceException actualException =
                await Assert.ThrowsAsync<ReplayingEventV2ClientServiceException>(
                    processReplayedTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedReplayingEventV2ClientServiceException);

            this.replayingEventV2CoordinationServiceMock.Verify(service =>
                service.ProcessReplayedListenerEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.replayingEventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnProcessReplayedAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var operationCanceledException =
                new OperationCanceledException();

            this.replayingEventV2CoordinationServiceMock.Setup(service =>
                service.ProcessReplayedListenerEventV2sAsync(randomCancellationToken))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask processReplayedTask =
                this.replayingEventV2Client
                    .ProcessReplayedListenerEventV2sAsync(randomCancellationToken);

            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    processReplayedTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                operationCanceledException);

            this.replayingEventV2CoordinationServiceMock.Verify(service =>
                service.ProcessReplayedListenerEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.replayingEventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
