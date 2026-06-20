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
        public async Task ShouldRemoveEventArchiveV2ByIdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid randomEventArchiveV2Id = GetRandomId();
            Guid inputEventArchiveV2Id = randomEventArchiveV2Id;

            EventArchiveV2 randomEventArchiveV2 =
                CreateRandomEventArchiveV2();

            EventArchiveV2 retrievedEventArchiveV2 =
                randomEventArchiveV2;

            EventArchiveV2 deletedEventArchiveV2 =
                retrievedEventArchiveV2;

            EventArchiveV2 expectedEventArchiveV2 =
                deletedEventArchiveV2.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventArchiveV2ByIdAsync(
                    inputEventArchiveV2Id,
                    randomCancellationToken))
                        .ReturnsAsync(retrievedEventArchiveV2);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteEventArchiveV2Async(
                    retrievedEventArchiveV2,
                    randomCancellationToken))
                        .ReturnsAsync(deletedEventArchiveV2);

            // when
            EventArchiveV2 actualEventArchiveV2 =
                await this.eventArchiveV2Service
                    .RemoveEventArchiveV2ByIdAsync(
                        inputEventArchiveV2Id,
                        randomCancellationToken);

            // then
            actualEventArchiveV2.Should().BeEquivalentTo(
                expectedEventArchiveV2);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventArchiveV2ByIdAsync(
                    inputEventArchiveV2Id,
                    randomCancellationToken),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteEventArchiveV2Async(
                    retrievedEventArchiveV2,
                    randomCancellationToken),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
