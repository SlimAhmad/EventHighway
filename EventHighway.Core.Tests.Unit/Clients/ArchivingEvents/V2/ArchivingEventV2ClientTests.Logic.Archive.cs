// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using Moq;

namespace EventHighway.Core.Tests.Unit.Clients.ArchivingEvents.V2
{
    public partial class ArchivingEventV2ClientTests
    {
        [Fact]
        public async Task ShouldArchiveDeadEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            this.archivingEventV2CoordinationServiceMock.Setup(service =>
                service.ArchiveEventV2sAsync(randomCancellationToken))
                    .Returns(ValueTask.CompletedTask);

            // when
            await this.archivingEventV2Client
                .ArchiveEventV2sAsync(randomCancellationToken);

            // then
            this.archivingEventV2CoordinationServiceMock.Verify(service =>
                service.ArchiveEventV2sAsync(randomCancellationToken),
                    Times.Once);

            this.archivingEventV2CoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
