// ---------------------------------------------------------------------------------- 
// Copyright (c) The Standard Organization, a coalition of the Good-Hearted Engineers 
// ----------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventsArchives.V1;
using EventHighway.Core.Models.Services.Processings.EventArchives.V1.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventArchives.V1
{
    public partial class EventArchiveV1ProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            EventArchiveV1 someEventArchiveV1 = CreateRandomEventArchiveV1();

            var expectedEventV1ArchiveProcessingDependencyValidationException =
                new EventArchiveV1ProcessingDependencyValidationException(
                    message: "Event archive validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.eventArchiveV1ServiceMock.Setup(service =>
                service.AddEventArchiveV1Async(It.IsAny<EventArchiveV1>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask<EventArchiveV1> addEventArchiveV1Task =
                this.eventArchiveV1ProcessingService.AddEventArchiveV1Async(someEventArchiveV1);

            EventArchiveV1ProcessingDependencyValidationException
                actualEventV1ArchiveProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<EventArchiveV1ProcessingDependencyValidationException>(
                        addEventArchiveV1Task.AsTask);

            // then
            actualEventV1ArchiveProcessingDependencyValidationException.Should().BeEquivalentTo(
                expectedEventV1ArchiveProcessingDependencyValidationException);

            this.eventArchiveV1ServiceMock.Verify(service =>
                service.AddEventArchiveV1Async(It.IsAny<EventArchiveV1>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveProcessingDependencyValidationException))),
                        Times.Once);

            this.eventArchiveV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnAddIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            EventArchiveV1 someEventArchiveV1 = CreateRandomEventArchiveV1();

            var expectedEventV1ArchiveProcessingDependencyException =
                new EventArchiveV1ProcessingDependencyException(
                    message: "Event archive dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.eventArchiveV1ServiceMock.Setup(service =>
                service.AddEventArchiveV1Async(It.IsAny<EventArchiveV1>()))
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<EventArchiveV1> addEventArchiveV1Task =
                this.eventArchiveV1ProcessingService.AddEventArchiveV1Async(someEventArchiveV1);

            EventArchiveV1ProcessingDependencyException
                actualEventV1ArchiveProcessingDependencyException =
                    await Assert.ThrowsAsync<EventArchiveV1ProcessingDependencyException>(
                        addEventArchiveV1Task.AsTask);

            // then
            actualEventV1ArchiveProcessingDependencyException.Should().BeEquivalentTo(
                expectedEventV1ArchiveProcessingDependencyException);

            this.eventArchiveV1ServiceMock.Verify(service =>
                service.AddEventArchiveV1Async(It.IsAny<EventArchiveV1>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveProcessingDependencyException))),
                        Times.Once);

            this.eventArchiveV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfExceptionOccursAndLogItAsync()
        {
            // given
            EventArchiveV1 someEventArchiveV1 = CreateRandomEventArchiveV1();
            var serviceException = new Exception();

            var failedEventV1ArchiveProcessingServiceException =
                new FailedEventArchiveV1ProcessingServiceException(
                    message: "Failed event archive service error occurred, contact support.",
                    innerException: serviceException);

            var expectedEventV1ArchiveProcessingExceptionException =
                new EventArchiveV1ProcessingServiceException(
                    message: "Event archive service error occurred, contact support.",
                    innerException: failedEventV1ArchiveProcessingServiceException);

            this.eventArchiveV1ServiceMock.Setup(service =>
                service.AddEventArchiveV1Async(It.IsAny<EventArchiveV1>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<EventArchiveV1> addEventArchiveV1Task =
                this.eventArchiveV1ProcessingService.AddEventArchiveV1Async(
                    someEventArchiveV1);

            EventArchiveV1ProcessingServiceException
                actualEventV1ArchiveProcessingServiceException =
                    await Assert.ThrowsAsync<EventArchiveV1ProcessingServiceException>(
                        addEventArchiveV1Task.AsTask);

            // then
            actualEventV1ArchiveProcessingServiceException.Should().BeEquivalentTo(
                expectedEventV1ArchiveProcessingExceptionException);

            this.eventArchiveV1ServiceMock.Verify(service =>
                service.AddEventArchiveV1Async(It.IsAny<EventArchiveV1>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV1ArchiveProcessingExceptionException))),
                        Times.Once);

            this.eventArchiveV1ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
