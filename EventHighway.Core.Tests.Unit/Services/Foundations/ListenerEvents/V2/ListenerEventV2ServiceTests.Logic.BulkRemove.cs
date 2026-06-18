// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEvents.V2
{
    public partial class ListenerEventV2ServiceTests
    {
        [Fact]
        public async Task ShouldBulkRemoveListenerEventV2sAsync()
        {
            // given
            IQueryable<ListenerEventV2> randomListenerEventV2s =
                CreateRandomListenerEventV2s();

            IEnumerable<ListenerEventV2> inputListenerEventV2s =
                randomListenerEventV2s;

            this.storageBrokerMock.Setup(broker =>
                broker.BulkDeleteListenerEventV2sAsync(
                    inputListenerEventV2s,
                    It.IsAny<CancellationToken>()))
                        .Returns(ValueTask.CompletedTask);

            // when
            await this.listenerEventV2Service.BulkRemoveListenerEventV2sAsync(
                inputListenerEventV2s,
                TestContext.Current.CancellationToken);

            // then
            this.storageBrokerMock.Verify(broker =>
                broker.BulkDeleteListenerEventV2sAsync(
                    inputListenerEventV2s,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
