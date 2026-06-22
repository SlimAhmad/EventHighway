// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
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
        public async Task
            ShouldThrowDependencyValidationExceptionOnModifyEventV2IfDependencyValidationErrorOccursAndLogItAsync(
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

            this.eventV2ServiceMock.Setup(service =>
                service.ModifyEventV2Async(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<EventV2> modifyEventV2Task =
                this.eventV2ProcessingService.ModifyEventV2Async(
                    someEventV2,
                    randomCancellationToken);

            EventV2ProcessingDependencyValidationException
                actualEventV2ProcessingDependencyValidationException =
                    await Assert.ThrowsAsync<EventV2ProcessingDependencyValidationException>(
                        modifyEventV2Task.AsTask);

            // then
            actualEventV2ProcessingDependencyValidationException.Should()
                .BeEquivalentTo(expectedEventV2ProcessingDependencyValidationException);

            this.eventV2ServiceMock.Verify(service =>
                service.ModifyEventV2Async(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ProcessingDependencyValidationException))),
                        Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyExceptions))]
        public async Task
            ShouldThrowDependencyExceptionOnModifyEventV2IfDependencyExceptionOccursAndLogItAsync(
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

            this.eventV2ServiceMock.Setup(service =>
                service.ModifyEventV2Async(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<EventV2> modifyEventV2Task =
                this.eventV2ProcessingService.ModifyEventV2Async(
                    someEventV2,
                    randomCancellationToken);

            EventV2ProcessingDependencyException
                actualEventV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<EventV2ProcessingDependencyException>(
                        modifyEventV2Task.AsTask);

            // then
            actualEventV2ProcessingDependencyException.Should()
                .BeEquivalentTo(expectedEventV2ProcessingDependencyException);

            this.eventV2ServiceMock.Verify(service =>
                service.ModifyEventV2Async(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ProcessingDependencyException))),
                        Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyEventV2IfTimeoutOccursAndLogItAsync()
        {
            // given
            EventV2 someEventV2 = CreateRandomEventV2();
            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutEventV2ProcessingException =
                new TimeoutEventV2ProcessingException(
                    message: "Failed event processing timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedEventV2ProcessingDependencyException =
                new EventV2ProcessingDependencyException(
                    message: "Event dependency error occurred, contact support.",
                    innerException: timeoutEventV2ProcessingException);

            this.eventV2ServiceMock.Setup(service =>
                service.ModifyEventV2Async(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()))
                        .ThrowsAsync(operationCanceledException);

            // when
            ValueTask<EventV2> modifyEventV2Task =
                this.eventV2ProcessingService.ModifyEventV2Async(
                    someEventV2,
                    TestContext.Current.CancellationToken);

            EventV2ProcessingDependencyException actualEventV2ProcessingDependencyException =
                await Assert.ThrowsAsync<EventV2ProcessingDependencyException>(
                    modifyEventV2Task.AsTask);

            // then
            actualEventV2ProcessingDependencyException.Should().BeEquivalentTo(
                expectedEventV2ProcessingDependencyException);

            this.eventV2ServiceMock.Verify(service =>
                service.ModifyEventV2Async(
                    It.IsAny<EventV2>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventV2ProcessingDependencyException))),
                        Times.Once);

            this.eventV2ServiceMock.VerifyNoOtherCalls();
            this.configurationBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
