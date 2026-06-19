// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Processings.EventArchives.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventArchives.V2
{
    public partial class EventArchiveV2ProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnBulkAddIfValidationErrorOccursAndLogItAsync(
            Xeption eventArchiveV2ValidationException)
        {
            // given
            IEnumerable<EventArchiveV2> someEventArchiveV2s =
                CreateRandomEventArchiveV2s().ToList();

            var expectedEventArchiveV2ProcessingDependencyValidationException =
                new EventArchiveV2ProcessingDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: eventArchiveV2ValidationException.InnerException as Xeption);

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.BulkAddEventArchiveV2sAsync(
                    someEventArchiveV2s,
                    TestContext.Current.CancellationToken))
                        .ThrowsAsync(eventArchiveV2ValidationException);

            // when
            ValueTask<IEnumerable<EventArchiveV2>> bulkAddEventArchiveV2sTask =
                this.eventArchiveV2ProcessingService
                    .BulkAddEventArchiveV2sAsync(
                        someEventArchiveV2s,
                        TestContext.Current.CancellationToken);

            EventArchiveV2ProcessingDependencyValidationException
                actualEventArchiveV2ProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<EventArchiveV2ProcessingDependencyValidationException>(
                        bulkAddEventArchiveV2sTask.AsTask);

            // then
            actualEventArchiveV2ProcessingDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventArchiveV2ProcessingDependencyValidationException);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.BulkAddEventArchiveV2sAsync(
                    someEventArchiveV2s,
                    TestContext.Current.CancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2ProcessingDependencyValidationException))),
                        Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnBulkAddIfDependencyErrorOccursAndLogItAsync(
            Xeption eventArchiveV2DependencyException)
        {
            // given
            IEnumerable<EventArchiveV2> someEventArchiveV2s =
                CreateRandomEventArchiveV2s().ToList();

            var expectedEventArchiveV2ProcessingDependencyException =
                new EventArchiveV2ProcessingDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: eventArchiveV2DependencyException.InnerException as Xeption);

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.BulkAddEventArchiveV2sAsync(
                    someEventArchiveV2s,
                    TestContext.Current.CancellationToken))
                        .ThrowsAsync(eventArchiveV2DependencyException);

            // when
            ValueTask<IEnumerable<EventArchiveV2>> bulkAddEventArchiveV2sTask =
                this.eventArchiveV2ProcessingService
                    .BulkAddEventArchiveV2sAsync(
                        someEventArchiveV2s,
                        TestContext.Current.CancellationToken);

            EventArchiveV2ProcessingDependencyException
                actualEventArchiveV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<EventArchiveV2ProcessingDependencyException>(
                        bulkAddEventArchiveV2sTask.AsTask);

            // then
            actualEventArchiveV2ProcessingDependencyException.Should()
                .BeEquivalentTo(expectedEventArchiveV2ProcessingDependencyException);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.BulkAddEventArchiveV2sAsync(
                    someEventArchiveV2s,
                    TestContext.Current.CancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2ProcessingDependencyException))),
                        Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
