// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEvents.V2
{
    public partial class ListenerEventV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldBulkRemoveListenerEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<ListenerEventV2> randomListenerEventV2s = CreateRandomListenerEventV2s();
            IEnumerable<ListenerEventV2> inputListenerEventV2s = randomListenerEventV2s;

            this.listenerEventV2ServiceMock.Setup(service =>
                service.BulkRemoveListenerEventV2sAsync(
                    inputListenerEventV2s,
                    randomCancellationToken))
                        .Returns(ValueTask.CompletedTask);

            // when
            await this.listenerEventV2ProcessingService.BulkRemoveListenerEventV2sAsync(
                inputListenerEventV2s,
                randomCancellationToken);

            // then
            this.listenerEventV2ServiceMock.Verify(service =>
                service.BulkRemoveListenerEventV2sAsync(
                    inputListenerEventV2s,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
