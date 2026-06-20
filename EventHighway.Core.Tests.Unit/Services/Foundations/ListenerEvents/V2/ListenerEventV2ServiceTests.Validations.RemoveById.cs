// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEvents.V2
{
    public partial class ListenerEventV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid invalidListenerEventV2Id = Guid.Empty;

            var invalidListenerEventV2Exception =
                new InvalidListenerEventV2Exception(
                    message: "Listener event is invalid, fix the errors and try again.");

            invalidListenerEventV2Exception.AddData(
                key: nameof(ListenerEventV2.Id),
                values: "Required");

            var expectedListenerEventV2ValidationException =
                new ListenerEventV2ValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventV2Exception);

            // when
            ValueTask<ListenerEventV2> removeListenerEventV2ByIdTask =
                this.listenerEventV2Service.RemoveListenerEventV2ByIdAsync(
                    invalidListenerEventV2Id,
                    randomCancellationToken);

            ListenerEventV2ValidationException actualListenerEventV2ValidationException =
                await Assert.ThrowsAsync<ListenerEventV2ValidationException>(
                    removeListenerEventV2ByIdTask.AsTask);

            // then
            actualListenerEventV2ValidationException.Should()
                .BeEquivalentTo(expectedListenerEventV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnRemoveByIdIfListenerEventV2IsNotFoundAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid nonExistingListenerEventV2Id = GetRandomId();
            ListenerEventV2 nullListenerEventV2 = null;

            var notFoundListenerEventV2Exception =
                new NotFoundListenerEventV2Exception(
                    message: $"Could not find listener event with id: {nonExistingListenerEventV2Id}.");

            var expectedListenerEventV2ValidationException =
                new ListenerEventV2ValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: notFoundListenerEventV2Exception);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(nullListenerEventV2);

            // when
            ValueTask<ListenerEventV2> removeListenerEventV2ByIdTask =
                this.listenerEventV2Service.RemoveListenerEventV2ByIdAsync(
                    nonExistingListenerEventV2Id,
                    randomCancellationToken);

            ListenerEventV2ValidationException actualListenerEventV2ValidationException =
                await Assert.ThrowsAsync<ListenerEventV2ValidationException>(
                    removeListenerEventV2ByIdTask.AsTask);

            // then
            actualListenerEventV2ValidationException.Should()
                .BeEquivalentTo(expectedListenerEventV2ValidationException);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteListenerEventV2Async(
                    It.IsAny<ListenerEventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
