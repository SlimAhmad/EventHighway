// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2.Exceptions;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventArchives.V2
{
    public partial class EventArchiveV2ServiceTests
    {
        [Fact]
        public async Task ShouldBulkAddEventArchiveV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken = TestContext.Current.CancellationToken;
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            IQueryable<EventArchiveV2> randomEventArchiveV2s =
                CreateRandomEventArchiveV2s();

            List<EventArchiveV2> inputEventArchiveV2s =
                randomEventArchiveV2s.ToList();

            List<EventArchiveV2> expectedEventArchiveV2s =
                inputEventArchiveV2s.Select(item => item.DeepClone()).ToList();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(new List<EventArchiveV2>().AsQueryable());

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            foreach (EventArchiveV2 item in expectedEventArchiveV2s)
            {
                item.ArchivedDate = randomDateTime;
            }

            this.storageBrokerMock.Setup(broker =>
                broker.BulkInsertEventArchiveV2sAsync(
                    It.Is<List<EventArchiveV2>>(actual =>
                        SameEventArchiveV2sAs(expectedEventArchiveV2s, actual)),
                            It.IsAny<CancellationToken>()))
                                .Returns(ValueTask.CompletedTask);

            // when
            IEnumerable<EventArchiveV2> actualEventArchiveV2s =
                await this.eventArchiveV2Service.BulkAddEventArchiveV2sAsync(
                    inputEventArchiveV2s,
                        randomCancellationToken);

            // then
            actualEventArchiveV2s.Should().BeEquivalentTo(expectedEventArchiveV2s);

            foreach (EventArchiveV2 item in inputEventArchiveV2s)
            {
                Assert.Equal(randomDateTime, item.ArchivedDate);
            }

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Exactly(inputEventArchiveV2s.Count + 1));

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.BulkInsertEventArchiveV2sAsync(
                    It.Is<List<EventArchiveV2>>(actual =>
                        SameEventArchiveV2sAs(expectedEventArchiveV2s, actual)),
                            It.IsAny<CancellationToken>()),
                                Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldBulkAddOnlyNonExistingEventArchiveV2sAndReturnAllAsync()
        {
            // given
            CancellationToken randomCancellationToken = TestContext.Current.CancellationToken;
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();

            List<EventArchiveV2> existingEventArchiveV2s =
                CreateRandomEventArchiveV2s().ToList();

            List<EventArchiveV2> newEventArchiveV2s =
                CreateRandomEventArchiveV2s().ToList();

            List<EventArchiveV2> inputEventArchiveV2s =
                existingEventArchiveV2s.Concat(newEventArchiveV2s).ToList();

            IQueryable<EventArchiveV2> storedEventArchiveV2s =
                existingEventArchiveV2s.AsQueryable();

            List<EventArchiveV2> expectedItemsToBulkAdd =
                newEventArchiveV2s.Select(item => item.DeepClone()).ToList();

            foreach (EventArchiveV2 item in expectedItemsToBulkAdd)
            {
                item.ArchivedDate = randomDateTime;
            }

            List<EventArchiveV2> expectedEventArchiveV2s =
                existingEventArchiveV2s.Concat(expectedItemsToBulkAdd).ToList();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(storedEventArchiveV2s);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.BulkInsertEventArchiveV2sAsync(
                    It.Is<List<EventArchiveV2>>(actual =>
                        SameEventArchiveV2sAs(expectedItemsToBulkAdd, actual)),
                            It.IsAny<CancellationToken>()))
                                .Returns(ValueTask.CompletedTask);

            // when
            IEnumerable<EventArchiveV2> actualEventArchiveV2s =
                await this.eventArchiveV2Service.BulkAddEventArchiveV2sAsync(
                    inputEventArchiveV2s,
                        randomCancellationToken);

            // then
            actualEventArchiveV2s.Should().BeEquivalentTo(expectedEventArchiveV2s);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Exactly(newEventArchiveV2s.Count + 1));

            this.storageBrokerMock.Verify(broker =>
                broker.BulkInsertEventArchiveV2sAsync(
                    It.Is<List<EventArchiveV2>>(actual =>
                        SameEventArchiveV2sAs(expectedItemsToBulkAdd, actual)),
                            It.IsAny<CancellationToken>()),
                                Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldBulkAddValidEventArchiveV2sAndLogInvalidOnesAsync()
        {
            // given
            CancellationToken randomCancellationToken = TestContext.Current.CancellationToken;
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();

            List<EventArchiveV2> validEventArchiveV2s =
                CreateRandomEventArchiveV2s().ToList();

            EventArchiveV2 invalidEventArchiveV2 =
                CreateRandomEventArchiveV2();

            invalidEventArchiveV2.Id = Guid.Empty;

            List<EventArchiveV2> inputEventArchiveV2s =
                validEventArchiveV2s
                    .Append(invalidEventArchiveV2).ToList();

            List<EventArchiveV2> expectedEventArchiveV2s =
                validEventArchiveV2s.Select(item => item.DeepClone()).ToList();

            foreach (EventArchiveV2 item in expectedEventArchiveV2s)
            {
                item.ArchivedDate = randomDateTime;
            }

            var invalidEventArchiveV2Exception =
                new InvalidEventArchiveV2Exception(
                    message: "Event archive is invalid, fix the errors and try again.");

            invalidEventArchiveV2Exception.UpsertDataList(
                key: nameof(EventArchiveV2.Id),
                value: "Required");

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(new List<EventArchiveV2>().AsQueryable());

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.BulkInsertEventArchiveV2sAsync(
                    It.Is<List<EventArchiveV2>>(actual =>
                        SameEventArchiveV2sAs(expectedEventArchiveV2s, actual)),
                            It.IsAny<CancellationToken>()))
                                .Returns(ValueTask.CompletedTask);

            // when
            IEnumerable<EventArchiveV2> actualEventArchiveV2s =
                await this.eventArchiveV2Service.BulkAddEventArchiveV2sAsync(
                    inputEventArchiveV2s,
                        randomCancellationToken);

            // then
            actualEventArchiveV2s.Should().BeEquivalentTo(expectedEventArchiveV2s);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Exactly(inputEventArchiveV2s.Count + 1));

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    invalidEventArchiveV2Exception))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.BulkInsertEventArchiveV2sAsync(
                    It.Is<List<EventArchiveV2>>(actual =>
                        SameEventArchiveV2sAs(expectedEventArchiveV2s, actual)),
                            It.IsAny<CancellationToken>()),
                                Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
