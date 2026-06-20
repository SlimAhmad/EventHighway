// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Processings.ListenerEventArchives.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEventArchives.V2
{
    public partial class ListenerEventArchiveV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnBulkAddIfListenerEventArchiveV2sIsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IEnumerable<ListenerEventArchiveV2> nullListenerEventArchiveV2s = null;

            var nullListenerEventArchiveV2ProcessingException =
                new NullListenerEventArchiveV2ProcessingException(
                    message: "Listener event archive is null.");

            var expectedListenerEventArchiveV2ProcessingValidationException =
                new ListenerEventArchiveV2ProcessingValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: nullListenerEventArchiveV2ProcessingException);

            // when
            ValueTask<IEnumerable<ListenerEventArchiveV2>> bulkAddListenerEventArchiveV2sTask =
                this.listenerEventArchiveV2ProcessingService
                    .BulkAddListenerEventArchiveV2sAsync(
                        nullListenerEventArchiveV2s,
                        randomCancellationToken);

            ListenerEventArchiveV2ProcessingValidationException
                actualListenerEventArchiveV2ProcessingValidationException =
                    await Assert.ThrowsAsync<ListenerEventArchiveV2ProcessingValidationException>(
                        bulkAddListenerEventArchiveV2sTask.AsTask);

            // then
            actualListenerEventArchiveV2ProcessingValidationException.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV2ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV2ProcessingValidationException))),
                        Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
