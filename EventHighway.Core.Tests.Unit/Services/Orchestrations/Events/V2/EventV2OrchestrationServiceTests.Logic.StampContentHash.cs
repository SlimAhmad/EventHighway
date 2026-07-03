// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.Events.V2
{
    public partial class EventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldStampContentHashOnStampContentHashAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var mockSequence = new MockSequence();
            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;
            string randomCleanedContent = GetRandomString();
            string randomHash = GetRandomString();
            EventV2 expectedEventV2 = inputEventV2.DeepClone();
            expectedEventV2.ContentHash = randomHash;

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

            // when
            EventV2 actualEventV2 =
                await this.eventV2OrchestrationService
                    .StampContentHashAsync(
                        inputEventV2,
                        randomCancellationToken);

            // then
            actualEventV2.Should().BeEquivalentTo(expectedEventV2);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RemoveVolatilePathsAsync(
                    inputEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.hashBrokerMock.Verify(broker =>
                broker.GenerateSha256Hash(randomCleanedContent),
                    Times.Once);

            this.eventAddressV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
