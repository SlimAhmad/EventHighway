// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEvents.V2
{
    public partial class ListenerEventV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldAddListenerEventV2Async()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            ListenerEventV2 randomListenerEventV2 =
                CreateRandomListenerEventV2();

            ListenerEventV2 inputListenerEventV2 =
                randomListenerEventV2;

            ListenerEventV2 addedListenerEventV2 =
                inputListenerEventV2;

            ListenerEventV2 expectedListenerEventV2 =
                addedListenerEventV2.DeepClone();

            this.listenerEventV2ServiceMock.Setup(broker =>
                broker.AddListenerEventV2Async(
                    inputListenerEventV2,
                    randomCancellationToken))
                        .ReturnsAsync(addedListenerEventV2);

            // when
            ListenerEventV2 actualListenerEventV2 =
                await this.listenerEventV2ProcessingService
                    .AddListenerEventV2Async(
                        inputListenerEventV2,
                        randomCancellationToken);

            // then
            actualListenerEventV2.Should().BeEquivalentTo(
                expectedListenerEventV2);

            this.listenerEventV2ServiceMock.Verify(broker =>
                broker.AddListenerEventV2Async(
                    inputListenerEventV2,
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
