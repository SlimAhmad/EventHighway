// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEvents.V2;
using EventHighway.Core.Models.Services.Processings.ListenerEvents.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEvents.V2
{
    public partial class ListenerEventV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnBulkRemoveIfListenerEventV2sIsNullAndLogItAsync()
        {
            // given
            List<ListenerEventV2> nullListenerEventV2s = null;

            var nullListenerEventV2ProcessingException =
                new NullListenerEventV2ProcessingException(message: "Listener event is null.");

            var expectedListenerEventV2ProcessingValidationException =
                new ListenerEventV2ProcessingValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: nullListenerEventV2ProcessingException);

            // when
            ValueTask bulkRemoveListenerEventV2sTask =
                this.listenerEventV2ProcessingService.BulkRemoveListenerEventV2sAsync(
                    nullListenerEventV2s,
                    TestContext.Current.CancellationToken);

            ListenerEventV2ProcessingValidationException
                actualListenerEventV2ProcessingValidationException =
                    await Assert.ThrowsAsync<ListenerEventV2ProcessingValidationException>(
                        bulkRemoveListenerEventV2sTask.AsTask);

            // then
            actualListenerEventV2ProcessingValidationException.Should().BeEquivalentTo(
                expectedListenerEventV2ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2ProcessingValidationException))),
                        Times.Once);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.BulkRemoveListenerEventV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventV2>>(),
                    It.IsAny<System.Threading.CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
