// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.Events.V2
{
    public partial class EventV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldBulkRemoveEventV2sAsync()
        {
            // given
            IQueryable<EventV2> randomEventV2s = CreateRandomEventV2s();
            IEnumerable<EventV2> inputEventV2s = randomEventV2s;

            this.eventV2ServiceMock.Setup(service =>
                service.BulkRemoveEventV2sAsync(
                    inputEventV2s,
                    It.IsAny<CancellationToken>()))
                        .Returns(ValueTask.CompletedTask);

            // when
            await this.eventV2ProcessingService.BulkRemoveEventV2sAsync(
                inputEventV2s,
                TestContext.Current.CancellationToken);

            // then
            this.eventV2ServiceMock.Verify(service =>
                service.BulkRemoveEventV2sAsync(
                    inputEventV2s,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
