// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2.Exceptions;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEventArchives.V2
{
    public partial class ListenerEventArchiveV2ServiceTests
    {
        [Fact]
        public async Task ShouldBulkAddListenerEventArchiveV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken = TestContext.Current.CancellationToken;
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            IQueryable<ListenerEventArchiveV2> randomListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s();

            List<ListenerEventArchiveV2> inputListenerEventArchiveV2s =
                randomListenerEventArchiveV2s.ToList();

            List<ListenerEventArchiveV2> expectedListenerEventArchiveV2s =
                inputListenerEventArchiveV2s.Select(item => item.DeepClone()).ToList();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(new List<ListenerEventArchiveV2>().AsQueryable());

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            foreach (ListenerEventArchiveV2 item in expectedListenerEventArchiveV2s)
            {
                item.ArchivedDate = randomDateTime;
            }

            this.storageBrokerMock.Setup(broker =>
                broker.BulkInsertListenerEventArchiveV2sAsync(
                    It.Is<List<ListenerEventArchiveV2>>(actual =>
                        SameListenerEventArchiveV2sAs(expectedListenerEventArchiveV2s, actual)),
                            randomCancellationToken))
                                .Returns(ValueTask.CompletedTask);

            // when
            IEnumerable<ListenerEventArchiveV2> actualListenerEventArchiveV2s =
                await this.listenerEventArchiveV2Service.BulkAddListenerEventArchiveV2sAsync(
                    inputListenerEventArchiveV2s,
                        randomCancellationToken);

            // then
            actualListenerEventArchiveV2s.Should().BeEquivalentTo(expectedListenerEventArchiveV2s);

            foreach (ListenerEventArchiveV2 item in inputListenerEventArchiveV2s)
            {
                Assert.Equal(randomDateTime, item.ArchivedDate);
            }

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Exactly(inputListenerEventArchiveV2s.Count + 1));

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllListenerEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.BulkInsertListenerEventArchiveV2sAsync(
                    It.Is<List<ListenerEventArchiveV2>>(actual =>
                        SameListenerEventArchiveV2sAs(expectedListenerEventArchiveV2s, actual)),
                            randomCancellationToken),
                                Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldBulkAddOnlyNonExistingListenerEventArchiveV2sAndReturnAllAsync()
        {
            // given
            CancellationToken randomCancellationToken = TestContext.Current.CancellationToken;
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();

            List<ListenerEventArchiveV2> existingListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s().ToList();

            List<ListenerEventArchiveV2> newListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s().ToList();

            List<ListenerEventArchiveV2> inputListenerEventArchiveV2s =
                existingListenerEventArchiveV2s.Concat(newListenerEventArchiveV2s).ToList();

            IQueryable<ListenerEventArchiveV2> storedListenerEventArchiveV2s =
                existingListenerEventArchiveV2s.AsQueryable();

            List<ListenerEventArchiveV2> expectedItemsToBulkAdd =
                newListenerEventArchiveV2s.Select(item => item.DeepClone()).ToList();

            foreach (ListenerEventArchiveV2 item in expectedItemsToBulkAdd)
            {
                item.ArchivedDate = randomDateTime;
            }

            List<ListenerEventArchiveV2> expectedListenerEventArchiveV2s =
                existingListenerEventArchiveV2s.Concat(expectedItemsToBulkAdd).ToList();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(storedListenerEventArchiveV2s);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.BulkInsertListenerEventArchiveV2sAsync(
                    It.Is<List<ListenerEventArchiveV2>>(actual =>
                        SameListenerEventArchiveV2sAs(expectedItemsToBulkAdd, actual)),
                            randomCancellationToken))
                                .Returns(ValueTask.CompletedTask);

            // when
            IEnumerable<ListenerEventArchiveV2> actualListenerEventArchiveV2s =
                await this.listenerEventArchiveV2Service.BulkAddListenerEventArchiveV2sAsync(
                    inputListenerEventArchiveV2s,
                        randomCancellationToken);

            // then
            actualListenerEventArchiveV2s.Should().BeEquivalentTo(expectedListenerEventArchiveV2s);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllListenerEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Exactly(newListenerEventArchiveV2s.Count + 1));

            this.storageBrokerMock.Verify(broker =>
                broker.BulkInsertListenerEventArchiveV2sAsync(
                    It.Is<List<ListenerEventArchiveV2>>(actual =>
                        SameListenerEventArchiveV2sAs(expectedItemsToBulkAdd, actual)),
                            randomCancellationToken),
                                Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldBulkAddValidListenerEventArchiveV2sAndLogInvalidOnesAsync()
        {
            // given
            CancellationToken randomCancellationToken = TestContext.Current.CancellationToken;
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();

            List<ListenerEventArchiveV2> validListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s().ToList();

            ListenerEventArchiveV2 invalidListenerEventArchiveV2 =
                CreateRandomListenerEventArchiveV2();

            invalidListenerEventArchiveV2.Id = Guid.Empty;

            List<ListenerEventArchiveV2> inputListenerEventArchiveV2s =
                validListenerEventArchiveV2s
                    .Append(invalidListenerEventArchiveV2).ToList();

            List<ListenerEventArchiveV2> expectedListenerEventArchiveV2s =
                validListenerEventArchiveV2s.Select(item => item.DeepClone()).ToList();

            foreach (ListenerEventArchiveV2 item in expectedListenerEventArchiveV2s)
            {
                item.ArchivedDate = randomDateTime;
            }

            var invalidListenerEventArchiveV2Exception =
                new InvalidListenerEventArchiveV2Exception(
                    message: "Listener event archive is invalid, fix the errors and try again.");

            invalidListenerEventArchiveV2Exception.UpsertDataList(
                key: nameof(ListenerEventArchiveV2.Id),
                value: "Required");

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllListenerEventArchiveV2sAsync(randomCancellationToken))
                    .ReturnsAsync(new List<ListenerEventArchiveV2>().AsQueryable());

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.BulkInsertListenerEventArchiveV2sAsync(
                    It.Is<List<ListenerEventArchiveV2>>(actual =>
                        SameListenerEventArchiveV2sAs(expectedListenerEventArchiveV2s, actual)),
                            randomCancellationToken))
                                .Returns(ValueTask.CompletedTask);

            // when
            IEnumerable<ListenerEventArchiveV2> actualListenerEventArchiveV2s =
                await this.listenerEventArchiveV2Service.BulkAddListenerEventArchiveV2sAsync(
                    inputListenerEventArchiveV2s,
                        randomCancellationToken);

            // then
            actualListenerEventArchiveV2s.Should().BeEquivalentTo(expectedListenerEventArchiveV2s);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Exactly(inputListenerEventArchiveV2s.Count + 1));

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    invalidListenerEventArchiveV2Exception))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllListenerEventArchiveV2sAsync(randomCancellationToken),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.BulkInsertListenerEventArchiveV2sAsync(
                    It.Is<List<ListenerEventArchiveV2>>(actual =>
                        SameListenerEventArchiveV2sAs(expectedListenerEventArchiveV2s, actual)),
                            randomCancellationToken),
                                Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
