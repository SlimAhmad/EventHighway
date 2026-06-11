// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.Events.V2
{
    public partial class EventV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRemoveEventV2ByIdAsync()
        {
            // given
            CancellationToken cancellationToken =
                TestContext.Current.CancellationToken;

            Guid randomEventV2Id = GetRandomId();
            Guid inputEventV2Id = randomEventV2Id;
            EventV2 randomEventV2 = CreateRandomEventV2();
            EventV2 removedEventV2 = randomEventV2;

            EventV2 expectedEventV2 =
                removedEventV2.DeepClone();

            this.eventV2ServiceMock.Setup(service =>
                service.RemoveEventV2ByIdAsync(
                    inputEventV2Id,
                    cancellationToken))
                        .ReturnsAsync(removedEventV2);

            // when
            EventV2 actualEventV2 =
                await this.eventV2ProcessingService
                    .RemoveEventV2ByIdAsync(
                        inputEventV2Id,
                        cancellationToken);

            // then
            actualEventV2.Should()
                .BeEquivalentTo(expectedEventV2);

            this.eventV2ServiceMock.Verify(service =>
                service.RemoveEventV2ByIdAsync(
                    inputEventV2Id,
                    cancellationToken),
                        Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
