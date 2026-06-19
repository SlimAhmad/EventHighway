// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
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
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnBulkAddIfValidationErrorOccursAndLogItAsync(
            Xeption listenerEventArchiveV2ValidationException)
        {
            // given
            IEnumerable<ListenerEventArchiveV2> someListenerEventArchiveV2s =
                CreateRandomListenerEventArchiveV2s().ToList();

            var expectedListenerEventArchiveV2ProcessingDependencyValidationException =
                new ListenerEventArchiveV2ProcessingDependencyValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: listenerEventArchiveV2ValidationException.InnerException as Xeption);

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    someListenerEventArchiveV2s,
                    TestContext.Current.CancellationToken))
                        .ThrowsAsync(listenerEventArchiveV2ValidationException);

            // when
            ValueTask<IEnumerable<ListenerEventArchiveV2>> bulkAddListenerEventArchiveV2sTask =
                this.listenerEventArchiveV2ProcessingService
                    .BulkAddListenerEventArchiveV2sAsync(
                        someListenerEventArchiveV2s,
                        TestContext.Current.CancellationToken);

            ListenerEventArchiveV2ProcessingDependencyValidationException
                actualListenerEventArchiveV2ProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<ListenerEventArchiveV2ProcessingDependencyValidationException>(
                        bulkAddListenerEventArchiveV2sTask.AsTask);

            // then
            actualListenerEventArchiveV2ProcessingDependencyValidationException.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV2ProcessingDependencyValidationException);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.BulkAddListenerEventArchiveV2sAsync(
                    someListenerEventArchiveV2s,
                    TestContext.Current.CancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV2ProcessingDependencyValidationException))),
                        Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
