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
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<ListenerEventV2> randomListenerEventV2s =
                CreateRandomListenerEventV2s();

            IEnumerable<ListenerEventV2> inputListenerEventV2s =
                randomListenerEventV2s;

            this.storageBrokerMock.Setup(broker =>
                broker.BulkDeleteListenerEventV2sAsync(
                    inputListenerEventV2s,
                    randomCancellationToken))
                        .Returns(ValueTask.CompletedTask);

            // when
            await this.listenerEventV2Service.BulkRemoveListenerEventV2sAsync(
                inputListenerEventV2s,
                randomCancellationToken);

            // then
            this.storageBrokerMock.Verify(broker =>
                broker.BulkDeleteListenerEventV2sAsync(
                    inputListenerEventV2s,
                    randomCancellationToken),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
