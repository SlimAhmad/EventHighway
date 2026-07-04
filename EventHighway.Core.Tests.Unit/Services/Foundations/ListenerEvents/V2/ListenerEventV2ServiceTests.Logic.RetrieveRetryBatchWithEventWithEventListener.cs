// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task ShouldRetrieveAllEligibleRetryListenerEventV2sWithNavigationsOldestFirstWhenTakeIsZeroAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int take = 0;
            DateTimeOffset now = GetRandomDateTimeOffset();

            List<ListenerEventV2> randomListenerEventV2List =
                CreateRandomListenerEventV2s().ToList();

            for (int index = 0; index < randomListenerEventV2List.Count; index++)
            {
                ListenerEventV2 listenerEventV2 = randomListenerEventV2List[index];
                listenerEventV2.Status = ListenerEventStatusV2.Error;
                listenerEventV2.RemainingRetryAttempts = GetRandomNumber();
                listenerEventV2.NextRetryAttemptNotBefore = null;
                listenerEventV2.CreatedDate = now.AddMinutes(-index);
            }

            IQueryable<ListenerEventV2> randomListenerEventV2s =
                randomListenerEventV2List.AsQueryable();

            IEnumerable<ListenerEventV2> expectedListenerEventV2s =
                randomListenerEventV2s
                    .OrderBy(listenerEventV2 => listenerEventV2.CreatedDate)
                    .DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllListenerEventV2sWithEventV2WithEventListenerV2Async(
                    randomCancellationToken))
                        .ReturnsAsync(randomListenerEventV2s);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(now);

            // when
            IEnumerable<ListenerEventV2> actualListenerEventV2s =
                await this.listenerEventV2Service
                    .RetrieveRetryBatchListenerEventV2sWithEventWithEventListenerAsync(
                        take,
                        randomCancellationToken);

            // then
            actualListenerEventV2s.Should().BeEquivalentTo(
                expectedListenerEventV2s,
                options => options.WithStrictOrdering());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllListenerEventV2sWithEventV2WithEventListenerV2Async(
                    randomCancellationToken),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
