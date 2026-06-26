// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Core.Models.Clients.HealthChecks.V2.Exceptions;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2;
using EventHighway.Core.Models.Coordinations.HealthChecks.V2.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.Core.Tests.Unit.Clients.HealthChecks.V2
{
    public partial class HealthRetryClientV2Tests
    {
        [Theory]
        [MemberData(nameof(ValidationExceptions))]
        public async Task ShouldThrowValidationExceptionOnRetrieveRetryHealthIfValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var expectedHealthRetryClientV2ValidationException =
                new HealthRetryClientV2ValidationException(
                    message: "Health client validation error occurred, fix the errors and try again.",
                    innerException: validationException.InnerException as Xeption,
                    data: (validationException.InnerException as Xeption).Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveRetryHealthV2Async(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(validationException);

            // when
            ValueTask<RetryHealthSummaryV2> retrieveTask =
                this.healthRetryClientV2.RetrieveRetryHealthV2Async(randomCancellationToken);

            HealthRetryClientV2ValidationException actualException =
                await Assert.ThrowsAsync<HealthRetryClientV2ValidationException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthRetryClientV2ValidationException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveRetryHealthV2Async(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveRetryHealthIfDependencyErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.Data.Add("ErrorCode", new List<string> { "DependencyError" });

            var healthV2CoordinationDependencyException =
                new HealthV2CoordinationDependencyException(
                    someMessage,
                    someInnerException);

            var expectedHealthRetryClientV2DependencyException =
                new HealthRetryClientV2DependencyException(
                    message: "Health client dependency error occurred, contact support.",
                    innerException: healthV2CoordinationDependencyException.InnerException as Xeption,
                    data: (healthV2CoordinationDependencyException.InnerException as Xeption).Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveRetryHealthV2Async(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(healthV2CoordinationDependencyException);

            // when
            ValueTask<RetryHealthSummaryV2> retrieveTask =
                this.healthRetryClientV2.RetrieveRetryHealthV2Async(randomCancellationToken);

            HealthRetryClientV2DependencyException actualException =
                await Assert.ThrowsAsync<HealthRetryClientV2DependencyException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthRetryClientV2DependencyException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveRetryHealthV2Async(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRetrieveRetryHealthIfServiceErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            string someMessage = GetRandomString();
            var someInnerException = new Xeption(someMessage);
            someInnerException.Data.Add("ErrorCode", new List<string> { "ServiceError" });

            var healthV2CoordinationServiceException =
                new HealthV2CoordinationServiceException(
                    someMessage,
                    someInnerException);

            var expectedHealthRetryClientV2DependencyException =
                new HealthRetryClientV2DependencyException(
                    message: "Health client dependency error occurred, contact support.",
                    innerException: healthV2CoordinationServiceException.InnerException as Xeption,
                    data: (healthV2CoordinationServiceException.InnerException as Xeption).Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveRetryHealthV2Async(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(healthV2CoordinationServiceException);

            // when
            ValueTask<RetryHealthSummaryV2> retrieveTask =
                this.healthRetryClientV2.RetrieveRetryHealthV2Async(randomCancellationToken);

            HealthRetryClientV2DependencyException actualException =
                await Assert.ThrowsAsync<HealthRetryClientV2DependencyException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthRetryClientV2DependencyException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveRetryHealthV2Async(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveRetryHealthIfUnexpectedErrorOccursAsync()
        {
            // given
            CancellationToken randomCancellationToken =
                TestContext.Current.CancellationToken;

            var someXeption = new Xeption(message: GetRandomString());

            var expectedHealthRetryClientV2ServiceException =
                new HealthRetryClientV2ServiceException(
                    message: "Health client service error occurred, contact support.",
                    innerException: someXeption,
                    data: someXeption.Data);

            this.healthV2CoordinationServiceMock.Setup(service =>
                service.RetrieveRetryHealthV2Async(It.IsAny<CancellationToken>()))
                    .ThrowsAsync(someXeption);

            // when
            ValueTask<RetryHealthSummaryV2> retrieveTask =
                this.healthRetryClientV2.RetrieveRetryHealthV2Async(randomCancellationToken);

            HealthRetryClientV2ServiceException actualException =
                await Assert.ThrowsAsync<HealthRetryClientV2ServiceException>(
                    retrieveTask.AsTask);

            // then
            actualException.Should()
                .BeEquivalentTo(expectedHealthRetryClientV2ServiceException);

            this.healthV2CoordinationServiceMock.Verify(service =>
                service.RetrieveRetryHealthV2Async(It.IsAny<CancellationToken>()),
                    Times.Once);

            this.healthV2CoordinationServiceMock.VerifyNoOtherCalls();
        }
    }
}
