// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
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
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            IQueryable<EventArchiveV2> randomEventArchiveV2s =
                CreateRandomEventArchiveV2s();

            List<EventArchiveV2> inputEventArchiveV2s =
                randomEventArchiveV2s.ToList();

            List<EventArchiveV2> expectedEventArchiveV2s =
                inputEventArchiveV2s.Select(item => item.DeepClone()).ToList();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            foreach (EventArchiveV2 item in expectedEventArchiveV2s)
            {
                item.ArchivedDate = randomDateTime;
            }

            this.storageBrokerMock.Setup(broker =>
                broker.InsertBulkEventArchiveV2sAsync(
                    It.Is<List<EventArchiveV2>>(actual =>
                        SameEventArchiveV2sAs(expectedEventArchiveV2s, actual)),
                            It.IsAny<CancellationToken>()))
                                .Returns(ValueTask.CompletedTask);

            // when
            IEnumerable<EventArchiveV2> actualEventArchiveV2s =
                await this.eventArchiveV2Service.BulkAddEventArchiveV2sAsync(
                    inputEventArchiveV2s,
                        TestContext.Current.CancellationToken);

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
                broker.InsertBulkEventArchiveV2sAsync(
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
