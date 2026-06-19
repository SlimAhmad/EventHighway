// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Orchestrations.Events.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.Events.V2
{
    public partial class EventV2OrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnSubmitIfValidationExceptionOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 someEventV2 = CreateRandomEventV2();

            var expectedEventV2OrchestrationDependencyValidationException =
                new EventV2OrchestrationDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventAddressV2ProcessingServiceMock.Setup(service =>
                service.RetrieveEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<EventV2> submitEventV2Task =
                this.eventV2OrchestrationService.SubmitEventV2Async(
                    someEventV2,
                    randomCancellationToken);

            EventV2OrchestrationDependencyValidationException
                actualEventV2OrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<EventV2OrchestrationDependencyValidationException>(
                        submitEventV2Task.AsTask);

            // then
            actualEventV2OrchestrationDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventV2OrchestrationDependencyValidationException);

            this.eventAddressV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2OrchestrationDependencyValidationException))),
                        Times.Once);

            this.eventV2ProcessingServiceMock.Verify(broker =>
                broker.AddEventV2Async(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventAddressV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnSubmitIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 someEventV2 = CreateRandomEventV2();

            var expectedEventV2OrchestrationDependencyException =
                new EventV2OrchestrationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventAddressV2ProcessingServiceMock.Setup(service =>
                service.RetrieveEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<EventV2> submitEventV2Task =
                this.eventV2OrchestrationService.SubmitEventV2Async(
                    someEventV2,
                    randomCancellationToken);

            EventV2OrchestrationDependencyException
                actualEventV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<EventV2OrchestrationDependencyException>(
                        submitEventV2Task.AsTask);

            // then
            actualEventV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedEventV2OrchestrationDependencyException);

            this.eventAddressV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventV2ProcessingServiceMock.Verify(broker =>
                broker.AddEventV2Async(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventAddressV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnSubmitIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 someEventV2 = CreateRandomEventV2();
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventV2OrchestrationServiceException =
                new FailedEventV2OrchestrationServiceException(
                    message: "Failed event service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventV2OrchestrationServiceException =
                new EventV2OrchestrationServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: failedEventV2OrchestrationServiceException);

            this.eventAddressV2ProcessingServiceMock.Setup(service =>
                service.RetrieveEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<EventV2> submitEventV2Task =
                this.eventV2OrchestrationService.SubmitEventV2Async(
                    someEventV2,
                    randomCancellationToken);

            EventV2OrchestrationServiceException
                actualEventV2OrchestrationServiceException =
                    await Assert.ThrowsAsync<EventV2OrchestrationServiceException>(
                        submitEventV2Task.AsTask);

            // then
            actualEventV2OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedEventV2OrchestrationServiceException);

            this.eventAddressV2ProcessingServiceMock.Verify(service =>
                service.RetrieveEventAddressV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2OrchestrationServiceException))),
                        Times.Once);

            this.eventV2ProcessingServiceMock.Verify(broker =>
                broker.AddEventV2Async(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventAddressV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
