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
    public partial class ArchivingEvent2OrchestrationServiceTests
    {
        [Theory]
        [MemberData(nameof(EventV2DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRetrieveDeadIfEventV2DependencyOccursAndLogItAsync(
            Xeption eventV2DependencyException)
        {
            // given
            var expectedArchivingEvent2OrchestrationDependencyException =
                new ArchivingEvent2OrchestrationDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: eventV2DependencyException.InnerException as Xeption);

            this.eventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllDeadEventV2sWithListenersAsync())
                    .ThrowsAsync(eventV2DependencyException);

            // when
            ArchivingEvent2OrchestrationDependencyException
                actualArchivingEvent2OrchestrationDependencyException =
                    await Assert.ThrowsAsync<ArchivingEvent2OrchestrationDependencyException>(async () =>
                    {
                        await foreach (EventV2 _ in
                            this.archivingEvent2OrchestrationService
                                .RetrieveAllDeadEventV2sWithListenersAsync(
                                    TestContext.Current.CancellationToken)) { }
                    });

            // then
            actualArchivingEvent2OrchestrationDependencyException.Should().BeEquivalentTo(
                expectedArchivingEvent2OrchestrationDependencyException);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllDeadEventV2sWithListenersAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEvent2OrchestrationDependencyException))),
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

            var failedArchivingEvent2OrchestrationServiceException =
                new FailedArchivingEvent2OrchestrationServiceException(
                    message: "Failed event service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedArchivingEvent2OrchestrationServiceException =
                new ArchivingEvent2OrchestrationServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: failedArchivingEvent2OrchestrationServiceException);

            this.eventV2ProcessingServiceMock.Setup(service =>
                service.RetrieveAllDeadEventV2sWithListenersAsync())
                    .ThrowsAsync(serviceException);

            // when
            ArchivingEvent2OrchestrationServiceException
                actualArchivingEvent2OrchestrationServiceException =
                    await Assert.ThrowsAsync<ArchivingEvent2OrchestrationServiceException>(async () =>
                    {
                        await foreach (EventV2 _ in
                            this.archivingEvent2OrchestrationService
                                .RetrieveAllDeadEventV2sWithListenersAsync(
                                    TestContext.Current.CancellationToken)) { }
                    });

            // then
            actualArchivingEvent2OrchestrationServiceException.Should().BeEquivalentTo(
                expectedArchivingEvent2OrchestrationServiceException);

            this.eventV2ProcessingServiceMock.Verify(service =>
                service.RetrieveAllDeadEventV2sWithListenersAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedArchivingEvent2OrchestrationServiceException))),
                        Times.Once);

            this.eventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.listenerEventV2ProcessingServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
