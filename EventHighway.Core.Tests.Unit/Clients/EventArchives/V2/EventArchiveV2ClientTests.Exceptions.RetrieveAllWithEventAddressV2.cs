// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventArchives.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.EventArchives.V2
{
    public partial class EventArchiveV2ClientTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowValidationExceptionOnRetrieveAllWithEventAddressV2IfValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedEventArchiveV2ClientValidationException =
                new EventArchiveV2ClientValidationException(
                    message: "Event archive client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption,
                    data: (validationException.InnerException as Xeption).Data);

            this.eventArchiveV2ServiceMock.Setup(service =>
                service.RetrieveAllEventArchiveV2sWithEventAddressV2Async(
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<IQueryable<EventArchiveV2>> retrieveAllTask =
                this.eventArchiveV2Client
                    .RetrieveAllEventArchiveV2sWithEventAddressV2Async(
                        randomCancellationToken);

            EventArchiveV2ClientValidationException actualEventArchiveV2ClientValidationException =
                await Assert.ThrowsAsync<EventArchiveV2ClientValidationException>(
                    retrieveAllTask.AsTask);

            // then
            actualEventArchiveV2ClientValidationException.Should()
                .BeEquivalentTo(expectedEventArchiveV2ClientValidationException);

            this.eventArchiveV2ServiceMock.Verify(service =>
                service.RetrieveAllEventArchiveV2sWithEventAddressV2Async(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventArchiveV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
