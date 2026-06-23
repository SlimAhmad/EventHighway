// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.Events.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.Events.V2
{
    public partial class EventV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnBulkRestoreIfEventV2sIsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEnumerable<EventV2> nullEventV2s = null;

            var nullEventV2Exception =
                new NullEventV2Exception(message: "Event is null.");

            var expectedEventV2ValidationException =
                new EventV2ValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: nullEventV2Exception);

            // when
            ValueTask<IEnumerable<EventV2>> bulkRestoreEventV2sTask =
                this.eventV2Service.BulkRestoreEventV2sAsync(
                    nullEventV2s,
                        randomCancellationToken);

            EventV2ValidationException actualEventV2ValidationException =
                await Assert.ThrowsAsync<EventV2ValidationException>(
                    bulkRestoreEventV2sTask.AsTask);

            // then
            actualEventV2ValidationException.Should().BeEquivalentTo(
                expectedEventV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.BulkInsertEventV2sAsync(
                    It.IsAny<List<EventV2>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
