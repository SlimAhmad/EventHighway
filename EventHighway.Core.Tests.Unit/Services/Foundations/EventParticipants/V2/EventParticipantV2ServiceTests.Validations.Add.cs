// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2;
using EventHighway.Core.Models.Services.Foundations.EventParticipants.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventParticipants.V2
{
    public partial class EventParticipantV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfEventParticipantV2IsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventParticipantV2 nullEventParticipantV2 = null;

            var nullEventParticipantV2Exception =
                new NullEventParticipantV2Exception(
                    message: "Event participant is null.");

            var expectedEventParticipantV2ValidationException =
                new EventParticipantV2ValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: nullEventParticipantV2Exception);

            // when
            ValueTask<EventParticipantV2> addEventParticipantV2Task =
                this.eventParticipantV2Service.AddEventParticipantV2Async(
                    nullEventParticipantV2, randomCancellationToken);

            EventParticipantV2ValidationException actualEventParticipantV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantV2ValidationException>(
                    addEventParticipantV2Task.AsTask);

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
                broker.InsertEventParticipantV2Async(
                    It.IsAny<EventParticipantV2>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotSameAsUpdatedDateAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            DateTimeOffset anotherRandomDateTimeOffset =
                GetRandomDateTimeOffset();

            EventParticipantV2 invalidEventParticipantV2 =
                CreateRandomEventParticipantV2(randomDateTimeOffset);

            invalidEventParticipantV2.UpdatedDate = anotherRandomDateTimeOffset;

            var invalidEventParticipantV2Exception =
                new InvalidEventParticipantV2Exception(
                    message: "Event participant is invalid, fix the errors and try again.");

            invalidEventParticipantV2Exception.AddData(
                key: nameof(EventParticipantV2.CreatedDate),
                values: $"Date is not the same as {nameof(EventParticipantV2.UpdatedDate)}");

            var expectedEventParticipantV2ValidationException =
                new EventParticipantV2ValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventParticipantV2> addEventParticipantV2Task =
                this.eventParticipantV2Service.AddEventParticipantV2Async(
                    invalidEventParticipantV2, randomCancellationToken);

            EventParticipantV2ValidationException actualEventParticipantV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantV2ValidationException>(
                    addEventParticipantV2Task.AsTask);

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
                broker.InsertEventParticipantV2Async(
                    It.IsAny<EventParticipantV2>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotRecentAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            DateTimeOffset invalidDateTimeOffset =
                randomDateTimeOffset.AddMinutes(GetRandomNegativeNumber());

            EventParticipantV2 invalidEventParticipantV2 =
                CreateRandomEventParticipantV2(invalidDateTimeOffset);


            var invalidEventParticipantV2Exception =
                new InvalidEventParticipantV2Exception(
                    message: "Event participant is invalid, fix the errors and try again.");

            invalidEventParticipantV2Exception.AddData(
                key: nameof(EventParticipantV2.CreatedDate),
                values: "Date is not recent");

            var expectedEventParticipantV2ValidationException =
                new EventParticipantV2ValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventParticipantV2> addEventParticipantV2Task =
                this.eventParticipantV2Service.AddEventParticipantV2Async(
                    invalidEventParticipantV2, randomCancellationToken);

            EventParticipantV2ValidationException actualEventParticipantV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantV2ValidationException>(
                    addEventParticipantV2Task.AsTask);

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
                broker.InsertEventParticipantV2Async(
                    It.IsAny<EventParticipantV2>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfEventParticipantV2IsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var invalidEventParticipantV2 = new EventParticipantV2
            {
                Id = Guid.Empty,
                Name = invalidText,
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
                values: "Required");

            var expectedEventParticipantV2ValidationException =
                new EventParticipantV2ValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantV2Exception);

            // when
            ValueTask<EventParticipantV2> addEventParticipantV2Task =
                this.eventParticipantV2Service.AddEventParticipantV2Async(
                    invalidEventParticipantV2, randomCancellationToken);

            EventParticipantV2ValidationException actualEventParticipantV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantV2ValidationException>(
                    addEventParticipantV2Task.AsTask);

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
                broker.InsertEventParticipantV2Async(
                    It.IsAny<EventParticipantV2>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfActiveFromIsSetToDefaultAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            EventParticipantV2 invalidEventParticipantV2 =
                CreateRandomEventParticipantV2(randomDateTimeOffset);

            invalidEventParticipantV2.ActiveFrom = default(DateTimeOffset);

            var invalidEventParticipantV2Exception =
                new InvalidEventParticipantV2Exception(
                    message: "Event participant is invalid, fix the errors and try again.");

            invalidEventParticipantV2Exception.AddData(
                key: nameof(EventParticipantV2.ActiveFrom),
                values: "Required");

            var expectedEventParticipantV2ValidationException =
                new EventParticipantV2ValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventParticipantV2> addEventParticipantV2Task =
                this.eventParticipantV2Service.AddEventParticipantV2Async(
                    invalidEventParticipantV2, randomCancellationToken);

            EventParticipantV2ValidationException actualEventParticipantV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantV2ValidationException>(
                    addEventParticipantV2Task.AsTask);

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
                broker.InsertEventParticipantV2Async(
                    It.IsAny<EventParticipantV2>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfActiveToIsSetToDefaultAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            EventParticipantV2 invalidEventParticipantV2 =
                CreateRandomEventParticipantV2(randomDateTimeOffset);

            invalidEventParticipantV2.ActiveTo = default(DateTimeOffset);

            var invalidEventParticipantV2Exception =
                new InvalidEventParticipantV2Exception(
                    message: "Event participant is invalid, fix the errors and try again.");

            invalidEventParticipantV2Exception.AddData(
                key: nameof(EventParticipantV2.ActiveTo),
                values: "Required");

            var expectedEventParticipantV2ValidationException =
                new EventParticipantV2ValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventParticipantV2> addEventParticipantV2Task =
                this.eventParticipantV2Service.AddEventParticipantV2Async(
                    invalidEventParticipantV2, randomCancellationToken);

            EventParticipantV2ValidationException actualEventParticipantV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantV2ValidationException>(
                    addEventParticipantV2Task.AsTask);

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
                broker.InsertEventParticipantV2Async(
                    It.IsAny<EventParticipantV2>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfActiveToIsNotAfterActiveFromAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            DateTimeOffset activeFrom = randomDateTimeOffset;

            DateTimeOffset activeTo =
                randomDateTimeOffset.AddMinutes(GetRandomNegativeNumber());

            EventParticipantV2 invalidEventParticipantV2 =
                CreateRandomEventParticipantV2(randomDateTimeOffset);

            invalidEventParticipantV2.ActiveFrom = activeFrom;
            invalidEventParticipantV2.ActiveTo = activeTo;

            var invalidEventParticipantV2Exception =
                new InvalidEventParticipantV2Exception(
                    message: "Event participant is invalid, fix the errors and try again.");

            invalidEventParticipantV2Exception.AddData(
                key: nameof(EventParticipantV2.ActiveTo),
                values: $"Date must be after {nameof(EventParticipantV2.ActiveFrom)}");

            var expectedEventParticipantV2ValidationException =
                new EventParticipantV2ValidationException(
                    message: "Event participant validation error occurred, fix the errors and try again.",
                    innerException: invalidEventParticipantV2Exception);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventParticipantV2> addEventParticipantV2Task =
                this.eventParticipantV2Service.AddEventParticipantV2Async(
                    invalidEventParticipantV2, randomCancellationToken);

            EventParticipantV2ValidationException actualEventParticipantV2ValidationException =
                await Assert.ThrowsAsync<EventParticipantV2ValidationException>(
                    addEventParticipantV2Task.AsTask);

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
                broker.InsertEventParticipantV2Async(
                    It.IsAny<EventParticipantV2>(), It.IsAny<CancellationToken>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
