// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.Events.V2
{
    public partial class EventV2ServiceTests
    {
        [Fact]
        public async Task ShouldBulkRestoreEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<EventV2> randomEventV2s = CreateRandomRestoreEventV2s();
            List<EventV2> inputEventV2s = randomEventV2s;

            List<EventV2> expectedEventV2s =
                inputEventV2s.Select(item => item.DeepClone()).ToList();

            this.storageBrokerMock.Setup(broker =>
                broker.BulkInsertEventV2sAsync(
                    It.Is<List<EventV2>>(actual =>
                        SameEventV2sAs(expectedEventV2s, actual)),
                            randomCancellationToken))
                                .Returns(ValueTask.CompletedTask);

            // when
            IEnumerable<EventV2> actualEventV2s =
                await this.eventV2Service.BulkRestoreEventV2sAsync(
                    inputEventV2s,
                        randomCancellationToken);

            // then
            actualEventV2s.Should().BeEquivalentTo(expectedEventV2s);

            this.storageBrokerMock.Verify(broker =>
                broker.BulkInsertEventV2sAsync(
                    It.Is<List<EventV2>>(actual =>
                        SameEventV2sAs(expectedEventV2s, actual)),
                            randomCancellationToken),
                                Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
