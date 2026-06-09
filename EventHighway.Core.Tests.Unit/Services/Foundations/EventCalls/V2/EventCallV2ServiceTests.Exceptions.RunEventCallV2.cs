// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2;
using EventHighway.Core.Models.Services.Foundations.EventCall.V2.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Foundations.EventCalls.V2
{
    public partial class EventCallV2ServiceTests
    {
        [Theory]
        [MemberData(nameof(CriticalEventHandlerDependencyExceptions))]
        public async Task ShouldThrowCriticalDependencyExceptionOnRunIfCriticalDependencyErrorOccursAndLogItAsync(
            Exception criticalDependencyException)
        {
            // given
            EventCallV2 someEventCallV2 = CreateRandomEventCallV2();
            criticalDependencyException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            this.eventHandlerBrokerMock.Setup(broker => broker.Name)
                .Returns(someEventCallV2.HandlerName);

            this.eventHandlerBrokerMock.Setup(broker => broker.RequiredParams)
                .Returns(System.Array.Empty<string>());

            this.eventHandlerBrokerMock
                .Setup(broker =>
                    broker.HandleAsync(
                        It.IsAny<string>(),
                        It.IsAny<IReadOnlyDictionary<string, string>>(),
                        It.IsAny<CancellationToken>()))
                .ThrowsAsync(criticalDependencyException);

            var failedEventCallV2DependencyException =
                new FailedEventCallV2DependencyException(
                    message: "Failed event call dependency error occurred, contact support.",
                    innerException: criticalDependencyException,
                    data: criticalDependencyException.Data);

            var expectedEventCallV2DependencyException =
                new EventCallV2DependencyException(
                    message: "Event call dependency error occurred, contact support.",
                    innerException: failedEventCallV2DependencyException);

            // when
            ValueTask<EventCallV2> runEventCallV2Task =
                this.eventCallV2Service.RunEventCallV2Async(someEventCallV2, TestContext.Current.CancellationToken);

            EventCallV2DependencyException actualEventCallV2DependencyException =
                await Assert.ThrowsAsync<EventCallV2DependencyException>(
                    runEventCallV2Task.AsTask);

            // then
            actualEventCallV2DependencyException.Should()
                .BeEquivalentTo(expectedEventCallV2DependencyException);

            this.eventHandlerBrokerMock.VerifyGet(
                broker => broker.Name,
                Times.AtLeastOnce);

            this.eventHandlerBrokerMock.VerifyGet(
                broker => broker.RequiredParams,
                Times.AtLeastOnce);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.HandleAsync(
                    It.IsAny<string>(),
                    It.IsAny<IReadOnlyDictionary<string, string>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.Is(SameExceptionAs(
                    expectedEventCallV2DependencyException))),
                        Times.Once);

            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(DependencyValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnRunIfDependencyValidationErrorOccursAndLogItAsync(
            Exception dependencyValidationException)
        {
            // given
            EventCallV2 someEventCallV2 = CreateRandomEventCallV2();
            dependencyValidationException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            this.eventHandlerBrokerMock.Setup(broker => broker.Name)
                .Returns(someEventCallV2.HandlerName);

            this.eventHandlerBrokerMock.Setup(broker => broker.RequiredParams)
                .Returns(System.Array.Empty<string>());

            this.eventHandlerBrokerMock.Setup(broker =>
                broker.HandleAsync(
                    It.IsAny<string>(),
                    It.IsAny<IReadOnlyDictionary<string, string>>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(dependencyValidationException);

            var failedEventCallV2DependencyValidationException =
                new FailedEventCallV2DependencyValidationException(
                    message: "Failed event call dependency validation error occurred, fix the errors and try again.",
                    innerException: dependencyValidationException,
                    data: dependencyValidationException.Data);

            var expectedEventCallV2DependencyValidationException =
                new EventCallV2DependencyValidationException(
                    message: "Event call validation error occurred, fix the errors and try again.",
                    innerException: failedEventCallV2DependencyValidationException);

            // when
            ValueTask<EventCallV2> runEventCallV2Task =
                this.eventCallV2Service.RunEventCallV2Async(someEventCallV2, TestContext.Current.CancellationToken);

            EventCallV2DependencyValidationException actualEventCallV2DependencyValidationException =
                await Assert.ThrowsAsync<EventCallV2DependencyValidationException>(
                    runEventCallV2Task.AsTask);

            // then
            actualEventCallV2DependencyValidationException.Should()
                .BeEquivalentTo(expectedEventCallV2DependencyValidationException);

            this.eventHandlerBrokerMock.VerifyGet(broker => broker.Name,
                Times.AtLeastOnce);

            this.eventHandlerBrokerMock.VerifyGet(broker => broker.RequiredParams,
                Times.AtLeastOnce);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.HandleAsync(
                    It.IsAny<string>(),
                    It.IsAny<IReadOnlyDictionary<string, string>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventCallV2DependencyValidationException))),
                        Times.Once);

            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(ServiceExceptions))]
        public async Task ShouldThrowDependencyExceptionOnRunIfServiceExceptionOccursAndLogItAsync(
            Exception serviceException)
        {
            // given
            EventCallV2 someEventCallV2 = CreateRandomEventCallV2();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            this.eventHandlerBrokerMock.Setup(broker => broker.Name)
                .Returns(someEventCallV2.HandlerName);

            this.eventHandlerBrokerMock.Setup(broker => broker.RequiredParams)
                .Returns(System.Array.Empty<string>());

            this.eventHandlerBrokerMock
                .Setup(broker =>
                    broker.HandleAsync(
                        It.IsAny<string>(),
                        It.IsAny<IReadOnlyDictionary<string, string>>(),
                        It.IsAny<CancellationToken>()))
                .ThrowsAsync(serviceException);

            var failedEventCallV2DependencyException =
                new FailedEventCallV2DependencyException(
                    message: "Failed event call dependency error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventCallV2DependencyException =
                new EventCallV2DependencyException(
                    message: "Event call dependency error occurred, contact support.",
                    innerException: failedEventCallV2DependencyException);

            // when
            ValueTask<EventCallV2> runEventCallV2Task =
                this.eventCallV2Service.RunEventCallV2Async(someEventCallV2, TestContext.Current.CancellationToken);

            EventCallV2DependencyException actualEventCallV2DependencyException =
                await Assert.ThrowsAsync<EventCallV2DependencyException>(
                    runEventCallV2Task.AsTask);

            // then
            actualEventCallV2DependencyException.Should()
                .BeEquivalentTo(expectedEventCallV2DependencyException);

            this.eventHandlerBrokerMock.VerifyGet(broker => broker.Name,
                Times.AtLeastOnce);

            this.eventHandlerBrokerMock.VerifyGet(broker => broker.RequiredParams,
                Times.AtLeastOnce);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.HandleAsync(
                    It.IsAny<string>(),
                    It.IsAny<IReadOnlyDictionary<string, string>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventCallV2DependencyException))),
                        Times.Once);

            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRunIfUnexpectedExceptionOccursAndLogItAsync()
        {
            // given
            EventCallV2 someEventCallV2 = CreateRandomEventCallV2();
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            this.eventHandlerBrokerMock.Setup(broker => broker.Name)
                .Returns(someEventCallV2.HandlerName);

            this.eventHandlerBrokerMock.Setup(broker => broker.RequiredParams)
                .Returns(System.Array.Empty<string>());

            this.eventHandlerBrokerMock
                .Setup(broker => broker.HandleAsync(
                    It.IsAny<string>(),
                    It.IsAny<IReadOnlyDictionary<string, string>>(),
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(serviceException);

            var failedEventCallV2ServiceException =
                new FailedEventCallV2ServiceException(
                    message: "Failed event call service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventCallV2ServiceException =
                new EventCallV2ServiceException(
                    message: "Event call service error occurred, contact support.",
                    innerException: failedEventCallV2ServiceException);

            // when
            ValueTask<EventCallV2> runEventCallV2Task =
                this.eventCallV2Service.RunEventCallV2Async(someEventCallV2, TestContext.Current.CancellationToken);

            EventCallV2ServiceException actualEventCallV2ServiceException =
                await Assert.ThrowsAsync<EventCallV2ServiceException>(
                    runEventCallV2Task.AsTask);

            // then
            actualEventCallV2ServiceException.Should()
                .BeEquivalentTo(expectedEventCallV2ServiceException);

            this.eventHandlerBrokerMock.VerifyGet(
                broker => broker.Name,
                Times.AtLeastOnce);

            this.eventHandlerBrokerMock.VerifyGet(
                broker => broker.RequiredParams,
                Times.AtLeastOnce);

            this.eventHandlerBrokerMock.Verify(broker =>
                broker.HandleAsync(
                    It.IsAny<string>(),
                    It.IsAny<IReadOnlyDictionary<string, string>>(),
                    It.IsAny<CancellationToken>()),
                        Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventCallV2ServiceException))),
                        Times.Once);

            this.eventHandlerBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
