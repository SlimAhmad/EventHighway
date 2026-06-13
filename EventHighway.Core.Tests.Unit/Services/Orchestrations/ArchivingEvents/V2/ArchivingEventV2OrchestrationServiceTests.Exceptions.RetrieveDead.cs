// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
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
        [MemberData(nameof(EventV2DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveDeadIfEventV2DependencyOccursAndLogItAsync(
            Xeption eventV2DependencyException)
        {
            // given
            var expectedArchivingEventV2OrchestrationDependencyException =
                new ArchivingEventV2OrchestrationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: eventV2DependencyException.InnerException as Xeption);

            this.eventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllDeadEventV2sWithListenersAsync())
                    .ThrowsAsync(eventV2DependencyException);

            // when
            ArchivingEventV2OrchestrationDependencyException
                actualArchivingEventV2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<ArchivingEventV2OrchestrationDependencyException>(async () =>
                    {
                        await foreach (EventV2 _ in
                            this.archivingEventV2OrchestrationService
                                .RetrieveAllDeadEventV2sWithListenersAsync(
                                    TestContext.Current.CancellationToken)) { }
                    });

            // then
            actualArchivingEventV2OrchestrationDependencyException.Should().BeEquivalentTo(
                expectedArchivingEventV2OrchestrationDependencyException);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllDeadEventV2sWithListenersAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2OrchestrationDependencyException))),
                        Times.Once);

            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveDeadIfExceptionOccursAndLogItAsync()
        {
            // given
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

            this.eventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllDeadEventV2sWithListenersAsync())
                    .ThrowsAsync(serviceException);

            // when
            ArchivingEventV2OrchestrationServiceException
                actualArchivingEventV2OrchestrationServiceException =
                    await Assert.ThrowsAsync<ArchivingEventV2OrchestrationServiceException>(async () =>
                    {
                        await foreach (EventV2 _ in
                            this.archivingEventV2OrchestrationService
                                .RetrieveAllDeadEventV2sWithListenersAsync(
                                    TestContext.Current.CancellationToken)) { }
                    });

            // then
            actualArchivingEventV2OrchestrationServiceException.Should().BeEquivalentTo(
                expectedArchivingEventV2OrchestrationServiceException);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllDeadEventV2sWithListenersAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEventV2OrchestrationServiceException))),
                        Times.Once);

            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
