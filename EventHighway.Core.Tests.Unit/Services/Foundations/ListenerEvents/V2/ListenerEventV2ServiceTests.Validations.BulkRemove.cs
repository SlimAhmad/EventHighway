// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEvents.V2
{
    public partial class ListenerEventV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnBulkRemoveIfListenerEventV2sIsNullAndLogItAsync()
        {
            // given
            List<ListenerEventV2> nullListenerEventV2s = null;

            var nullListenerEventV2Exception =
                new NullListenerEventV2Exception(
                    message: "Listener event is null.");

            var expectedListenerEventV2ValidationException =
                new ListenerEventV2ValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: nullListenerEventV2Exception);

            // when
            ValueTask bulkRemoveListenerEventV2sTask =
                this.listenerEventV2Service.BulkRemoveListenerEventV2sAsync(
                    nullListenerEventV2s,
                    TestContext.Current.CancellationToken);

            ListenerEventV2ValidationException actualListenerEventV2ValidationException =
                await Assert.ThrowsAsync<ListenerEventV2ValidationException>(
                    bulkRemoveListenerEventV2sTask.AsTask);

            // then
            actualListenerEventV2ValidationException.Should()
                .BeEquivalentTo(expectedListenerEventV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
