// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Orchestrations.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Orchestrations.ArchivingEvents.V2
{
    public partial class ArchivingEventV2OrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnRemoveDeadIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            EventV2 someEventV2 = CreateRandomEventV2();

            var expectedArchivingEventV2OrchestrationDependencyValidationException =
                new ArchivingEventV2OrchestrationDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RemoveListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask removeEventV2AndListenerEventV2sTask =
                this.archivingEventV2OrchestrationService.RemoveEventV2AndListenerEventV2sAsync(
                    someEventV2,
                    TestContext.Current.CancellationToken);

            ArchivingEventV2OrchestrationDependencyValidationException
                actualArchivingEventV2OrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<ArchivingEventV2OrchestrationDependencyValidationException>(
                        removeEventV2AndListenerEventV2sTask.AsTask);

            // then
            actualArchivingEventV2OrchestrationDependencyValidationException.Should().BeEquivalentTo(
                expectedArchivingEventV2OrchestrationDependencyValidationException);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RemoveListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2OrchestrationDependencyValidationException))),
                        Times.Once);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RemoveEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRemoveDeadIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            EventV2 someEventV2 = CreateRandomEventV2();

            var expectedArchivingEventV2OrchestrationDependencyException =
                new ArchivingEventV2OrchestrationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RemoveListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask removeEventV2AndListenerEventV2sTask =
                this.archivingEventV2OrchestrationService.RemoveEventV2AndListenerEventV2sAsync(
                    someEventV2,
                    TestContext.Current.CancellationToken);

            ArchivingEventV2OrchestrationDependencyException
                actualArchivingEventV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<ArchivingEventV2OrchestrationDependencyException>(
                        removeEventV2AndListenerEventV2sTask.AsTask);

            // then
            actualArchivingEventV2OrchestrationDependencyException.Should().BeEquivalentTo(
                expectedArchivingEventV2OrchestrationDependencyException);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RemoveListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RemoveEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveDeadIfExceptionOccursAndLogItAsync()
        {
            // given
            EventV2 someEventV2 = CreateRandomEventV2();
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedArchivingEventV2OrchestrationServiceException =
                new FailedArchivingEventV2OrchestrationServiceException(
                    message: "Failed event service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedArchivingEventV2OrchestrationServiceException =
                new ArchivingEventV2OrchestrationServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: failedArchivingEventV2OrchestrationServiceException);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RemoveListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask removeEventV2AndListenerEventV2sTask =
                this.archivingEventV2OrchestrationService.RemoveEventV2AndListenerEventV2sAsync(
                    someEventV2,
                    TestContext.Current.CancellationToken);

            ArchivingEventV2OrchestrationServiceException
                actualArchivingEventV2OrchestrationServiceException =
                    await Assert.ThrowsAsync<ArchivingEventV2OrchestrationServiceException>(
                        removeEventV2AndListenerEventV2sTask.AsTask);

            // then
            actualArchivingEventV2OrchestrationServiceException.Should().BeEquivalentTo(
                expectedArchivingEventV2OrchestrationServiceException);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RemoveListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2OrchestrationServiceException))),
                        Times.Once);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RemoveEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
        }
    }
}
