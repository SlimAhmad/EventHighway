// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventArchives.V1
{
    public partial class EventV1ArchiveServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfEventV1ArchiveIsNullAndLogItAsync()
        {
            // given
            EventArchiveV1 nullEventV1Archive = null;

            var nullEventV1ArchiveException =
                new NullEventArchiveV1Exception(
                    message: "Event archive is null.");

            var expectedEventV1ArchiveValidationException =
                new EventArchiveV1ValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: nullEventV1ArchiveException);

            // when
            ValueTask<EventArchiveV1> addEventV1ArchiveTask =
                this.eventV1ArchiveService.AddEventArchiveV1Async(nullEventV1Archive);

            EventArchiveV1ValidationException actualEventV1ArchiveValidationException =
                await Assert.ThrowsAsync<EventArchiveV1ValidationException>(
                    addEventV1ArchiveTask.AsTask);

            // then
            actualEventV1ArchiveValidationException.Should().BeEquivalentTo(
                expectedEventV1ArchiveValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventV1ArchiveAsync(
                    It.IsAny<EventArchiveV1>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        private async Task ShouldThrowValidationExceptionOnAddIfEventV1ArchiveIsInvalidAndLogItAsync(
            string invalidText)
        {
            EventArchiveV1Type invalidType = GetInvalidEnum<EventArchiveV1Type>();

            var invalidEventV1Archive = new EventArchiveV1
            {
                Id = Guid.Empty,
                Type = invalidType,
                Content = invalidText
            };

            var invalidEventV1ArchiveException =
                new InvalidEventArchiveV1Exception(
                    message: "Event archive is invalid, fix the errors and try again.");

            invalidEventV1ArchiveException.AddData(
                key: nameof(EventArchiveV1.Id),
                values: "Required");

            invalidEventV1ArchiveException.AddData(
                key: nameof(EventArchiveV1.Content),
                values: "Required");

            invalidEventV1ArchiveException.AddData(
                key: nameof(EventArchiveV1.Type),
                values: "Value is not recognized");

            invalidEventV1ArchiveException.AddData(
                key: nameof(EventArchiveV1.CreatedDate),
                values: "Required");

            invalidEventV1ArchiveException.AddData(
                key: nameof(EventArchiveV1.UpdatedDate),
                values: "Required");

            invalidEventV1ArchiveException.AddData(
                key: nameof(EventArchiveV1.ArchivedDate),
                values: "Required");

            invalidEventV1ArchiveException.AddData(
                key: nameof(EventArchiveV1.EventAddressId),
                values: "Required");

            var expectedEventV1ArchiveValidationException =
                new EventArchiveV1ValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV1ArchiveException);

            // when
            ValueTask<EventArchiveV1> addEventV1ArchiveTask =
                this.eventV1ArchiveService.AddEventArchiveV1Async(invalidEventV1Archive);

            EventArchiveV1ValidationException actualEventV1ArchiveValidationException =
                await Assert.ThrowsAsync<EventArchiveV1ValidationException>(
                    addEventV1ArchiveTask.AsTask);

            // then
            actualEventV1ArchiveValidationException.Should().BeEquivalentTo(
                expectedEventV1ArchiveValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventV1ArchiveAsync(
                    It.IsAny<EventArchiveV1>()),
                        Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutesBeforeAndAfterNow))]
        public async Task ShouldThrowValidationExceptionOnAddIfArchiveDateIsNotRecentAndLogItAsync(
            int minutesBeforeAndAfterNow)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            EventArchiveV1 randomEventV1Archive =
                CreateRandomEventV1Archive(
                    date: randomDateTimeOffset.AddMinutes(minutesBeforeAndAfterNow));

            EventArchiveV1 invalidEventV1Archive = randomEventV1Archive;

            var invalidEventV1ArchiveException =
                new InvalidEventArchiveV1Exception(
                    message: "Event archive is invalid, fix the errors and try again.");

            invalidEventV1ArchiveException.AddData(
                key: nameof(EventArchiveV1.ArchivedDate),
                values: "Date is not recent");

            var expectedEventV1ArchiveValidationException =
                new EventArchiveV1ValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidEventV1ArchiveException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<EventArchiveV1> addEventV1ArchiveTask =
                this.eventV1ArchiveService.AddEventArchiveV1Async(invalidEventV1Archive);

            EventArchiveV1ValidationException actualEventV1ArchiveValidationException =
                await Assert.ThrowsAsync<EventArchiveV1ValidationException>(
                    addEventV1ArchiveTask.AsTask);

            // then
            actualEventV1ArchiveValidationException.Should().BeEquivalentTo(
                expectedEventV1ArchiveValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventV1ArchiveAsync(It.IsAny<EventArchiveV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
