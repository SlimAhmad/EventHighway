// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventArchives.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.EventArchives.V2
{
    public partial class EventArchiveV2OrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(EventArchiveV2ValidationExceptions))]
        [MemberData(nameof(ListenerEventArchiveV2ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnBulkAddIfValidationExceptionOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            IQueryable<EventArchiveV2> someEventArchiveV2s = CreateRandomEventArchiveV2s();
            IEnumerable<EventArchiveV2> inputEventArchiveV2s = someEventArchiveV2s;

            var expectedEventArchiveV2OrchestrationDependencyValidationException =
                new EventArchiveV2OrchestrationDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.BulkAddEventArchiveV2sAsync(
                    inputEventArchiveV2s,
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask bulkAddEventArchiveV2sTask =
                this.eventArchiveV2OrchestrationService.BulkAddEventArchiveV2sAsync(
                    inputEventArchiveV2s,
                    TestContext.Current.CancellationToken);

            EventArchiveV2OrchestrationDependencyValidationException
                actualEventArchiveV2OrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<EventArchiveV2OrchestrationDependencyValidationException>(
                        bulkAddEventArchiveV2sTask.AsTask);

            // then
            actualEventArchiveV2OrchestrationDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventArchiveV2OrchestrationDependencyValidationException);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.BulkAddEventArchiveV2sAsync(
                    inputEventArchiveV2s,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventArchiveV2OrchestrationDependencyValidationException))),
                        Times.Once);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    It.IsAny<IEnumerable<ListenerEventArchiveV2>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
