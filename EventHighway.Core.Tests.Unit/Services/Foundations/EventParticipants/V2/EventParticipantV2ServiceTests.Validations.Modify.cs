// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventParticipants.V2
{
    public partial class EventParticipantV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfEventParticipantV2IsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventParticipantV2 nullEventParticipantV2 = null;

            var nullEventParticipantV2Exception =
                new NullEventParticipantV2Exception(message: "Event participant is null.");

            var expectedEventParticipantV2ValidationException =
                new EventParticipantV2ValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: nullEventParticipantV2Exception);

            // when
            ValueTask<EventParticipantV2> modifyEventParticipantV2Task =
                this.eventParticipantV2Service.ModifyEventParticipantV2Async(
                    nullEventParticipantV2, randomCancellationToken);

            EventParticipantV2ValidationException actualEventParticipantV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantV2ValidationException>(
                    modifyEventParticipantV2Task.AsTask);

            // then
            actualEventParticipantV2ValidationException.Should().BeEquivalentTo(
                expectedEventParticipantV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2ValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventParticipantV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEventParticipantV2Async(
                    It.IsAny<EventParticipantV2>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfEventParticipantV2IsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var invalidEventParticipantV2 = new EventParticipantV2
            {
                Id = Guid.Empty,
                Name = invalidText
            };

            var invalidEventParticipantV2Exception =
                new InvalidEventParticipantV2Exception(
                    message: "Event participant is invalid, fix the errors and try again.");

            invalidEventParticipantV2Exception.AddData(
                key: nameof(EventParticipantV2.Id),
                values: "Required");

            invalidEventParticipantV2Exception.AddData(
                key: nameof(EventParticipantV2.Name),
                values: "Required");

            invalidEventParticipantV2Exception.AddData(
                key: nameof(EventParticipantV2.CreatedDate),
                values: "Required");

            invalidEventParticipantV2Exception.AddData(
                key: nameof(EventParticipantV2.UpdatedDate),

                values:
                [
                    "Required",
                    $"Date is the same as {nameof(EventParticipantV2.CreatedDate)}.",
                    "Date is not recent"
                ]);

            var expectedEventParticipantV2ValidationException =
                new EventParticipantV2ValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(GetRandomDateTimeOffset());

            // when
            ValueTask<EventParticipantV2> modifyEventParticipantV2Task =
                this.eventParticipantV2Service.ModifyEventParticipantV2Async(
                    invalidEventParticipantV2, randomCancellationToken);

            EventParticipantV2ValidationException actualEventParticipantV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantV2ValidationException>(
                    modifyEventParticipantV2Task.AsTask);

            // then
            actualEventParticipantV2ValidationException.Should().BeEquivalentTo(
                expectedEventParticipantV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventParticipantV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEventParticipantV2Async(
                    It.IsAny<EventParticipantV2>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            EventParticipantV2 randomEventParticipantV2 = CreateRandomEventParticipantV2(randomDateTimeOffset);
            EventParticipantV2 invalidEventParticipantV2 = randomEventParticipantV2;

            var invalidEventParticipantV2Exception =
                new InvalidEventParticipantV2Exception(
                    message: "Event participant is invalid, fix the errors and try again.");

            invalidEventParticipantV2Exception.AddData(
                key: nameof(EventParticipantV2.UpdatedDate),
                values: $"Date is the same as {nameof(EventParticipantV2.CreatedDate)}.");

            var expectedEventParticipantV2ValidationException =
                new EventParticipantV2ValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventParticipantV2> modifyEventParticipantV2Task =
                this.eventParticipantV2Service.ModifyEventParticipantV2Async(
                    invalidEventParticipantV2, randomCancellationToken);

            EventParticipantV2ValidationException actualEventParticipantV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantV2ValidationException>(
                    modifyEventParticipantV2Task.AsTask);

            // then
            actualEventParticipantV2ValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
               broker.GetDateTimeOffsetAsync(),
                   Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventParticipantV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEventParticipantV2Async(
                    It.IsAny<EventParticipantV2>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(-61)]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(
            int secondsBeforeOrAfter)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            int randomDaysAgo = GetRandomNegativeNumber();
            EventParticipantV2 randomEventParticipantV2 = CreateRandomEventParticipantV2(randomDateTimeOffset);
            EventParticipantV2 invalidEventParticipantV2 = randomEventParticipantV2;
            invalidEventParticipantV2.CreatedDate = randomDateTimeOffset.AddDays(randomDaysAgo);
            invalidEventParticipantV2.UpdatedDate = randomDateTimeOffset.AddSeconds(secondsBeforeOrAfter);

            var invalidEventParticipantV2Exception =
                new InvalidEventParticipantV2Exception(
                    message: "Event participant is invalid, fix the errors and try again.");

            invalidEventParticipantV2Exception.AddData(
                key: nameof(EventParticipantV2.UpdatedDate),
                values: "Date is not recent");

            var expectedEventParticipantV2ValidationException =
                new EventParticipantV2ValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventParticipantV2> modifyEventParticipantV2Task =
                this.eventParticipantV2Service.ModifyEventParticipantV2Async(
                    invalidEventParticipantV2, randomCancellationToken);

            EventParticipantV2ValidationException actualEventParticipantV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantV2ValidationException>(
                    modifyEventParticipantV2Task.AsTask);

            // then
            actualEventParticipantV2ValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventParticipantV2ByIdAsync(
                    It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEventParticipantV2Async(
                    It.IsAny<EventParticipantV2>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfEventParticipantV2DoesNotExistAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            int randomDaysAgo = GetRandomNegativeNumber();
            EventParticipantV2 randomEventParticipantV2 = CreateRandomEventParticipantV2(randomDateTime);
            EventParticipantV2 nonExistingEventParticipantV2 = randomEventParticipantV2;
            nonExistingEventParticipantV2.CreatedDate = randomDateTime.AddDays(randomDaysAgo);
            EventParticipantV2 nullEventParticipantV2 = null;

            var notFoundEventParticipantV2Exception =
                new NotFoundEventParticipantV2Exception(
                    message: $"Could not find event participant with id: {nonExistingEventParticipantV2.Id}.");

            var expectedEventParticipantV2ValidationException =
                new EventParticipantV2ValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: notFoundEventParticipantV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventParticipantV2ByIdAsync(
                    nonExistingEventParticipantV2.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(nullEventParticipantV2);

            // when
            ValueTask<EventParticipantV2> modifyEventParticipantV2Task =
                this.eventParticipantV2Service.ModifyEventParticipantV2Async(
                    nonExistingEventParticipantV2, randomCancellationToken);

            EventParticipantV2ValidationException actualEventParticipantV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantV2ValidationException>(
                    modifyEventParticipantV2Task.AsTask);

            // then
            actualEventParticipantV2ValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventParticipantV2ByIdAsync(
                    nonExistingEventParticipantV2.Id, It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEventParticipantV2Async(
                    It.IsAny<EventParticipantV2>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationErrorOnModifyIfStorageCreatedDateNotSameAsCreatedDateAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int randomDaysAgo = GetRandomNegativeNumber();
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            EventParticipantV2 randomEventParticipantV2 = CreateRandomEventParticipantV2(randomDateTime);
            EventParticipantV2 invalidEventParticipantV2 = randomEventParticipantV2;
            invalidEventParticipantV2.CreatedDate = randomDateTime.AddDays(randomDaysAgo);
            DateTimeOffset randomOtherDateTime = GetRandomDateTimeOffset();
            EventParticipantV2 storageEventParticipantV2 = invalidEventParticipantV2.DeepClone();
            storageEventParticipantV2.CreatedDate = randomOtherDateTime;

            var invalidEventParticipantV2Exception =
                new InvalidEventParticipantV2Exception(
                    message: "Event participant is invalid, fix the errors and try again.");

            invalidEventParticipantV2Exception.AddData(
                key: nameof(EventParticipantV2.CreatedDate),
                values: "Date is not the same as storage.");

            var expectedEventParticipantV2ValidationException =
                new EventParticipantV2ValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventParticipantV2ByIdAsync(
                    invalidEventParticipantV2.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(storageEventParticipantV2);

            // when
            ValueTask<EventParticipantV2> modifyEventParticipantV2Task =
                this.eventParticipantV2Service.ModifyEventParticipantV2Async(
                    invalidEventParticipantV2, randomCancellationToken);

            EventParticipantV2ValidationException actualEventParticipantV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantV2ValidationException>(
                    modifyEventParticipantV2Task.AsTask);

            // then
            actualEventParticipantV2ValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventParticipantV2ByIdAsync(
                    invalidEventParticipantV2.Id, It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2ValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsEarlierThanStorageAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int randomTimeAgo = GetRandomNegativeNumber();
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            DateTimeOffset earlierDateTime = randomDateTime.AddDays(randomTimeAgo);
            EventParticipantV2 randomEventParticipantV2 = CreateRandomEventParticipantV2(randomDateTime);
            EventParticipantV2 invalidEventParticipantV2 = randomEventParticipantV2;
            invalidEventParticipantV2.CreatedDate = earlierDateTime;
            EventParticipantV2 storageEventParticipantV2 = invalidEventParticipantV2.DeepClone();
            DateTimeOffset earlierSeconds = randomDateTime.AddSeconds(randomTimeAgo);
            invalidEventParticipantV2.UpdatedDate = earlierSeconds;

            var invalidEventParticipantV2Exception =
                new InvalidEventParticipantV2Exception(
                    message: "Event participant is invalid, fix the errors and try again.");

            invalidEventParticipantV2Exception.AddData(
                key: nameof(EventParticipantV2.UpdatedDate),
                values: "Date is earlier than storage.");

            var expectedEventParticipantV2ValidationException =
                new EventParticipantV2ValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventParticipantV2ByIdAsync(
                    invalidEventParticipantV2.Id, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(storageEventParticipantV2);

            // when
            ValueTask<EventParticipantV2> modifyEventParticipantV2Task =
                this.eventParticipantV2Service.ModifyEventParticipantV2Async(
                    invalidEventParticipantV2, randomCancellationToken);

            EventParticipantV2ValidationException actualEventParticipantV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantV2ValidationException>(
                    modifyEventParticipantV2Task.AsTask);

            // then
            actualEventParticipantV2ValidationException.Should()
                .BeEquivalentTo(expectedEventParticipantV2ValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventParticipantV2ByIdAsync(
                    invalidEventParticipantV2.Id, It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventParticipantV2ValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
