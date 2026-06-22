// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.Events.V2
{
    public partial class EventV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveEventV2CountBySignatureAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 randomEventV2 =
                CreateRandomEventV2();

            EventV2 inputEventV2 = randomEventV2;
            int randomCount = GetRandomNumber();
            int expectedCount = randomCount;

            this.eventV2ServiceMock
                .Setup(service => service.RetrieveEventV2CountBySignatureAsync(
                    inputEventV2,
                    randomCancellationToken))
                        .ReturnsAsync(randomCount);

            // when
            int actualCount =
                await this.eventV2ProcessingService
                    .RetrieveEventV2CountBySignatureAsync(
                        inputEventV2,
                        randomCancellationToken);

            // then
            actualCount.Should().Be(expectedCount);

            this.eventV2ServiceMock.Verify(service =>
                service.RetrieveEventV2CountBySignatureAsync(
                    inputEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
