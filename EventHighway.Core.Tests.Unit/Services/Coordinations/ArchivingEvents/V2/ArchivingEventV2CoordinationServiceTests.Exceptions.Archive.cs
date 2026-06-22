// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Coordinations.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Orchestrations.ArchivingEvents.V2.Exceptions;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
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
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedArchivingEventV2CoordinationDependencyValidationException =
                new ArchivingEventV2CoordinationDependencyValidationException(
                    message: "Archiving event validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.archivingEventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfQuarantinedEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Enumerable.Empty<EventV2>());

            this.archivingEventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfDeadEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask archiveDeadEventV2sTask =
                this.archivingEventV2CoordinationService
                    .ArchiveEventV2sAsync(randomCancellationToken);

            ArchivingEventV2CoordinationDependencyValidationException
                actualArchivingEventV2CoordinationDependencyValidationException =
                    await Assert.ThrowsAsync<ArchivingEventV2CoordinationDependencyValidationException>(
                        archiveDeadEventV2sTask.AsTask);

            // then
            actualArchivingEventV2CoordinationDependencyValidationException.Should()
                .BeEquivalentTo(expectedArchivingEventV2CoordinationDependencyValidationException);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfQuarantinedEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfDeadEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2CoordinationDependencyValidationException))),
                        Times.Once);

            this.archivingEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnArchiveDeadEventV2sIfDependencyErrorOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedArchivingEventV2CoordinationDependencyException =
                new ArchivingEventV2CoordinationDependencyException(
                    message: "Archiving event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.archivingEventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfQuarantinedEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Enumerable.Empty<EventV2>());

            this.archivingEventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfDeadEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask archiveDeadEventV2sTask =
                this.archivingEventV2CoordinationService
                    .ArchiveEventV2sAsync(randomCancellationToken);

            ArchivingEventV2CoordinationDependencyException
                actualArchivingEventV2CoordinationDependencyException =
                    await Assert.ThrowsAsync<ArchivingEventV2CoordinationDependencyException>(
                        archiveDeadEventV2sTask.AsTask);

            // then
            actualArchivingEventV2CoordinationDependencyException.Should()
                .BeEquivalentTo(expectedArchivingEventV2CoordinationDependencyException);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfQuarantinedEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfDeadEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2CoordinationDependencyException))),
                        Times.Once);

            this.archivingEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnArchiveDeadEventV2sAsync()
        {
            // given
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask archiveDeadEventV2sTask =
                this.archivingEventV2CoordinationService
                    .ArchiveEventV2sAsync(cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    archiveDeadEventV2sTask.AsTask);

            actualException.Should().NotBeOfType<ArchivingEventV2CoordinationDependencyException>();
            actualException.Should().NotBeOfType<ArchivingEventV2CoordinationServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.archivingEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnArchiveDeadEventV2sIfTimeoutOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutArchivingEventV2CoordinationException =
                new TimeoutArchivingEventV2CoordinationException(
                    message: "Failed archiving event coordination timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedArchivingEventV2CoordinationDependencyException =
                new ArchivingEventV2CoordinationDependencyException(
                    message: "Archiving event dependency error occurred, contact support.",
                    innerException: timeoutArchivingEventV2CoordinationException);

            this.archivingEventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfQuarantinedEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Enumerable.Empty<EventV2>());

            this.archivingEventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfDeadEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(operationCanceledException);

            // when
            ValueTask archiveDeadEventV2sTask =
                this.archivingEventV2CoordinationService
                    .ArchiveEventV2sAsync(randomCancellationToken);

            ArchivingEventV2CoordinationDependencyException
                actualArchivingEventV2CoordinationDependencyException =
                    await Assert.ThrowsAsync<ArchivingEventV2CoordinationDependencyException>(
                        archiveDeadEventV2sTask.AsTask);

            // then
            actualArchivingEventV2CoordinationDependencyException.Should()
                .BeEquivalentTo(expectedArchivingEventV2CoordinationDependencyException);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfQuarantinedEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfDeadEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2CoordinationDependencyException))),
                        Times.Once);

            this.archivingEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnArchiveDeadEventV2sIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

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
                service.RetrieveBatchOfQuarantinedEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Enumerable.Empty<EventV2>());

            this.archivingEventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfDeadEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask archiveDeadEventV2sTask =
                this.archivingEventV2CoordinationService
                    .ArchiveEventV2sAsync(randomCancellationToken);

            ArchivingEventV2CoordinationServiceException
                actualArchivingEventV2CoordinationServiceException =
                    await Assert.ThrowsAsync<ArchivingEventV2CoordinationServiceException>(
                        archiveDeadEventV2sTask.AsTask);

            // then
            actualArchivingEventV2CoordinationServiceException.Should()
                .BeEquivalentTo(expectedArchivingEventV2CoordinationServiceException);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfQuarantinedEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfDeadEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2CoordinationServiceException))),
                        Times.Once);

            this.archivingEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldArchiveDeadEventV2sEvenWhenQuarantinedEventV2sThrowAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption();
            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyError" });

            var archivingEventV2OrchestrationDependencyException =
                new ArchivingEventV2OrchestrationDependencyException(
                    someMessage,
                    someInnerException);

            var expectedArchivingEventV2CoordinationDependencyException =
                new ArchivingEventV2CoordinationDependencyException(
                    message: "Archiving event dependency error occurred, contact support.",
                    innerException: someInnerException);

            this.archivingEventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfQuarantinedEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(archivingEventV2OrchestrationDependencyException);

            this.archivingEventV2OrchestrationServiceMock.Setup(service =>
                service.RetrieveBatchOfDeadEventV2sAsync(It.IsAny<CancellationToken>()))
                    .ReturnsAsync(Enumerable.Empty<EventV2>());

            // when
            ValueTask archiveEventV2sTask =
                this.archivingEventV2CoordinationService
                    .ArchiveEventV2sAsync(randomCancellationToken);

            ArchivingEventV2CoordinationDependencyException
                actualArchivingEventV2CoordinationDependencyException =
                    await Assert.ThrowsAsync<ArchivingEventV2CoordinationDependencyException>(
                        archiveEventV2sTask.AsTask);

            // then
            actualArchivingEventV2CoordinationDependencyException.Should()
                .BeEquivalentTo(expectedArchivingEventV2CoordinationDependencyException);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfQuarantinedEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.archivingEventV2OrchestrationServiceMock.Verify(service =>
                service.RetrieveBatchOfDeadEventV2sAsync(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2CoordinationDependencyException))),
                        Times.Once);

            this.archivingEventV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.eventArchiveV2OrchestrationServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
