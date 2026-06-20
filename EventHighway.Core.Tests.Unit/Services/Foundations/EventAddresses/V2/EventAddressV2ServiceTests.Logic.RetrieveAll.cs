// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventAddresses.V2
{
    public partial class EventAddressV2ServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveAllEventAddressV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<EventAddressV2> randomEventAddressV2s =
                CreateRandomEventAddressV2s();

            IQueryable<EventAddressV2> retrievedEventAddressV2s =
                randomEventAddressV2s;

            IQueryable<EventAddressV2> expectedEventAddressV2s =
                randomEventAddressV2s.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllEventAddressV2sAsync(randomCancellationToken))
                    .ReturnsAsync(retrievedEventAddressV2s);

            // when
            IQueryable<EventAddressV2> actualEventAddressV2s =
                await this.eventAddressV2Service
                    .RetrieveAllEventAddressV2sAsync(randomCancellationToken);

            // then
            actualEventAddressV2s.Should().BeEquivalentTo(
                expectedEventAddressV2s);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllEventAddressV2sAsync(randomCancellationToken),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
