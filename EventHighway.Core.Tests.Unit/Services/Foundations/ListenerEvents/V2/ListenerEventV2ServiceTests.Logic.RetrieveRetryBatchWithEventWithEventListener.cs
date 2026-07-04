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

        [Fact]
        public async Task ShouldRetrieveOnlyEligibleRetryListenerEventV2sExcludingIneligibleAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int take = 0;
            DateTimeOffset now = GetRandomDateTimeOffset();

            ListenerEventV2 eligibleWithNullGate = CreateRandomListenerEventV2();
            eligibleWithNullGate.Status = ListenerEventStatusV2.Error;
            eligibleWithNullGate.RemainingRetryAttempts = GetRandomNumber();
            eligibleWithNullGate.NextRetryAttemptNotBefore = null;
            eligibleWithNullGate.CreatedDate = now.AddMinutes(-4);

            ListenerEventV2 eligibleWithPastGate = CreateRandomListenerEventV2();
            eligibleWithPastGate.Status = ListenerEventStatusV2.Error;
            eligibleWithPastGate.RemainingRetryAttempts = GetRandomNumber();
            eligibleWithPastGate.NextRetryAttemptNotBefore = now.AddMinutes(-1);
            eligibleWithPastGate.CreatedDate = now.AddMinutes(-3);

            ListenerEventV2 ineligibleBecauseSuccess = CreateRandomListenerEventV2();
            ineligibleBecauseSuccess.Status = ListenerEventStatusV2.Success;
            ineligibleBecauseSuccess.RemainingRetryAttempts = GetRandomNumber();
            ineligibleBecauseSuccess.NextRetryAttemptNotBefore = null;

            ListenerEventV2 ineligibleBecauseNoRetries = CreateRandomListenerEventV2();
            ineligibleBecauseNoRetries.Status = ListenerEventStatusV2.Error;
            ineligibleBecauseNoRetries.RemainingRetryAttempts = 0;
            ineligibleBecauseNoRetries.NextRetryAttemptNotBefore = null;

            ListenerEventV2 ineligibleBecauseFutureGate = CreateRandomListenerEventV2();
            ineligibleBecauseFutureGate.Status = ListenerEventStatusV2.Error;
            ineligibleBecauseFutureGate.RemainingRetryAttempts = GetRandomNumber();
            ineligibleBecauseFutureGate.NextRetryAttemptNotBefore = now.AddMinutes(5);

            IQueryable<ListenerEventV2> randomListenerEventV2s = new List<ListenerEventV2>
            {
                ineligibleBecauseFutureGate,
                eligibleWithPastGate,
                ineligibleBecauseSuccess,
                eligibleWithNullGate,
                ineligibleBecauseNoRetries
            }.AsQueryable();

            IEnumerable<ListenerEventV2> expectedListenerEventV2s =
                new List<ListenerEventV2> { eligibleWithNullGate, eligibleWithPastGate }
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

        [Fact]
        public async Task ShouldRetrieveBatchOfEligibleRetryListenerEventV2sWithNavigationsOldestFirstAndTakeAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            int take = 2;
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
                    .Take(take)
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
