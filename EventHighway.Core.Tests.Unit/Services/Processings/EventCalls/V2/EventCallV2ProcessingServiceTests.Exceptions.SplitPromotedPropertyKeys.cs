// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Core.Models.Services.Processings.EventCalls.V2.Exceptions;
using EventHighway.Core.Services.Processings.EventCalls.V2;
using FluentAssertions;
using Moq;

namespace EventHighway.Core.Tests.Unit.Services.Processings.EventCalls.V2
{
    public partial class EventCallV2ProcessingServiceTests
    {
        [Fact]
        public async Task ShouldThrowServiceExceptionOnSplitPromotedPropertyKeysIfServiceErrorOccursAndLogItAsync()
        {
            // given
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
                this.loggingBrokerMock.Object)
            {
                CallBase = true
            };

            eventCallV2ProcessingServiceMock.Setup(service =>
                service.ValidatePromotedProperties(It.IsAny<string>()))
                    .Throws(serviceException);

            IEventCallV2ProcessingService eventCallV2ProcessingService =
                eventCallV2ProcessingServiceMock.Object;

            // when
            ValueTask<IEnumerable<string>> splitPromotedPropertyKeysTask =
                eventCallV2ProcessingService.SplitPromotedPropertyKeysAsync(
                    somePromotedProperties,
                    TestContext.Current.CancellationToken);

            EventCallV2ProcessingServiceException
                actualEventCallV2ProcessingServiceException =
                    await Assert.ThrowsAsync<EventCallV2ProcessingServiceException>(
                        splitPromotedPropertyKeysTask.AsTask);

            // then
            actualEventCallV2ProcessingServiceException.Should()
                .BeEquivalentTo(expectedEventCallV2ProcessingServiceException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventCallV2ProcessingServiceException))),
                        Times.Once);

            this.eventCallV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnSplitPromotedPropertyKeysIfTimeoutOccursAndLogItAsync()
        {
            // given
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
                this.loggingBrokerMock.Object)
            {
                CallBase = true
            };

            eventCallV2ProcessingServiceMock.Setup(service =>
                service.ValidatePromotedProperties(It.IsAny<string>()))
                    .Throws(operationCanceledException);

            IEventCallV2ProcessingService eventCallV2ProcessingService =
                eventCallV2ProcessingServiceMock.Object;

            // when
            ValueTask<IEnumerable<string>> splitPromotedPropertyKeysTask =
                eventCallV2ProcessingService.SplitPromotedPropertyKeysAsync(
                    somePromotedProperties,
                    TestContext.Current.CancellationToken);

            EventCallV2ProcessingDependencyException
                actualEventCallV2ProcessingDependencyException =
                    await Assert.ThrowsAsync<EventCallV2ProcessingDependencyException>(
                        splitPromotedPropertyKeysTask.AsTask);

            // then
            actualEventCallV2ProcessingDependencyException.Should()
                .BeEquivalentTo(expectedEventCallV2ProcessingDependencyException);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogErrorAsync(It.Is(SameExceptionAs(
                    expectedEventCallV2ProcessingDependencyException))),
                        Times.Once);

            this.eventCallV2ServiceMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
