// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.ListenerEventArchives.V2
{
    public partial class ListenerEventArchiveV2ServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnBulkAddIfListenerEventArchiveV2sIsNullAndLogItAsync()
        {
            // given
            List<ListenerEventArchiveV2> nullListenerEventArchiveV2s = null;

            var nullListenerEventArchiveV2Exception =
                new NullListenerEventArchiveV2Exception(
                    message: "Listener event archive is null.");

            var expectedListenerEventArchiveV2ValidationException =
                new ListenerEventArchiveV2ValidationException(
                    message: "Listener event archive validation error occurred, fix the errors and try again.",
                    innerException: nullListenerEventArchiveV2Exception);

            // when
            ValueTask<IEnumerable<ListenerEventArchiveV2>> bulkAddListenerEventArchiveV2sTask =
                this.listenerEventArchiveV2Service.BulkAddListenerEventArchiveV2sAsync(
                    nullListenerEventArchiveV2s,
                    TestContext.Current.CancellationToken);

            ListenerEventArchiveV2ValidationException actualListenerEventArchiveV2ValidationException =
                await Assert.ThrowsAsync<ListenerEventArchiveV2ValidationException>(
                    bulkAddListenerEventArchiveV2sTask.AsTask);

            // then
            actualListenerEventArchiveV2ValidationException.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV2ValidationException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedListenerEventArchiveV2ValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
