// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2.Exceptions;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.Events.V2
{
    public partial class EventV2ServiceTests
    {
        [Fact]
        public async Task ShouldBulkRestoreEventV2sAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<EventV2> randomEventV2s = CreateRandomRestoreEventV2s();
            List<EventV2> inputEventV2s = randomEventV2s;

            List<EventV2> expectedEventV2s =
                inputEventV2s.Select(item => item.DeepClone()).ToList();

            this.storageBrokerMock.Setup(broker =>
                broker.BulkInsertEventV2sAsync(
                    It.Is<List<EventV2>>(actual =>
                        SameEventV2sAs(expectedEventV2s, actual)),
                            randomCancellationToken))
                                .Returns(ValueTask.CompletedTask);

            // when
            IEnumerable<EventV2> actualEventV2s =
                await this.eventV2Service.BulkRestoreEventV2sAsync(
                    inputEventV2s,
                        randomCancellationToken);

            // then
            actualEventV2s.Should().BeEquivalentTo(expectedEventV2s);

            this.storageBrokerMock.Verify(broker =>
                broker.BulkInsertEventV2sAsync(
                    It.Is<List<EventV2>>(actual =>
                        SameEventV2sAs(expectedEventV2s, actual)),
                            randomCancellationToken),
                                Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldBulkRestoreValidEventV2sAndLogInvalidOnesAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            List<EventV2> validEventV2s = CreateRandomRestoreEventV2s();

            var invalidEventV2 = new EventV2
            {
                Id = Guid.Empty,
                Content = null,
                EventName = null,
                Type = GetInvalidEnum<EventTypeV2>(),
                Status = GetInvalidEnum<EventStatusV2>(),
                EventAddressId = Guid.Empty
            };

            List<EventV2> inputEventV2s =
                validEventV2s.Append(invalidEventV2).ToList();

            List<EventV2> expectedEventV2s =
                validEventV2s.Select(item => item.DeepClone()).ToList();

            var invalidEventV2Exception =
                new InvalidEventV2Exception(
                    message: "Event is invalid, fix the errors and try again.");

            invalidEventV2Exception.AddData(
                key: nameof(EventV2.Id),
                values: "Required");

            invalidEventV2Exception.AddData(
                key: nameof(EventV2.Content),
                values: "Required");

            invalidEventV2Exception.AddData(
                key: nameof(EventV2.EventName),
                values: "Required");

            invalidEventV2Exception.AddData(
                key: nameof(EventV2.EventAddressId),
                values: "Required");

            invalidEventV2Exception.AddData(
                key: nameof(EventV2.Type),
                values: "Value is not recognized");

            invalidEventV2Exception.AddData(
                key: nameof(EventV2.Status),
                values: "Value is not recognized");

            invalidEventV2Exception.AddData(
                key: nameof(EventV2.CreatedDate),
                values: "Required");

            invalidEventV2Exception.AddData(
                key: nameof(EventV2.UpdatedDate),
                values: "Required");

            this.storageBrokerMock.Setup(broker =>
                broker.BulkInsertEventV2sAsync(
                    It.Is<List<EventV2>>(actual =>
                        SameEventV2sAs(expectedEventV2s, actual)),
                            randomCancellationToken))
                                .Returns(ValueTask.CompletedTask);

            // when
            IEnumerable<EventV2> actualEventV2s =
                await this.eventV2Service.BulkRestoreEventV2sAsync(
                    inputEventV2s,
                        randomCancellationToken);

            // then
            actualEventV2s.Should().BeEquivalentTo(expectedEventV2s);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    invalidEventV2Exception))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.BulkInsertEventV2sAsync(
                    It.Is<List<EventV2>>(actual =>
                        SameEventV2sAs(expectedEventV2s, actual)),
                            randomCancellationToken),
                                Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
