// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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
        public async Task ShouldAddEventArchiveV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset =
                GetRandomDateTimeOffset();

            EventArchiveV2 randomEventArchiveV2 =
                CreateRandomEventArchiveV2(
                    date: randomDateTimeOffset);

            EventArchiveV2 inputEventArchiveV2 =
                randomEventArchiveV2;

            EventArchiveV2 insertedEventArchiveV2 =
                inputEventArchiveV2;

            EventArchiveV2 expectedEventArchiveV2 =
                insertedEventArchiveV2.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertEventArchiveV2Async(
                    inputEventArchiveV2,
                    randomCancellationToken))
                        .ReturnsAsync(insertedEventArchiveV2);

            // when
            EventArchiveV2 actualEventArchiveV2 =
                await this.eventArchiveV2Service
                    .AddEventArchiveV2Async(
                        inputEventArchiveV2,
                        randomCancellationToken);

            // then
            actualEventArchiveV2.Should().BeEquivalentTo(
                expectedEventArchiveV2);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertEventArchiveV2Async(
                    inputEventArchiveV2,
                    randomCancellationToken),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
