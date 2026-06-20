// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

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
        public async Task ShouldThrowValidationExceptionOnAddIfListenerEventArchiveV2IsNullAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            ListenerEventArchiveV2 nullListenerEventArchiveV2 = null;

            var nullListenerEventArchiveV2ProcessingException =
                new NullListenerEventArchiveV2ProcessingException(
                    message: "Listener event archive is null.");

            var expectedListenerEventArchiveV2ProcessingValidationException =
                new ListenerEventArchiveV2ProcessingValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: nullListenerEventArchiveV2ProcessingException);

            // when
            ValueTask<ListenerEventArchiveV2> addListenerEventArchiveV2Task =
                this.listenerEventArchiveV2ProcessingService.AddListenerEventArchiveV2Async(
                    nullListenerEventArchiveV2,
                    randomCancellationToken);

            ListenerEventArchiveV2ProcessingValidationException
                actualListenerEventArchiveV2ProcessingValidationException =
                    await Assert.ThrowsAsync<ListenerEventArchiveV2ProcessingValidationException>(
                        addListenerEventArchiveV2Task.AsTask);

            // then
            actualListenerEventArchiveV2ProcessingValidationException.Should().BeEquivalentTo(
                expectedListenerEventArchiveV2ProcessingValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV2ProcessingValidationException))),
                        Times.Once);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.AddListenerEventArchiveV2Async(
                    It.IsAny<ListenerEventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
