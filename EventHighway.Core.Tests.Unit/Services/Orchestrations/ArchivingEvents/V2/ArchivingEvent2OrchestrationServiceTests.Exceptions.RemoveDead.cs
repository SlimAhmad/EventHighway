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
    public partial class ArchivingEvent2OrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(EventV2ValidationExceptions))]
        [MemberData(nameof(ListenerEventV2ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnRemoveDeadIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            EventV2 someEventV2 = CreateRandomEventV2();

            var expectedArchivingEvent2OrchestrationDependencyValidationException =
                new ArchivingEvent2OrchestrationDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RemoveListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask removeEventV2AndListenerEventV2sTask =
                this.archivingEvent2OrchestrationService.RemoveEventV2AndListenerEventV2sAsync(
                    someEventV2,
                    TestContext.Current.CancellationToken);

            ArchivingEvent2OrchestrationDependencyValidationException
                actualArchivingEvent2OrchestrationDependencyValidationException =
                    await Assert.ThrowsAsync<ArchivingEvent2OrchestrationDependencyValidationException>(
                        removeEventV2AndListenerEventV2sTask.AsTask);

            // then
            actualArchivingEvent2OrchestrationDependencyValidationException.Should().BeEquivalentTo(
                expectedArchivingEvent2OrchestrationDependencyValidationException);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RemoveListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEvent2OrchestrationDependencyValidationException))),
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
        [MemberData(nameof(EventV2DependencyExceptions))]
        [MemberData(nameof(ListenerEventV2DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRemoveDeadIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            EventV2 someEventV2 = CreateRandomEventV2();

            var expectedArchivingEvent2OrchestrationDependencyException =
                new ArchivingEvent2OrchestrationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RemoveListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask removeEventV2AndListenerEventV2sTask =
                this.archivingEvent2OrchestrationService.RemoveEventV2AndListenerEventV2sAsync(
                    someEventV2,
                    TestContext.Current.CancellationToken);

            ArchivingEvent2OrchestrationDependencyException
                actualArchivingEvent2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<ArchivingEvent2OrchestrationDependencyException>(
                        removeEventV2AndListenerEventV2sTask.AsTask);

            // then
            actualArchivingEvent2OrchestrationDependencyException.Should().BeEquivalentTo(
                expectedArchivingEvent2OrchestrationDependencyException);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RemoveListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEvent2OrchestrationDependencyException))),
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

            var failedArchivingEvent2OrchestrationServiceException =
                new FailedArchivingEvent2OrchestrationServiceException(
                    message: "Failed event service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedArchivingEvent2OrchestrationServiceException =
                new ArchivingEvent2OrchestrationServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: failedArchivingEvent2OrchestrationServiceException);

            this.listenerEventV2ProcessingServiceMock.Setup(service =>
                service.RemoveListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask removeEventV2AndListenerEventV2sTask =
                this.archivingEvent2OrchestrationService.RemoveEventV2AndListenerEventV2sAsync(
                    someEventV2,
                    TestContext.Current.CancellationToken);

            ArchivingEvent2OrchestrationServiceException
                actualArchivingEvent2OrchestrationServiceException =
                    await Assert.ThrowsAsync<ArchivingEvent2OrchestrationServiceException>(
                        removeEventV2AndListenerEventV2sTask.AsTask);

            // then
            actualArchivingEvent2OrchestrationServiceException.Should().BeEquivalentTo(
                expectedArchivingEvent2OrchestrationServiceException);

            this.listenerEventV2ProcessingServiceMock.Verify(service =>
                service.RemoveListenerEventV2ByIdAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEvent2OrchestrationServiceException))),
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
