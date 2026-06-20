// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Processings.EventArchives.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventArchives.V2
{
    public partial class EventArchiveV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnBulkRemoveIfEventArchiveV2sIsNullAndLogItAsync()
        {
            // given
            IEnumerable<EventArchiveV2> nullEventArchiveV2s = null;

            var nullEventArchiveV2ProcessingException =
                new NullEventArchiveV2ProcessingException(
                    message: "Event archive is null.");

            var expectedEventArchiveV2ProcessingValidationException =
                new EventArchiveV2ProcessingValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: nullEventArchiveV2ProcessingException);

            // when
            ValueTask bulkRemoveEventArchiveV2sTask =
                this.eventArchiveV2ProcessingService
                    .BulkRemoveEventArchiveV2sAsync(
                        nullEventArchiveV2s,
                        TestContext.Current.CancellationToken);

            EventArchiveV2ProcessingValidationException
                actualEventArchiveV2ProcessingValidationException =
                    await Assert.ThrowsAsync<EventArchiveV2ProcessingValidationException>(
                        bulkRemoveEventArchiveV2sTask.AsTask);

            // then
            actualEventArchiveV2ProcessingValidationException.Should()
                .BeEquivalentTo(expectedEventArchiveV2ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2ProcessingValidationException))),
                        Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
