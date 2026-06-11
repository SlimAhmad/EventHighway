// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEventArchives.V2
{
    public partial class ListenerEventArchiveV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldAddListenerEventArchiveV2Async()
        {
            // given
            CancellationToken cancellationToken =
                TestContext.Current.CancellationToken;

            ListenerEventArchiveV2 randomListenerEventArchiveV2 =
                CreateRandomListenerEventArchiveV2();

            ListenerEventArchiveV2 inputListenerEventArchiveV2 =
                randomListenerEventArchiveV2;

            ListenerEventArchiveV2 storageListenerEventArchiveV2 =
                inputListenerEventArchiveV2;

            ListenerEventArchiveV2 expectedListenerEventArchiveV2 =
                storageListenerEventArchiveV2.DeepClone();

            this.listenerEventArchiveV2ServiceMock.Setup(broker =>
                broker.AddListenerEventArchiveV2Async(
                    inputListenerEventArchiveV2,
                    cancellationToken))
                        .ReturnsAsync(storageListenerEventArchiveV2);

            // when
            ListenerEventArchiveV2 actualListenerEventArchiveV2 =
                await this.listenerEventArchiveV2ProcessingService
                    .AddListenerEventArchiveV2Async(
                        inputListenerEventArchiveV2,
                        cancellationToken);

            // then
            actualListenerEventArchiveV2.Should().BeEquivalentTo(
                expectedListenerEventArchiveV2);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.AddListenerEventArchiveV2Async(
                    inputListenerEventArchiveV2,
                    cancellationToken),
                        Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
