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

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertBulkEventArchiveV2sAsync(
                    inputEventArchiveV2s,
                    It.IsAny<CancellationToken>()))
                        .Returns(ValueTask.CompletedTask);

            // when
            await this.eventArchiveV2Service.BulkAddEventArchiveV2sAsync(
                inputEventArchiveV2s,
                TestContext.Current.CancellationToken);

            // then
            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                Times.Once);

            foreach (EventArchiveV2 item in inputEventArchiveV2s)
            {
                Assert.Equal(randomDateTime, item.ArchivedDate);
            }

            this.storageBrokerMock.Verify(broker =>
                broker.InsertBulkEventArchiveV2sAsync(
                    inputEventArchiveV2s,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
