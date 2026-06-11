// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;
using EventHighway.Core.Models.Services.Processings.EventCalls.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventCalls.V2
{
    public partial class EventCallV2ProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(EventCallV2ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnRunIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption eventCallV2ValidationException)
        {
            // given
            EventCallV2 someEventCallV2 = CreateRandomEventCallV2();

            var expectedEventCallV2ProcessingDependencyValidationException =
                new EventCallV2ProcessingDependencyValidationException(
                    message: "Event call validation error occurred, fix the errors and try again.",
                    innerException: eventCallV2ValidationException.InnerException as Xeption);

            this.eventCallV2ServiceMock.Setup(service =>
                service.RunEventCallV2Async(
                    It.IsAny<EventCallV2>(),
                    TestContext.Current.CancellationToken))
                        .ThrowsAsync(eventCallV2ValidationException);

            // when
            ValueTask<EventCallV2> runEventCallV2Task =
                this.eventCallV2ProcessingService.RunEventCallV2Async(
                    someEventCallV2,
                    TestContext.Current.CancellationToken);

            EventCallV2ProcessingDependencyValidationException
                actualEventCallV2ProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<EventCallV2ProcessingDependencyValidationException>(
                        runEventCallV2Task.AsTask);

            // then
            actualEventCallV2ProcessingDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventCallV2ProcessingDependencyValidationException);

            this.eventCallV2ServiceMock.Verify(service =>
                service.RunEventCallV2Async(
                    It.IsAny<EventCallV2>(),
                    TestContext.Current.CancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventCallV2ProcessingDependencyValidationException))),
                        Times.Once);

            this.eventCallV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(EventCallV2DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRunIfDependencyExceptionOccursAndLogItAsync(
            Xeption eventCallV2DependencyException)
        {
            // given
            EventCallV2 someEventCallV2 = CreateRandomEventCallV2();

            var expectedEventCallV2ProcessingDependencyException =
                new EventCallV2ProcessingDependencyException(
                    message: "Event call dependency error occurred, contact support.",
                    innerException: eventCallV2DependencyException.InnerException as Xeption);

            this.eventCallV2ServiceMock.Setup(service =>
                service.RunEventCallV2Async(
                    It.IsAny<EventCallV2>(),
                    TestContext.Current.CancellationToken))
                        .ThrowsAsync(eventCallV2DependencyException);

            // when
            ValueTask<EventCallV2> runEventCallV2Task =
                this.eventCallV2ProcessingService.RunEventCallV2Async(
                    someEventCallV2,
                    TestContext.Current.CancellationToken);

            EventCallV2ProcessingDependencyException
                actualEventCallV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<EventCallV2ProcessingDependencyException>(
                        runEventCallV2Task.AsTask);

            // then
            actualEventCallV2ProcessingDependencyException.Should()
                .BeEquivalentTo(expectedEventCallV2ProcessingDependencyException);

            this.eventCallV2ServiceMock.Verify(service =>
                service.RunEventCallV2Async(
                    It.IsAny<EventCallV2>(),
                    TestContext.Current.CancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventCallV2ProcessingDependencyException))),
                        Times.Once);

            this.eventCallV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRunIfExceptionOccursAndLogItAsync()
        {
            // given
            EventCallV2 someEventCallV2 = CreateRandomEventCallV2();
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventCallV2ProcessingServiceException =
                new FailedEventCallV2ProcessingServiceException(
                    message: "Failed event call service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventCallV2ProcessingServiceException =
                new EventCallV2ProcessingServiceException(
                    message: "Event call service error occurred, contact support.",
                    innerException: failedEventCallV2ProcessingServiceException);

            this.eventCallV2ServiceMock.Setup(service =>
                service.RunEventCallV2Async(
                    It.IsAny<EventCallV2>(),
                    TestContext.Current.CancellationToken))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<EventCallV2> runEventCallV2Task =
                this.eventCallV2ProcessingService.RunEventCallV2Async(
                    someEventCallV2,
                    TestContext.Current.CancellationToken);

            EventCallV2ProcessingServiceException
                actualEventCallV2ProcessingServiceException =
                    await Assert.ThrowsAsync<EventCallV2ProcessingServiceException>(
                        runEventCallV2Task.AsTask);

            // then
            actualEventCallV2ProcessingServiceException.Should()
                .BeEquivalentTo(expectedEventCallV2ProcessingServiceException);

            this.eventCallV2ServiceMock.Verify(service =>
                service.RunEventCallV2Async(
                    It.IsAny<EventCallV2>(),
                    TestContext.Current.CancellationToken),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventCallV2ProcessingServiceException))),
                        Times.Once);

            this.eventCallV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
