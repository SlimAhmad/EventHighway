// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.Events.V2
{
    public partial class EventV2ServiceTests
    {
        [Fact]
        public async Task ShouldModifyEventV2Async()
        {
            // given
            DateTimeOffset randomDateTime =
                GetRandomDateTimeOffset();

            int randomDaysAgo = GetRandomNegativeNumber();

            EventV2 randomEventV2 =
                CreateRandomEventV2(randomDateTime);

            EventV2 inputEventV2 =
                randomEventV2;

            inputEventV2.CreatedDate =
                randomDateTime.AddDays(randomDaysAgo);

            EventV2 storageEventV2 =
                inputEventV2.DeepClone();

            int randomSecondsAgo =
                GetRandomNegativeNumber();

            DateTimeOffset storageUpdatedDate =
                randomDateTime.AddSeconds(
                    randomSecondsAgo);

            storageEventV2.UpdatedDate =
                storageUpdatedDate;

            EventV2 persistedEventV2 =
                inputEventV2;

            EventV2 expectedEventV2 =
                persistedEventV2.DeepClone();

            Guid eventV2Id = inputEventV2.Id;

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTime);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectEventV2ByIdAsync(
                    eventV2Id))
                        .ReturnsAsync(
                            storageEventV2);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateEventV2Async(
                    inputEventV2))
                        .ReturnsAsync(persistedEventV2);

            // when
            EventV2 actualEventV2 =
                await this.eventV2Service
                    .ModifyEventV2Async(
                        inputEventV2);

            // then
            actualEventV2.Should().BeEquivalentTo(
                expectedEventV2);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectEventV2ByIdAsync(
                    eventV2Id),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateEventV2Async(
                    inputEventV2),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
