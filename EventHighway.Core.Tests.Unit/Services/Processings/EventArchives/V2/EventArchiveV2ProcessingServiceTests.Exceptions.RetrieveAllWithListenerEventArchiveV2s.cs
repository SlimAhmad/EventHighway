// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
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
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAllWithListenerEventArchiveV2sIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedEventArchiveV2ProcessingDependencyException =
                new EventArchiveV2ProcessingDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sWithListenerEventArchiveV2sAsync(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<IQueryable<EventArchiveV2>> retrieveAllTask =
                this.eventArchiveV2ProcessingService
                    .RetrieveAllEventArchiveV2sWithListenerEventArchiveV2sAsync(randomCancellationToken);

            EventArchiveV2ProcessingDependencyException actualException =
                await Assert.ThrowsAsync<EventArchiveV2ProcessingDependencyException>(
                    retrieveAllTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedEventArchiveV2ProcessingDependencyException);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sWithListenerEventArchiveV2sAsync(
                    It.IsAny<CancellationToken>()),
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
