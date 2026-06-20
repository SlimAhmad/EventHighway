// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEventArchives.V2
{
    public partial class ListenerEventArchiveV2ServiceTests
    {
        [Fact]
        public async Task ShouldBulkRemoveListenerEventArchiveV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<ListenerEventArchiveV2> randomListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s();

            IEnumerable<ListenerEventArchiveV2> inputListenerEventArchiveV2s =
                randomListenerEventArchiveV2s;

            this.storageBrokerMock.Setup(broker =>
                broker.BulkDeleteListenerEventArchiveV2sAsync(
                    inputListenerEventArchiveV2s,
                        randomCancellationToken))
                            .Returns(ValueTask.CompletedTask);

            // when
            await this.listenerEventArchiveV2Service.BulkRemoveListenerEventArchiveV2sAsync(
                inputListenerEventArchiveV2s,
                    randomCancellationToken);

            // then
            this.storageBrokerMock.Verify(broker =>
                broker.BulkDeleteListenerEventArchiveV2sAsync(
                    inputListenerEventArchiveV2s,
                        randomCancellationToken),
                            Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
