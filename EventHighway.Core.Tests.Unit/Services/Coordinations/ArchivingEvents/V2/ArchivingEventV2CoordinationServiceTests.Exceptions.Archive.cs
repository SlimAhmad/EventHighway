// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.ArchivingEvents.V2.Exceptions;
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
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            var expectedArchivingEventV2CoordinationDependencyValidationException =
                new ArchivingEventV2CoordinationDependencyValidationException(
                    message: "Archiving event validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.archivingEventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfDeadEventV2sAsync())
                    .ThrowsAsync(validationException);

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

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfDeadEventV2sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2CoordinationDependencyValidationException))),
                        Times.Once);

            this.archivingEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnArchiveDeadEventV2sIfDependencyErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();

            var expectedArchivingEventV2CoordinationDependencyException =
                new ArchivingEventV2CoordinationDependencyException(
                    message: "Archiving event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.archivingEventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfDeadEventV2sAsync())
                    .ThrowsAsync(dependencyException);

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

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfDeadEventV2sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2CoordinationDependencyException))),
                        Times.Once);

            this.archivingEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnArchiveDeadEventV2sIfExceptionOccursAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
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

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetDateTimeOffsetAsync())
                    .ReturnsAsync(randomDateTimeOffset);

            this.archivingEventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfDeadEventV2sAsync())
                    .ThrowsAsync(serviceException);

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

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfDeadEventV2sAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2CoordinationServiceException))),
                        Times.Once);

            this.archivingEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
