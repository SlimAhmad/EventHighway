// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventArchives.V2
{
    public partial class EventArchiveV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldAddEventArchiveV2Async()
        {
            // given
            CancellationToken cancellationToken =
                TestContext.Current.CancellationToken;

            EventArchiveV2 randomEventArchiveV2 =
                CreateRandomEventArchiveV2();

            EventArchiveV2 inputEventArchiveV2 =
                randomEventArchiveV2;

            EventArchiveV2 addedEventArchiveV2 =
                inputEventArchiveV2;

            EventArchiveV2 expectedEventArchiveV2 =
                addedEventArchiveV2.DeepClone();

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.AddEventArchiveV2Async(
                    inputEventArchiveV2,
                    cancellationToken))
                        .ReturnsAsync(addedEventArchiveV2);

            // when
            EventArchiveV2 actualEventArchiveV2 =
                await this.eventArchiveV2ProcessingService
                    .AddEventArchiveV2Async(
                        inputEventArchiveV2,
                        cancellationToken);

            // then
            actualEventArchiveV2.Should().BeEquivalentTo(
                expectedEventArchiveV2);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.AddEventArchiveV2Async(
                    inputEventArchiveV2,
                    cancellationToken),
                        Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
