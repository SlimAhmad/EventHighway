// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEvents.V2
{
    public partial class ListenerEventV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldBulkRestoreListenerEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEnumerable<ListenerEventV2> randomListenerEventV2s = CreateRandomListenerEventV2s();
            IEnumerable<ListenerEventV2> inputListenerEventV2s = randomListenerEventV2s;
            IEnumerable<ListenerEventV2> returnedListenerEventV2s = randomListenerEventV2s;
            IEnumerable<ListenerEventV2> expectedListenerEventV2s = returnedListenerEventV2s;

            this.listenerEventV2ServiceMock.Setup(service =>
                service.BulkRestoreListenerEventV2sAsync(
                    inputListenerEventV2s,
                    randomCancellationToken))
                        .ReturnsAsync(returnedListenerEventV2s);

            // when
            IEnumerable<ListenerEventV2> actualListenerEventV2s =
                await this.listenerEventV2ProcessingService.BulkRestoreListenerEventV2sAsync(
                    inputListenerEventV2s,
                    randomCancellationToken);

            // then
            actualListenerEventV2s.Should().BeEquivalentTo(expectedListenerEventV2s);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.BulkRestoreListenerEventV2sAsync(
                    inputListenerEventV2s,
                    randomCancellationToken),
                        Times.Once);

            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
