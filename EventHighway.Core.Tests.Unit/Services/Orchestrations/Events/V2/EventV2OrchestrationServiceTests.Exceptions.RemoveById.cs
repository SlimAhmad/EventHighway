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
        [MemberData(nameof(EventV2ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnRemoveByIdIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventV2Id = GetRandomId();

            var expectedEventV2OrchestrationDependencyValidationException =
                new EventV2OrchestrationDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventV2ProcessingServiceMock.Setup(service =>
                service.RemoveEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<EventV2> removeEventV2ByIdTask =
                this.eventV2OrchestrationService.RemoveEventV2ByIdAsync(
                    someEventV2Id,
                    randomCancellationToken);

            EventV2OrchestrationDependencyValidationException
                actualEventV2OrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<EventV2OrchestrationDependencyValidationException>(
                        removeEventV2ByIdTask.AsTask);

            // then
            actualEventV2OrchestrationDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventV2OrchestrationDependencyValidationException);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RemoveEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2OrchestrationDependencyValidationException))),
                        Times.Once);

            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(EventV2DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRemoveByIdIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventV2Id = GetRandomId();

            var expectedEventV2OrchestrationDependencyException =
                new EventV2OrchestrationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventV2ProcessingServiceMock.Setup(service =>
                service.RemoveEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<EventV2> removeEventV2ByIdTask =
                this.eventV2OrchestrationService.RemoveEventV2ByIdAsync(
                    someEventV2Id,
                    randomCancellationToken);

            EventV2OrchestrationDependencyException
                actualEventV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<EventV2OrchestrationDependencyException>(
                        removeEventV2ByIdTask.AsTask);

            // then
            actualEventV2OrchestrationDependencyException.Should()
                .BeEquivalentTo(expectedEventV2OrchestrationDependencyException);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RemoveEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveByIdIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            Guid someEventV2Id = GetRandomId();
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

            this.eventV2ProcessingServiceMock.Setup(service =>
                service.RemoveEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<EventV2> removeEventV2ByIdTask =
                this.eventV2OrchestrationService.RemoveEventV2ByIdAsync(
                    someEventV2Id,
                    randomCancellationToken);

            EventV2OrchestrationServiceException
                actualEventV2OrchestrationServiceException =
                    await Assert.ThrowsAsync<EventV2OrchestrationServiceException>(
                        removeEventV2ByIdTask.AsTask);

            // then
            actualEventV2OrchestrationServiceException.Should()
                .BeEquivalentTo(expectedEventV2OrchestrationServiceException);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RemoveEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2OrchestrationServiceException))),
                        Times.Once);

            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventCallV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
