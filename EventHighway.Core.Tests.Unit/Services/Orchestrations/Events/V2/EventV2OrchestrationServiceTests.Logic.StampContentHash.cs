// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.Events.V2
{
    public partial class EventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldStampContentHashOnSubmitEventV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var mockSequence = new MockSequence();
            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;
            EventAddressV2 randomEventAddressV2 = CreateRandomEventAddressV2();
            EventAddressV2 retrievedEventAddressV2 = randomEventAddressV2;
            string randomCleanedContent = GetRandomString();
            string randomHash = GetRandomString();
            EventV2 addedEventV2 = inputEventV2;
            EventV2 expectedEventV2 = addedEventV2.DeepClone();
            expectedEventV2.ContentHash = randomHash;

            this.eventAddressV2ProcessingServiceMock
                .InSequence(mockSequence)
                .Setup(service => service.RetrieveEventAddressV2ByIdAsync(
                    inputEventV2.EventAddressId,
                    randomCancellationToken))
                        .ReturnsAsync(retrievedEventAddressV2);

            this.eventV2ProcessingServiceMock
                .InSequence(mockSequence)
                .Setup(service => service.RemoveVolatilePathsAsync(
                    inputEventV2,
                    randomCancellationToken))
                        .ReturnsAsync(randomCleanedContent);

            this.hashBrokerMock
                .InSequence(mockSequence)
                .Setup(broker => broker.GenerateSha256Hash(randomCleanedContent))
                    .Returns(randomHash);

            this.eventV2ProcessingServiceMock
                .InSequence(mockSequence)
                .Setup(service => service.AddEventV2Async(
                    inputEventV2,
                    randomCancellationToken))
                        .ReturnsAsync(addedEventV2);

            // when
            EventV2 actualEventV2 =
                await this.eventV2OrchestrationService
                    .SubmitEventV2Async(
                        inputEventV2,
                        randomCancellationToken);

            // then
            actualEventV2.Should().BeEquivalentTo(expectedEventV2);

            this.eventAddressV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventAddressV2ByIdAsync(
                    inputEventV2.EventAddressId,
                    randomCancellationToken),
                        Times.Once);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RemoveVolatilePathsAsync(
                    inputEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.hashBrokerMock.Verify(broker =>
                broker.GenerateSha256Hash(randomCleanedContent),
                    Times.Once);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.AddEventV2Async(
                    inputEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventAddressV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
