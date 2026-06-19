// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V2;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Coordinations.ArchivingEvents.V2
{
    public partial class ArchivingEventV2CoordinationServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task
            ShouldThrowDependencyValidationOnArchiveDeadEventV2sIfDependencyValidationErrorOccursAndLogItAsync(
                Xeption validationException)
        {
            // given
            var expectedArchivingEventV2CoordinationDependencyValidationException =
                new ArchivingEventV2CoordinationDependencyValidationException(
                    message: "Archiving event validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.archivingEventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllDeadEventV2sWithListenersAsync(
                    It.IsAny<CancellationToken>()))
                        .Returns(CreateThrowingAsyncEnumerable(validationException));

            // when
            ValueTask archiveDeadEventV2sTask =
                this.archivingEventV2CoordinationService
                    .ArchiveDeadEventV2sAsync(TestContext.Current.CancellationToken);

            ArchivingEventV2CoordinationDependencyValidationException
                actualArchivingEventV2CoordinationDependencyValidationException =
                    await Assert.ThrowsAsync<ArchivingEventV2CoordinationDependencyValidationException>(
                        archiveDeadEventV2sTask.AsTask);

            // then
            actualArchivingEventV2CoordinationDependencyValidationException.Should()
                .BeEquivalentTo(expectedArchivingEventV2CoordinationDependencyValidationException);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllDeadEventV2sWithListenersAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2CoordinationDependencyValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.AddEventArchiveV2WithListenerEventArchiveV2sAsync(
                    It.IsAny<EventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RemoveEventV2AndListenerEventV2sAsync(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.archivingEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnArchiveDeadEventV2sIfDependencyErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            var expectedArchivingEventV2CoordinationDependencyException =
                new ArchivingEventV2CoordinationDependencyException(
                    message: "Archiving event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.archivingEventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllDeadEventV2sWithListenersAsync(
                    It.IsAny<CancellationToken>()))
                        .Returns(CreateThrowingAsyncEnumerable(dependencyException));

            // when
            ValueTask archiveDeadEventV2sTask =
                this.archivingEventV2CoordinationService
                    .ArchiveDeadEventV2sAsync(TestContext.Current.CancellationToken);

            ArchivingEventV2CoordinationDependencyException
                actualArchivingEventV2CoordinationDependencyException =
                    await Assert.ThrowsAsync<ArchivingEventV2CoordinationDependencyException>(
                        archiveDeadEventV2sTask.AsTask);

            // then
            actualArchivingEventV2CoordinationDependencyException.Should()
                .BeEquivalentTo(expectedArchivingEventV2CoordinationDependencyException);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllDeadEventV2sWithListenersAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2CoordinationDependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.AddEventArchiveV2WithListenerEventArchiveV2sAsync(
                    It.IsAny<EventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RemoveEventV2AndListenerEventV2sAsync(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.archivingEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnArchiveDeadEventV2sIfExceptionOccursAndLogItAsync()
        {
            // given
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "UnexpectedError" });

            var failedArchivingEventV2CoordinationServiceException =
                new FailedArchivingEventV2CoordinationServiceException(
                    message: "Failed archiving event service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedArchivingEventV2CoordinationServiceException =
                new ArchivingEventV2CoordinationServiceException(
                    message: "Archiving event service error occurred, contact support.",
                    innerException: failedArchivingEventV2CoordinationServiceException);

            this.archivingEventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveAllDeadEventV2sWithListenersAsync(
                    It.IsAny<CancellationToken>()))
                        .Returns(CreateThrowingAsyncEnumerable(serviceException));

            // when
            ValueTask archiveDeadEventV2sTask =
                this.archivingEventV2CoordinationService
                    .ArchiveDeadEventV2sAsync(TestContext.Current.CancellationToken);

            ArchivingEventV2CoordinationServiceException
                actualArchivingEventV2CoordinationServiceException =
                    await Assert.ThrowsAsync<ArchivingEventV2CoordinationServiceException>(
                        archiveDeadEventV2sTask.AsTask);

            // then
            actualArchivingEventV2CoordinationServiceException.Should()
                .BeEquivalentTo(expectedArchivingEventV2CoordinationServiceException);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveAllDeadEventV2sWithListenersAsync(
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2CoordinationServiceException))),
                        Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Never);

            this.eventArchiveV2OrchestrationServiceMock.Verify(service =>
                service.AddEventArchiveV2WithListenerEventArchiveV2sAsync(
                    It.IsAny<EventArchiveV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RemoveEventV2AndListenerEventV2sAsync(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.archivingEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
        }
    }
}
