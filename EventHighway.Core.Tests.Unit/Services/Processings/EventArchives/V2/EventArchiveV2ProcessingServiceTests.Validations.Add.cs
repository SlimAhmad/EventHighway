// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
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
        public async Task ShouldThrowValidationExceptionOnAddIfEventArchiveV2IsNullAndLogItAsync()
        {
            // given
            EventArchiveV2 nullEventArchiveV2 = null;

            var nullEventArchiveV2ProcessingException =
                new NullEventArchiveV2ProcessingException(
                    message: "Event archive is null.");

            var expectedEventArchiveV2ProcessingValidationException =
                new EventArchiveV2ProcessingValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: nullEventArchiveV2ProcessingException);

            // when
            ValueTask<EventArchiveV2> addEventArchiveV2Task =
                this.eventArchiveV2ProcessingService.AddEventArchiveV2Async(
                    nullEventArchiveV2,
                    TestContext.Current.CancellationToken);

            EventArchiveV2ProcessingValidationException
                actualEventArchiveV2ProcessingValidationException =
                    await Assert.ThrowsAsync<EventArchiveV2ProcessingValidationException>(
                        addEventArchiveV2Task.AsTask);

            // then
            actualEventArchiveV2ProcessingValidationException.Should().BeEquivalentTo(
                expectedEventArchiveV2ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2ProcessingValidationException))),
                        Times.Once);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.AddEventArchiveV2Async(
                    It.IsAny<EventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
