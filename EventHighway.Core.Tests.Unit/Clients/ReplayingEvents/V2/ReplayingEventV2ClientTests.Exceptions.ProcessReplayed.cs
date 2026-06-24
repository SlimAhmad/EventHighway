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
    }
}
