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
        public async Task ShouldMarkEventV2AsImmediateAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 randomEventV2 =
                CreateRandomEventV2();

            EventV2 inputEventV2 = randomEventV2;
            EventV2 modifiedEventV2 = inputEventV2;

            EventV2 expectedEventV2 =
                modifiedEventV2.DeepClone();

            this.eventV2ProcessingServiceMock.Setup(service =>
                service.MarkEventV2AsImmediateAsync(inputEventV2, randomCancellationToken))
                    .ReturnsAsync(modifiedEventV2);

            // when
            EventV2 actualEventV2 =
                await this.eventV2OrchestrationService
                    .MarkEventV2AsImmediateAsync(
                        inputEventV2,
                        randomCancellationToken);

            // then
            actualEventV2.Should().BeEquivalentTo(
                expectedEventV2);

            this.eventV2ProcessingServiceMock.Verify(broker =>
                broker.MarkEventV2AsImmediateAsync(inputEventV2, randomCancellationToken),
                    Times.Once);

            this.eventV2ProcessingServiceMock
                .VerifyNoOtherCalls();

            this.eventAddressV2ProcessingServiceMock
                .VerifyNoOtherCalls();

            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
