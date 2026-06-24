// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
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
        public async Task ShouldThrowValidationExceptionOnBulkRestoreIfListenerEventV2sIsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEnumerable<ListenerEventV2> nullListenerEventV2s = null;

            var nullListenerEventV2ProcessingException =
                new NullListenerEventV2ProcessingException(
                    message: "Listener event is null.");

            var expectedListenerEventV2ProcessingValidationException =
                new ListenerEventV2ProcessingValidationException(
                    message: "Listener event validation error occurred, fix the errors and try again.",
                    innerException: nullListenerEventV2ProcessingException);

            // when
            ValueTask<IEnumerable<ListenerEventV2>> bulkRestoreListenerEventV2sTask =
                this.listenerEventV2ProcessingService.BulkRestoreListenerEventV2sAsync(
                    nullListenerEventV2s,
                    randomCancellationToken);

            ListenerEventV2ProcessingValidationException actualException =
                await Assert.ThrowsAsync<ListenerEventV2ProcessingValidationException>(
                    bulkRestoreListenerEventV2sTask.AsTask);

            // then
            actualException.Should().BeEquivalentTo(
                expectedListenerEventV2ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventV2ProcessingValidationException))),
                        Times.Once);

            this.listenerEventV2ServiceMock.Verify(service =>
                service.BulkRestoreListenerEventV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventV2>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.listenerEventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
