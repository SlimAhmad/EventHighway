// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Processings.ListenerEventArchives.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.ListenerEventArchives.V2
{
    public partial class ListenerEventArchiveV2ProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveAllIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedListenerEventArchiveV2ProcessingDependencyException =
                new ListenerEventArchiveV2ProcessingDependencyException(
                    message: "Listener event archive dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<IQueryable<ListenerEventArchiveV2>> retrieveAllTask =
                this.listenerEventArchiveV2ProcessingService
                    .RetrieveAllListenerEventArchiveV2sAsync(randomCancellationToken);

            ListenerEventArchiveV2ProcessingDependencyException actualException =
                await Assert.ThrowsAsync<ListenerEventArchiveV2ProcessingDependencyException>(
                    retrieveAllTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV2ProcessingDependencyException);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV2ProcessingDependencyException))),
                        Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
