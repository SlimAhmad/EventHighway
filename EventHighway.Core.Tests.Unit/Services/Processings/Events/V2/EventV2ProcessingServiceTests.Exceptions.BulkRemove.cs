// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.Events.V2;
using EventHighway.Core.Models.Services.Processings.Events.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.Events.V2
{
    public partial class EventV2ProcessingServiceTests
    {
        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationOnBulkRemoveIfDependencyValidationErrorOccursAndLogItAsync(
            Xeption eventV2ValidationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<EventV2> someEventV2s = CreateRandomEventV2s();
            IEnumerable<EventV2> inputEventV2s = someEventV2s;

            var expectedEventV2ProcessingDependencyValidationException =
                new EventV2ProcessingDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: eventV2ValidationException.InnerException as Xeption);

            this.eventV2ServiceMock.Setup(service =>
                service.BulkRemoveEventV2sAsync(
                    inputEventV2s,
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(eventV2ValidationException);

            // when
            ValueTask bulkRemoveEventV2sTask =
                this.eventV2ProcessingService.BulkRemoveEventV2sAsync(
                    inputEventV2s,
                    randomCancellationToken);

            EventV2ProcessingDependencyValidationException
                actualEventV2ProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<EventV2ProcessingDependencyValidationException>(
                        bulkRemoveEventV2sTask.AsTask);

            // then
            actualEventV2ProcessingDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventV2ProcessingDependencyValidationException);

            this.eventV2ServiceMock.Verify(service =>
                service.BulkRemoveEventV2sAsync(
                    inputEventV2s,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ProcessingDependencyValidationException))),
                        Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnBulkRemoveIfDependencyExceptionOccursAndLogItAsync(
            Xeption eventV2DependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            IQueryable<EventV2> someEventV2s = CreateRandomEventV2s();
            IEnumerable<EventV2> inputEventV2s = someEventV2s;

            var expectedEventV2ProcessingDependencyException =
                new EventV2ProcessingDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: eventV2DependencyException.InnerException as Xeption);

            this.eventV2ServiceMock.Setup(service =>
                service.BulkRemoveEventV2sAsync(
                    inputEventV2s,
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(eventV2DependencyException);

            // when
            ValueTask bulkRemoveEventV2sTask =
                this.eventV2ProcessingService.BulkRemoveEventV2sAsync(
                    inputEventV2s,
                    randomCancellationToken);

            EventV2ProcessingDependencyException actualEventV2ProcessingDependencyException =
                await Assert.ThrowsAsync<EventV2ProcessingDependencyException>(
                    bulkRemoveEventV2sTask.AsTask);

            // then
            actualEventV2ProcessingDependencyException.Should()
                .BeEquivalentTo(expectedEventV2ProcessingDependencyException);

            this.eventV2ServiceMock.Verify(service =>
                service.BulkRemoveEventV2sAsync(
                    inputEventV2s,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ProcessingDependencyException))),
                        Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnBulkRemoveIfExceptionOccursAndLogItAsync()
        {
            // given
            IQueryable<EventV2> someEventV2s = CreateRandomEventV2s();
            IEnumerable<EventV2> inputEventV2s = someEventV2s;
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventV2ProcessingServiceException =
                new FailedEventV2ProcessingServiceException(
                    message: "Failed event service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventV2ProcessingServiceException =
                new EventV2ProcessingServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: failedEventV2ProcessingServiceException);

            this.eventV2ServiceMock.Setup(service =>
                service.BulkRemoveEventV2sAsync(
                    inputEventV2s,
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask bulkRemoveEventV2sTask =
                this.eventV2ProcessingService.BulkRemoveEventV2sAsync(
                    inputEventV2s,
                    TestContext.Current.CancellationToken);

            EventV2ProcessingServiceException actualEventV2ProcessingServiceException =
                await Assert.ThrowsAsync<EventV2ProcessingServiceException>(
                    bulkRemoveEventV2sTask.AsTask);

            // then
            actualEventV2ProcessingServiceException.Should()
                .BeEquivalentTo(expectedEventV2ProcessingServiceException);

            this.eventV2ServiceMock.Verify(service =>
                service.BulkRemoveEventV2sAsync(
                    inputEventV2s,
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ProcessingServiceException))),
                        Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
