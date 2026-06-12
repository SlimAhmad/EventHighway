// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventAddresses.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2;
using EventHighway.Core.Models.Services.Foundations.EventAddresses.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.EventAddresses.V2
{
    public partial class EventAddressesV2ClientTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task
            ShouldThrowDependencyValidationExceptionOnRegisterIfDependencyValidationErrorOccursAsync(
                Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventAddressV2 someEventAddressV2 = CreateRandomEventAddressV2();

            var expectedEventAddressV2ClientDependencyValidationException =
                new EventAddressV2ClientDependencyValidationException(
                    message: "Event address client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.AddEventAddressV2Async(
                    It.IsAny<EventAddressV2>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<EventAddressV2> registerEventAddressV2Task =
                this.eventAddressV2sClient.RegisterEventAddressV2Async(
                    someEventAddressV2,
                    randomCancellationToken);

            EventAddressV2ClientDependencyValidationException
                actualEventAddressV2ClientDependencyValidationException =
                    await Assert.ThrowsAsync<EventAddressV2ClientDependencyValidationException>(
                        registerEventAddressV2Task.AsTask);

            // then
            actualEventAddressV2ClientDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventAddressV2ClientDependencyValidationException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.AddEventAddressV2Async(
                    It.IsAny<EventAddressV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRegisterIfDependencyErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventAddressV2 someEventAddressV2 = CreateRandomEventAddressV2();
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var eventAddressV2DependencyException =
                new EventAddressV2DependencyException(
                    someMessage,
                    someInnerException);

            var expectedEventAddressV2ClientDependencyException =
                new EventAddressV2ClientDependencyException(
                    message: "Event address client dependency error occurred, contact support.",
                    innerException: eventAddressV2DependencyException.InnerException as Xeption);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.AddEventAddressV2Async(
                    It.IsAny<EventAddressV2>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(eventAddressV2DependencyException);

            // when
            ValueTask<EventAddressV2> registerEventAddressV2Task =
                this.eventAddressV2sClient.RegisterEventAddressV2Async(
                    someEventAddressV2,
                    randomCancellationToken);

            EventAddressV2ClientDependencyException
                actualEventAddressV2ClientDependencyException =
                    await Assert.ThrowsAsync<EventAddressV2ClientDependencyException>(
                        registerEventAddressV2Task.AsTask);

            // then
            actualEventAddressV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedEventAddressV2ClientDependencyException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.AddEventAddressV2Async(
                    It.IsAny<EventAddressV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRegisterIfServiceErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventAddressV2 someEventAddressV2 = CreateRandomEventAddressV2();
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var eventAddressV2ServiceException =
                new EventAddressV2ServiceException(
                    someMessage,
                    someInnerException);

            var expectedEventAddressV2ClientServiceException =
                new EventAddressV2ClientServiceException(
                    message: "Event address client service error occurred, contact support.",
                    innerException: eventAddressV2ServiceException.InnerException as Xeption);

            this.eventAddressV2ServiceMock.Setup(service =>
                service.AddEventAddressV2Async(
                    It.IsAny<EventAddressV2>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(eventAddressV2ServiceException);

            // when
            ValueTask<EventAddressV2> registerEventAddressV2Task =
                this.eventAddressV2sClient.RegisterEventAddressV2Async(
                    someEventAddressV2,
                    randomCancellationToken);

            EventAddressV2ClientServiceException actualEventAddressV2ClientServiceException =
                await Assert.ThrowsAsync<EventAddressV2ClientServiceException>(
                    registerEventAddressV2Task.AsTask);

            // then
            actualEventAddressV2ClientServiceException.Should()
                .BeEquivalentTo(expectedEventAddressV2ClientServiceException);

            this.eventAddressV2ServiceMock.Verify(service =>
                service.AddEventAddressV2Async(
                    It.IsAny<EventAddressV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventAddressV2ServiceMock.VerifyNoOtherCalls();
        }
    }
}
