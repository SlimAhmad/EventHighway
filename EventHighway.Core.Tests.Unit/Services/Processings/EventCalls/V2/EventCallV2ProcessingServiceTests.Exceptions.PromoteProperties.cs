// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Foundations.PromotedProperties;
using EventHighway.Core.Models.Services.Processings.EventCalls.V2.Exceptions;
using EventHighway.Core.Services.Processings.EventCalls.V2;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventCalls.V2
{
    public partial class EventCallV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowServiceExceptionOnPromotePropertiesIfServiceErrorOccursAndLogItAsync()
        {
            // given
            string someContent = GetRandomString();
            string somePromotedProperties = GetRandomString();
            var serviceException = new Exception();
            serviceException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var failedEventCallV2ProcessingServiceException =
                new FailedEventCallV2ProcessingServiceException(
                    message: "Failed event call service error occurred, contact support.",
                    innerException: serviceException,
                    data: serviceException.Data);

            var expectedEventCallV2ProcessingServiceException =
                new EventCallV2ProcessingServiceException(
                    message: "Event call service error occurred, contact support.",
                    innerException: failedEventCallV2ProcessingServiceException);

            var eventCallV2ProcessingServiceMock = new Mock<EventCallV2ProcessingService>(
                this.eventCallV2ServiceMock.Object,
                this.jsonBrokerMock.Object,
                this.loggingBrokerMock.Object)
            {
                CallBase = true
            };

            eventCallV2ProcessingServiceMock.Setup(service =>
                service.ValidatePromotePropertiesInputs(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                        .Throws(serviceException);

            IEventCallV2ProcessingService eventCallV2ProcessingService =
                eventCallV2ProcessingServiceMock.Object;

            // when
            ValueTask<List<PromotedProperty>> promotePropertiesTask =
                eventCallV2ProcessingService.PromotePropertiesAsync(
                    someContent,
                    somePromotedProperties,
                    TestContext.Current.CancellationToken);

            EventCallV2ProcessingServiceException
                actualEventCallV2ProcessingServiceException =
                    await Assert.ThrowsAsync<EventCallV2ProcessingServiceException>(
                        promotePropertiesTask.AsTask);

            // then
            actualEventCallV2ProcessingServiceException.Should()
                .BeEquivalentTo(expectedEventCallV2ProcessingServiceException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventCallV2ProcessingServiceException))),
                        Times.Once);

            this.eventCallV2ServiceMock.VerifyNoOtherCalls();
            this.jsonBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnPromotePropertiesIfTimeoutOccursAndLogItAsync()
        {
            // given
            string someContent = GetRandomString();
            string somePromotedProperties = GetRandomString();

            var operationCanceledException = new OperationCanceledException();

            var timeoutException =
                new TimeoutException("The dependency operation timed out.");

            var timeoutEventCallV2ProcessingException =
                new TimeoutEventCallV2ProcessingException(
                    message: "Failed event call processing timeout error occurred, contact support.",
                    innerException: timeoutException,
                    data: timeoutException.Data);

            var expectedEventCallV2ProcessingDependencyException =
                new EventCallV2ProcessingDependencyException(
                    message: "Event call dependency error occurred, contact support.",
                    innerException: timeoutEventCallV2ProcessingException);

            var eventCallV2ProcessingServiceMock = new Mock<EventCallV2ProcessingService>(
                this.eventCallV2ServiceMock.Object,
                this.jsonBrokerMock.Object,
                this.loggingBrokerMock.Object)
            {
                CallBase = true
            };

            eventCallV2ProcessingServiceMock.Setup(service =>
                service.ValidatePromotePropertiesInputs(
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                        .Throws(operationCanceledException);

            IEventCallV2ProcessingService eventCallV2ProcessingService =
                eventCallV2ProcessingServiceMock.Object;

            // when
            ValueTask<List<PromotedProperty>> promotePropertiesTask =
                eventCallV2ProcessingService.PromotePropertiesAsync(
                    someContent,
                    somePromotedProperties,
                    TestContext.Current.CancellationToken);

            EventCallV2ProcessingDependencyException
                actualEventCallV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<EventCallV2ProcessingDependencyException>(
                        promotePropertiesTask.AsTask);

            // then
            actualEventCallV2ProcessingDependencyException.Should()
                .BeEquivalentTo(expectedEventCallV2ProcessingDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventCallV2ProcessingDependencyException))),
                        Times.Once);

            this.eventCallV2ServiceMock.VerifyNoOtherCalls();
            this.jsonBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowOperationCanceledExceptionRawWhenCancellationIsRequestedOnPromotePropertiesAsync()
        {
            // given
            string someContent = GetRandomString();
            string somePromotedProperties = GetRandomString();

            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            CancellationToken cancelledToken = cancellationTokenSource.Token;

            // when
            ValueTask<List<PromotedProperty>> promotePropertiesTask =
                this.eventCallV2ProcessingService.PromotePropertiesAsync(
                    someContent,
                    somePromotedProperties,
                    cancelledToken);

            // then
            OperationCanceledException actualException =
                await Assert.ThrowsAsync<OperationCanceledException>(
                    promotePropertiesTask.AsTask);

            actualException.Should().NotBeOfType<EventCallV2ProcessingDependencyException>();
            actualException.Should().NotBeOfType<EventCallV2ProcessingServiceException>();
            actualException.CancellationToken.IsCancellationRequested.Should().BeTrue();

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCriticalAsync(It.IsAny<Xeption>()),
                    Times.Never);

            this.eventCallV2ServiceMock.VerifyNoOtherCalls();
            this.jsonBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
