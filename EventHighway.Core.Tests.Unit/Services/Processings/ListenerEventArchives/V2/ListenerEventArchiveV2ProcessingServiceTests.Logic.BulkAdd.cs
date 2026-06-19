// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEventArchives.V2
{
    public partial class ListenerEventArchiveV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldBulkAddListenerEventArchiveV2sAsync()
        {
            // given
            IEnumerable<ListenerEventArchiveV2> randomListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s().ToList();

            IEnumerable<ListenerEventArchiveV2> inputListenerEventArchiveV2s =
                randomListenerEventArchiveV2s;

            IEnumerable<ListenerEventArchiveV2> returnedListenerEventArchiveV2s =
                randomListenerEventArchiveV2s;

            IEnumerable<ListenerEventArchiveV2> expectedListenerEventArchiveV2s =
                returnedListenerEventArchiveV2s;

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    inputListenerEventArchiveV2s,
                    TestContext.Current.CancellationToken))
                        .ReturnsAsync(returnedListenerEventArchiveV2s);

            // when
            IEnumerable<ListenerEventArchiveV2> actualListenerEventArchiveV2s =
                await this.listenerEventArchiveV2ProcessingService
                    .BulkAddListenerEventArchiveV2sAsync(
                        inputListenerEventArchiveV2s,
                        TestContext.Current.CancellationToken);

            // then
            actualListenerEventArchiveV2s.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV2s);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    inputListenerEventArchiveV2s,
                    TestContext.Current.CancellationToken),
                        Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
