// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
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
        public async Task ShouldThrowValidationExceptionOnReplayIfValidationErrorOccursAsync(
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
                service.ReplayEventArchiveV2sAsync(
                    It.IsAny<Guid?>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(validationException);

            // when
            ValueTask replayTask =
                this.replayingEventV2Client.ReplayEventArchiveV2sAsync(
                    null, null, null, null, randomCancellationToken);

            ReplayingEventV2ClientValidationException actualException =
                await Assert.ThrowsAsync<ReplayingEventV2ClientValidationException>(
                    replayTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedReplayingEventV2ClientValidationException);

            this.replayingEventV2CoordinationServiceMock.Verify(service =>
                service.ReplayEventArchiveV2sAsync(
                    It.IsAny<Guid?>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            this.replayingEventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnReplayIfDependencyErrorOccursAsync()
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
                service.ReplayEventArchiveV2sAsync(
                    It.IsAny<Guid?>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(replayingEventV2CoordinationDependencyException);

            // when
            ValueTask replayTask =
                this.replayingEventV2Client.ReplayEventArchiveV2sAsync(
                    null, null, null, null, randomCancellationToken);

            ReplayingEventV2ClientDependencyException actualException =
                await Assert.ThrowsAsync<ReplayingEventV2ClientDependencyException>(
                    replayTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedReplayingEventV2ClientDependencyException);

            this.replayingEventV2CoordinationServiceMock.Verify(service =>
                service.ReplayEventArchiveV2sAsync(
                    It.IsAny<Guid?>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            this.replayingEventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnReplayIfServiceErrorOccursAsync()
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
                service.ReplayEventArchiveV2sAsync(
                    It.IsAny<Guid?>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(replayingEventV2CoordinationServiceException);

            // when
            ValueTask replayTask =
                this.replayingEventV2Client.ReplayEventArchiveV2sAsync(
                    null, null, null, null, randomCancellationToken);

            ReplayingEventV2ClientDependencyException actualException =
                await Assert.ThrowsAsync<ReplayingEventV2ClientDependencyException>(
                    replayTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedReplayingEventV2ClientDependencyException);

            this.replayingEventV2CoordinationServiceMock.Verify(service =>
                service.ReplayEventArchiveV2sAsync(
                    It.IsAny<Guid?>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            this.replayingEventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnReplayIfUnexpectedErrorOccursAsync()
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
                service.ReplayEventArchiveV2sAsync(
                    It.IsAny<Guid?>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(someXeption);

            // when
            ValueTask replayTask =
                this.replayingEventV2Client.ReplayEventArchiveV2sAsync(
                    null, null, null, null, randomCancellationToken);

            ReplayingEventV2ClientServiceException actualException =
                await Assert.ThrowsAsync<ReplayingEventV2ClientServiceException>(
                    replayTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedReplayingEventV2ClientServiceException);

            this.replayingEventV2CoordinationServiceMock.Verify(service =>
                service.ReplayEventArchiveV2sAsync(
                    It.IsAny<Guid?>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            this.replayingEventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnReplayAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var operationCanceledException =
                new OperationCanceledException();

            this.replayingEventV2CoordinationServiceMock.Setup(service =>
                service.ReplayEventArchiveV2sAsync(
                    It.IsAny<Guid?>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(operationCanceledException);

            // when
            ValueTask replayTask =
                this.replayingEventV2Client.ReplayEventArchiveV2sAsync(
                    null, null, null, null, randomCancellationToken);

            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    replayTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                operationCanceledException);

            this.replayingEventV2CoordinationServiceMock.Verify(service =>
                service.ReplayEventArchiveV2sAsync(
                    It.IsAny<Guid?>(),
                    It.IsAny<IEnumerable<Guid>>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<DateTimeOffset?>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            this.replayingEventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
