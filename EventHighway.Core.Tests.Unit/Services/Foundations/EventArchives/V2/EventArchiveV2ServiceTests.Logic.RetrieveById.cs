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
        public async Task ShouldRetrieveEventArchiveV2ByIdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid randomEventArchiveV2Id = GetRandomId();
            Guid inputEventArchiveV2Id = randomEventArchiveV2Id;

            EventArchiveV2 randomEventArchiveV2 =
                CreateRandomEventArchiveV2();

            EventArchiveV2 selectedEventArchiveV2 =
                randomEventArchiveV2;

            EventArchiveV2 expectedEventArchiveV2 =
                selectedEventArchiveV2.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventArchiveV2ByIdAsync(
                    inputEventArchiveV2Id,
                    randomCancellationToken))
                        .ReturnsAsync(selectedEventArchiveV2);

            // when
            EventArchiveV2 actualEventArchiveV2 =
                await this.eventArchiveV2Service
                    .RetrieveEventArchiveV2ByIdAsync(
                        inputEventArchiveV2Id,
                        randomCancellationToken);

            // then
            actualEventArchiveV2.Should()
                .BeEquivalentTo(expectedEventArchiveV2);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventArchiveV2ByIdAsync(
                    inputEventArchiveV2Id,
                    randomCancellationToken),
                        Times.Once());

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
