// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task ShouldBulkAddListenerEventArchiveV2sAsync()
        {
            // given
            DateTimeOffset randomDateTime = GetRandomDateTimeOffset();
            IQueryable<ListenerEventArchiveV2> randomListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s();

            List<ListenerEventArchiveV2> inputListenerEventArchiveV2s =
                randomListenerEventArchiveV2s.ToList();

            List<ListenerEventArchiveV2> expectedListenerEventArchiveV2s =
                inputListenerEventArchiveV2s.Select(item => item.DeepClone()).ToList();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            foreach (ListenerEventArchiveV2 item in expectedListenerEventArchiveV2s)
            {
                item.ArchivedDate = randomDateTime;
            }

            this.storageBrokerMock.Setup(broker =>
                broker.InsertBulkListenerEventArchiveV2sAsync(
                    It.Is<List<ListenerEventArchiveV2>>(actual =>
                        SameListenerEventArchiveV2sAs(expectedListenerEventArchiveV2s, actual)),
                            It.IsAny<CancellationToken>()))
                                .Returns(ValueTask.CompletedTask);

            // when
            IEnumerable<ListenerEventArchiveV2> actualListenerEventArchiveV2s =
                await this.listenerEventArchiveV2Service.BulkAddListenerEventArchiveV2sAsync(
                    inputListenerEventArchiveV2s,
                        TestContext.Current.CancellationToken);

            // then
            actualListenerEventArchiveV2s.Should().BeEquivalentTo(expectedListenerEventArchiveV2s);

            foreach (ListenerEventArchiveV2 item in inputListenerEventArchiveV2s)
            {
                Assert.Equal(randomDateTime, item.ArchivedDate);
            }

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Exactly(inputListenerEventArchiveV2s.Count + 1));

            this.storageBrokerMock.Verify(broker =>
                broker.InsertBulkListenerEventArchiveV2sAsync(
                    It.Is<List<ListenerEventArchiveV2>>(actual =>
                        SameListenerEventArchiveV2sAs(expectedListenerEventArchiveV2s, actual)),
                            It.IsAny<CancellationToken>()),
                                Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
