// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V1.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEventArchives.V1
{
    public partial class ListenerEventV1ArchiveServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfListenerEventV1ArchiveIsNullAndLogItAsync()
        {
            // given
            ListenerEventArchiveV1 nullListenerEventV1Archive = null;

            var nullListenerEventV1ArchiveException =
                new NullListenerEventArchiveV1Exception(message: "Listener event archive is null.");

            var expectedListenerEventV1ArchiveValidationException =
                new ListenerEventArchiveV1ValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: nullListenerEventV1ArchiveException);

            // when
            ValueTask<ListenerEventArchiveV1> addListenerEventV1ArchiveTask =
                this.listenerEventV1ArchiveService.AddListenerEventV1ArchiveAsync(nullListenerEventV1Archive);

            ListenerEventArchiveV1ValidationException actualListenerEventV1ArchiveValidationException =
                await Assert.ThrowsAsync<ListenerEventArchiveV1ValidationException>(
                    addListenerEventV1ArchiveTask.AsTask);

            // then
            actualListenerEventV1ArchiveValidationException.Should().BeEquivalentTo(
                expectedListenerEventV1ArchiveValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ArchiveValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventV1ArchiveAsync(It.IsAny<ListenerEventArchiveV1>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        private async Task ShouldThrowValidationExceptionOnAddIfListenerEventV1ArchiveIsInvalidAndLogItAsync(
            string invalidText)
        {
            ListenerEventArchiveV1Status invalidListenerEventV1ArchiveStatus =
                GetInvalidEnum<ListenerEventArchiveV1Status>();

            var invalidListenerEventV1Archive = new ListenerEventArchiveV1
            {
                Id = Guid.Empty,
                Response = invalidText,
                ResponseReasonPhrase = invalidText,
                Status = invalidListenerEventV1ArchiveStatus,
                EventId = Guid.Empty,
                EventAddressId = Guid.Empty,
                EventListenerId = Guid.Empty
            };

            var invalidListenerEventV1ArchiveException =
                new InvalidListenerEventArchiveV1Exception(
                    message: "Listener event archive is invalid, fix the errors and try again.");

            invalidListenerEventV1ArchiveException.AddData(
                key: nameof(ListenerEventArchiveV1.Id),
                values: "Required");

            invalidListenerEventV1ArchiveException.AddData(
                key: nameof(ListenerEventArchiveV1.EventId),
                values: "Required");

            invalidListenerEventV1ArchiveException.AddData(
                key: nameof(ListenerEventArchiveV1.EventAddressId),
                values: "Required");

            invalidListenerEventV1ArchiveException.AddData(
                key: nameof(ListenerEventArchiveV1.EventListenerId),
                values: "Required");

            invalidListenerEventV1ArchiveException.AddData(
                key: nameof(ListenerEventArchiveV1.Status),
                values: "Value is not recognized");

            invalidListenerEventV1ArchiveException.AddData(
                key: nameof(ListenerEventArchiveV1.CreatedDate),
                values: "Required");

            invalidListenerEventV1ArchiveException.AddData(
                key: nameof(ListenerEventArchiveV1.UpdatedDate),
                values: "Required");

            invalidListenerEventV1ArchiveException.AddData(
                key: nameof(ListenerEventArchiveV1.ArchivedDate),
                values: "Required");

            var expectedListenerEventV1ArchiveValidationException =
                new ListenerEventArchiveV1ValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventV1ArchiveException);

            // when
            ValueTask<ListenerEventArchiveV1> addListenerEventV1ArchiveTask =
                this.listenerEventV1ArchiveService.AddListenerEventV1ArchiveAsync(invalidListenerEventV1Archive);

            ListenerEventArchiveV1ValidationException actualListenerEventV1ArchiveValidationException =
                await Assert.ThrowsAsync<ListenerEventArchiveV1ValidationException>(
                    addListenerEventV1ArchiveTask.AsTask);

            // then
            actualListenerEventV1ArchiveValidationException.Should().BeEquivalentTo(
                expectedListenerEventV1ArchiveValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ArchiveValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventV1ArchiveAsync(It.IsAny<ListenerEventArchiveV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutesBeforeAndAfterNow))]
        public async Task ShouldThrowValidationExceptionOnAddIfArchivedDateIsNotRecentAndLogItAsync(
            int minutesBeforeAndAfter)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            ListenerEventArchiveV1 randomListenerEventV1Archive =
                CreateRandomListenerEventV1Archive(randomDateTimeOffset
                    .AddMinutes(minutes: minutesBeforeAndAfter));

            ListenerEventArchiveV1 invalidListenerEventV1Archive = randomListenerEventV1Archive;

            var invalidListenerEventV1ArchiveException =
                new InvalidListenerEventArchiveV1Exception(
                    message: "Listener event archive is invalid, fix the errors and try again.");

            invalidListenerEventV1ArchiveException.AddData(
                key: nameof(ListenerEventArchiveV1.ArchivedDate),
                values: "Date is not recent");

            var expectedListenerEventV1ArchiveValidationException =
                new ListenerEventArchiveV1ValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: invalidListenerEventV1ArchiveException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            // when
            ValueTask<ListenerEventArchiveV1> addListenerEventV1ArchiveTask =
                this.listenerEventV1ArchiveService.AddListenerEventV1ArchiveAsync(invalidListenerEventV1Archive);

            ListenerEventArchiveV1ValidationException actualListenerEventV1ArchiveValidationException =
                await Assert.ThrowsAsync<ListenerEventArchiveV1ValidationException>(
                    addListenerEventV1ArchiveTask.AsTask);

            // then
            actualListenerEventV1ArchiveValidationException.Should().BeEquivalentTo(
                expectedListenerEventV1ArchiveValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV1ArchiveValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventV1ArchiveAsync(It.IsAny<ListenerEventArchiveV1>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
