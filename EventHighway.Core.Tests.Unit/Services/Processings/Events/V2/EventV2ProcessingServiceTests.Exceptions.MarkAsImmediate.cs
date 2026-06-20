// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
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
        public async Task ShouldThrowDependencyValidationErrorOnMarkImmediateIfDependencyValidationOccursAndLogItAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 someEventV2 = CreateRandomEventV2();

            var expectedEventV2ProcessingDependencyValidationException =
                new EventV2ProcessingDependencyValidationException(
                    message: "Event validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption);

            this.dateTimeBrokerMock.Setup(service =>
                service.GetDateTimeOffsetAsync())
                    .ThrowsAsync(validationException);

            // when
            ValueTask<EventV2> markEventV2AsImmediateTask =
                this.eventV2ProcessingService.MarkEventV2AsImmediateAsync(someEventV2, randomCancellationToken);

            EventV2ProcessingDependencyValidationException
                actualEventV2ProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<EventV2ProcessingDependencyValidationException>(
                        markEventV2AsImmediateTask.AsTask);

            // then
            actualEventV2ProcessingDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventV2ProcessingDependencyValidationException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ProcessingDependencyValidationException))),
                        Times.Once);

            this.eventV2ServiceMock.Verify(service =>
                service.ModifyEventV2Async(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnMarkImmediateIfDependencyExceptionOccursAndLogItAsync(
            Xeption dependencyException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 someEventV2 = CreateRandomEventV2();

            var expectedEventV2ProcessingDependencyException =
                new EventV2ProcessingDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption);

            this.dateTimeBrokerMock.Setup(service =>
                service.GetDateTimeOffsetAsync())
                    .ThrowsAsync(dependencyException);

            // when
            ValueTask<EventV2> markEventV2AsImmediateTask =
                this.eventV2ProcessingService.MarkEventV2AsImmediateAsync(someEventV2, randomCancellationToken);

            EventV2ProcessingDependencyException
                actualEventV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<EventV2ProcessingDependencyException>(
                        markEventV2AsImmediateTask.AsTask);

            // then
            actualEventV2ProcessingDependencyException.Should()
                .BeEquivalentTo(expectedEventV2ProcessingDependencyException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ProcessingDependencyException))),
                        Times.Once);

            this.eventV2ServiceMock.Verify(service =>
                service.ModifyEventV2Async(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnMarkImmediateIfExceptionOccursAndLogItAsync()
        {
            // given
            CancellationToken cancellationToken =
                TestContext.Current.CancellationToken;

            EventV2 someEventV2 = CreateRandomEventV2();
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventV2ProcessingServiceException =
                new FailedEventV2ProcessingServiceException(
                    message: "Failed event service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventV2ProcessingExceptionException =
                new EventV2ProcessingServiceException(
                    message: "Event service error occurred, contact support.",
                    innerException: failedEventV2ProcessingServiceException);

            this.dateTimeBrokerMock.Setup(service =>
                service.GetDateTimeOffsetAsync())
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<EventV2> markEventV2AsImmediateTask =
                this.eventV2ProcessingService.MarkEventV2AsImmediateAsync(
                    someEventV2,
                    cancellationToken);

            EventV2ProcessingServiceException
                actualEventV2ProcessingServiceException =
                    await Assert.ThrowsAsync<EventV2ProcessingServiceException>(
                        markEventV2AsImmediateTask.AsTask);

            // then
            actualEventV2ProcessingServiceException.Should()
                .BeEquivalentTo(expectedEventV2ProcessingExceptionException);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetDateTimeOffsetAsync(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ProcessingExceptionException))),
                        Times.Once);

            this.eventV2ServiceMock.Verify(service =>
                service.ModifyEventV2Async(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Never);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
