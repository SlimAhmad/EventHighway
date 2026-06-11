// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEvents.V2
{
    public partial class ListenerEventV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldRemoveListenerEventV2ByIdAsync()
        {
            // given
            CancellationToken cancellationToken =
                TestContext.Current.CancellationToken;

            Guid randomListenerEventId = GetRandomId();
            Guid inputListenerEventId = randomListenerEventId;

            ListenerEventV2 randomListenerEventV2 =
                CreateRandomListenerEventV2();

            ListenerEventV2 removedListenerEventV2 =
                randomListenerEventV2;

            ListenerEventV2 expectedListenerEventV2 =
                removedListenerEventV2.DeepClone();

            this.listenerEventV2ServiceMock.Setup(service =>
                service.RemoveListenerEventV2ByIdAsync(
                    inputListenerEventId,
                    cancellationToken))
                        .ReturnsAsync(removedListenerEventV2);

            // when
            ListenerEventV2 actualListenerEventV2 =
                await this.listenerEventV2ProcessingService
                    .RemoveListenerEventV2ByIdAsync(
                        inputListenerEventId,
                        cancellationToken);

            // then
            actualListenerEventV2.Should()
                .BeEquivalentTo(expectedListenerEventV2);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.RemoveListenerEventV2ByIdAsync(
                    inputListenerEventId,
                    cancellationToken),
                        Times.Once);

            this.listenerEventV2ServiceMock
                .VerifyNoOtherCalls();

            this.loggingBrokerMock
                .VerifyNoOtherCalls();
        }
    }
}
