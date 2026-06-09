// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventListeners.V2
{
    public partial class EventListenerV2ServiceTests
    {
        [Fact]
        private async Task ShouldRemoveEventListenerV2ByIdAsync()
        {
            // given
            CancellationToken cancellationToken =
                TestContext.Current.CancellationToken;

            Guid randomEventListenerV2Id = GetRandomId();
            Guid inputEventListenerV2Id = randomEventListenerV2Id;

            EventListenerV2 randomEventListenerV2 =
                CreateRandomEventListenerV2();

            EventListenerV2 retrievedEventListenerV2 =
                randomEventListenerV2;

            EventListenerV2 deletedEventListenerV2 =
                retrievedEventListenerV2;

            EventListenerV2 expectedEventListenerV2 =
                deletedEventListenerV2.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventListenerV2ByIdAsync(
                    inputEventListenerV2Id,
                    cancellationToken))
                        .ReturnsAsync(retrievedEventListenerV2);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteEventListenerV2Async(
                    retrievedEventListenerV2,
                    cancellationToken))
                        .ReturnsAsync(deletedEventListenerV2);

            // when
            EventListenerV2 actualEventListenerV2 =
                await this.eventListenerV2Service
                    .RemoveEventListenerV2ByIdAsync(
                        inputEventListenerV2Id,
                        cancellationToken);

            // then
            actualEventListenerV2.Should().BeEquivalentTo(
                expectedEventListenerV2);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventListenerV2ByIdAsync(
                    inputEventListenerV2Id,
                    cancellationToken),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteEventListenerV2Async(
                    retrievedEventListenerV2,
                    cancellationToken),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
