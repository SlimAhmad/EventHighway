// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEventArchives.V2
{
    public partial class ListenerEventArchiveV2ServiceTests
    {
        [Fact]
        public async Task ShouldAddListenerEventArchiveV2Async()
        {
            // given
            CancellationToken cancellationToken =
                TestContext.Current.CancellationToken;

            DateTimeOffset randomDateTimeOffset =
                GetRandomDateTimeOffset();

            ListenerEventArchiveV2 randomListenerEventArchiveV2 =
                CreateRandomListenerEventArchiveV2(date: randomDateTimeOffset);

            ListenerEventArchiveV2 inputListenerEventArchiveV2 =
                randomListenerEventArchiveV2;

            ListenerEventArchiveV2 insertedListenerEventArchiveV2 =
                inputListenerEventArchiveV2;

            ListenerEventArchiveV2 expectedListenerEventArchiveV2 =
                insertedListenerEventArchiveV2.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.InsertListenerEventArchiveV2Async(
                    inputListenerEventArchiveV2,
                    cancellationToken))
                        .ReturnsAsync(insertedListenerEventArchiveV2);

            // when
            ListenerEventArchiveV2 actualListenerEventArchiveV2 =
                await this.listenerEventArchiveV2Service
                    .AddListenerEventArchiveV2Async(
                        inputListenerEventArchiveV2,
                        cancellationToken);

            // then
            actualListenerEventArchiveV2.Should().BeEquivalentTo(
                expectedListenerEventArchiveV2);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertListenerEventArchiveV2Async(
                    inputListenerEventArchiveV2,
                    cancellationToken),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
