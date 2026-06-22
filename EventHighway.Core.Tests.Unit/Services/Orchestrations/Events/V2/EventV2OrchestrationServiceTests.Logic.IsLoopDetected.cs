// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.Events.V2
{
    public partial class EventV2OrchestrationServiceTests
    {
        [Fact]
        public async Task ShouldIsLoopDetectedAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 inputEventV2 = randomEventV2;
            bool returnedResult = true;
            bool expectedResult = returnedResult;

            this.eventV2ProcessingServiceMock
                .Setup(service => service.IsLoopDetectedAsync(
                    inputEventV2,
                    randomCancellationToken))
                        .ReturnsAsync(returnedResult);

            // when
            bool actualResult =
                await this.eventV2OrchestrationService
                    .IsLoopDetectedAsync(
                        inputEventV2,
                        randomCancellationToken);

            // then
            actualResult.Should().Be(expectedResult);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.IsLoopDetectedAsync(
                    inputEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventAddressV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.hashBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
