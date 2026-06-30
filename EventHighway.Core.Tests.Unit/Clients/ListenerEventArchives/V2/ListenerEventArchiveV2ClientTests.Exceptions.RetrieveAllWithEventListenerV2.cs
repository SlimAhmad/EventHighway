// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.ListenerEventArchives.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2;
using EventHighway.Core.Models.Services.Foundations.ListenerEventArchives.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.ListenerEventArchives.V2
{
    public partial class ListenerEventArchiveV2ClientTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowValidationExceptionOnRetrieveAllWithEventListenerV2IfValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedListenerEventArchiveV2ClientValidationException =
                new ListenerEventArchiveV2ClientValidationException(
                    message: "Listener event archive client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption,
                    data: (validationException.InnerException as Xeption).Data);

            this.listenerEventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllListenerEventArchiveV2sWithEventListenerV2Async(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<IQueryable<ListenerEventArchiveV2>> retrieveAllTask =
                this.listenerEventArchiveV2Client
                    .RetrieveAllListenerEventArchiveV2sWithEventListenerV2Async(
                        randomCancellationToken);

            ListenerEventArchiveV2ClientValidationException actualListenerEventArchiveV2ClientValidationException =
                await Assert.ThrowsAsync<ListenerEventArchiveV2ClientValidationException>(
                    retrieveAllTask.AsTask);

            // then
            actualListenerEventArchiveV2ClientValidationException.Should()
                .BeEquivalentTo(expectedListenerEventArchiveV2ClientValidationException);

            this.listenerEventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllListenerEventArchiveV2sWithEventListenerV2Async(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.listenerEventArchiveV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
