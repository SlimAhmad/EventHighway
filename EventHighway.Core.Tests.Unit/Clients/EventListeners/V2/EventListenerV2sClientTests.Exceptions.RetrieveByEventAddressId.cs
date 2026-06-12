// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.EventListeners.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.EventListeners.V2;
using EventHighway.Core.Models.Services.Orchestrations.EventListeners.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.EventListeners.V2
{
    public partial class EventListenerV2sClientTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task
            ShouldThrowDependencyValidationExceptionOnRetrieveByEventAddressIdIfValidationErrorOccursAsync(
                Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventAddressId = GetRandomId();

            var expectedEventListenerV2ClientDependencyValidationException =
                new EventListenerV2ClientDependencyValidationException(
                    message: "Event listener client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<IQueryable<EventListenerV2>> retrieveEventListenerV2sTask =
                this.eventListenerV2sClient.RetrieveEventListenerV2sByEventAddressIdAsync(
                    someEventAddressId,
                    randomCancellationToken);

            EventListenerV2ClientDependencyValidationException
                actualEventListenerV2ClientDependencyValidationException =
                    await Assert.ThrowsAsync<EventListenerV2ClientDependencyValidationException>(
                        retrieveEventListenerV2sTask.AsTask);

            // then
            actualEventListenerV2ClientDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventListenerV2ClientDependencyValidationException);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowDependencyExceptionOnRetrieveByEventAddressIdIfDependencyErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventAddressId = GetRandomId();
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var eventListenerV2OrchestrationDependencyException =
                new EventListenerV2OrchestrationDependencyException(
                    someMessage,
                    someInnerException);

            var expectedEventListenerV2ClientDependencyException =
                new EventListenerV2ClientDependencyException(
                    message: "Event listener client dependency error occurred, contact support.",

                    innerException: eventListenerV2OrchestrationDependencyException
                        .InnerException as Xeption);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(eventListenerV2OrchestrationDependencyException);

            // when
            ValueTask<IQueryable<EventListenerV2>> retrieveEventListenerV2sTask =
                this.eventListenerV2sClient.RetrieveEventListenerV2sByEventAddressIdAsync(
                    someEventAddressId,
                    randomCancellationToken);

            EventListenerV2ClientDependencyException
                actualEventListenerV2ClientDependencyException =
                    await Assert.ThrowsAsync<EventListenerV2ClientDependencyException>(
                        retrieveEventListenerV2sTask.AsTask);

            // then
            actualEventListenerV2ClientDependencyException.Should()
                .BeEquivalentTo(expectedEventListenerV2ClientDependencyException);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task
            ShouldThrowServiceExceptionOnRetrieveByEventAddressIdIfServiceErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventAddressId = GetRandomId();
            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.AddData(GetRandomString(), GetRandomString());

            var eventListenerV2OrchestrationServiceException =
                new EventListenerV2OrchestrationServiceException(
                    someMessage,
                    someInnerException);

            var expectedEventListenerV2ClientServiceException =
                new EventListenerV2ClientServiceException(
                    message: "Event listener client service error occurred, contact support.",

                    innerException: eventListenerV2OrchestrationServiceException
                        .InnerException as Xeption);

            this.eventListenerV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(eventListenerV2OrchestrationServiceException);

            // when
            ValueTask<IQueryable<EventListenerV2>> retrieveEventListenerV2sTask =
                this.eventListenerV2sClient.RetrieveEventListenerV2sByEventAddressIdAsync(
                    someEventAddressId,
                    randomCancellationToken);

            EventListenerV2ClientServiceException actualEventListenerV2ClientServiceException =
                await Assert.ThrowsAsync<EventListenerV2ClientServiceException>(
                    retrieveEventListenerV2sTask.AsTask);

            // then
            actualEventListenerV2ClientServiceException.Should()
                .BeEquivalentTo(expectedEventListenerV2ClientServiceException);

            this.eventListenerV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveEventListenerV2sByEventAddressIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.eventListenerV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
