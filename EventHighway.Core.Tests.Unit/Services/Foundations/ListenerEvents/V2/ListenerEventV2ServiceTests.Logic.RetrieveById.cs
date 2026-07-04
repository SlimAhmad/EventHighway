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

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEvents.V2
{
    public partial class ListenerEventV2ServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveListenerEventV2ByIdAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            ListenerEventV2 randomListenerEventV2 = CreateRandomListenerEventV2();
            ListenerEventV2 storageListenerEventV2 = randomListenerEventV2;
            ListenerEventV2 expectedListenerEventV2 = storageListenerEventV2.DeepClone();
            Guid inputListenerEventV2Id = storageListenerEventV2.Id;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectListenerEventV2ByIdAsync(
                    inputListenerEventV2Id,
                    randomCancellationToken))
                        .ReturnsAsync(storageListenerEventV2);

            // when
            ListenerEventV2 actualListenerEventV2 =
                await this.listenerEventV2Service.RetrieveListenerEventV2ByIdAsync(
                    inputListenerEventV2Id,
                    randomCancellationToken);

            // then
            actualListenerEventV2.Should().BeEquivalentTo(expectedListenerEventV2);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectListenerEventV2ByIdAsync(
                    inputListenerEventV2Id,
                    randomCancellationToken),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
