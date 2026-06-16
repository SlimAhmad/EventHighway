// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventAddresses.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.EventAddresses.V2
{
    public partial class EventAddressV2ClientTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowValidationExceptionOnRetrieveOrRegisterIfValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventAddressV2 someEventAddressV2 = CreateRandomEventAddressV2();

            var expectedEventAddressV2ClientValidationException =
                new EventAddressV2ClientValidationException(
                    message: "Event address client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption,
                    data: (validationException.InnerException as Xeption).Data);

            this.eventAddressV2ProcessingServiceMock.Setup(service =>
                service.RetrieveOrRegisterEventAddressV2Async(
                    It.IsAny<EventAddressV2>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<EventAddressV2> retrieveOrRegisterEventAddressV2Task =
                this.eventAddressV2Client.RetrieveOrRegisterEventAddressV2Async(
                    someEventAddressV2,
                    randomCancellationToken);

            EventAddressV2ClientValidationException actualEventAddressV2ClientValidationException =
                await Assert.ThrowsAsync<EventAddressV2ClientValidationException>(
                    retrieveOrRegisterEventAddressV2Task.AsTask);

            // then
            actualEventAddressV2ClientValidationException.Should()
                .BeEquivalentTo(expectedEventAddressV2ClientValidationException);

            this.eventAddressV2ProcessingServiceMock.Verify(service =>
                service.RetrieveOrRegisterEventAddressV2Async(
                    It.IsAny<EventAddressV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventAddressV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
