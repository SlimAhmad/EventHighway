// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.EventHandlers.Models.Foundations.Rest.Exceptions;
using FluentAssertions;
using Moq;
using RESTFulSense.Exceptions;
using Xeptions;

namespace EventHighway.EventHandlers.Tests.Unit.Services.Rest
{
    public partial class RestServiceTests
    {
        [Theory]
        [MemberData(nameof(CriticalDependencyExceptions))]
        public async Task
            ShouldThrowCriticalDependencyExceptionOnPostWithBearerTokenIfCriticalDependencyErrorOccursAndLogItAsync(
                Xeption criticalDependencyException)
        {
            // given
            string randomContent = GetRandomString();

            var someHandlerParams = CreateBearerTokenHandlerParams(
                url: GetRandomString(),
                clientId: GetRandomString(),
                clientSecret: GetRandomString(),
                scope: GetRandomString(),
                grantType: GetRandomString(),
                tokenUrl: GetRandomString());

            var failedRestServiceException =
                new FailedRestServiceException(
                    message: "Failed rest service error occurred, contact support.",
                    innerException: criticalDependencyException,
                    data: criticalDependencyException.Data);

            var expectedRestServiceDependencyException =
                new RestServiceDependencyException(
                    message: "Rest service dependency error occurred, contact support.",
                    innerException: failedRestServiceException);

            this.apiBrokerMock.Setup(broker =>
                broker.PostWithBearerTokenAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                        .ThrowsAsync(criticalDependencyException);

            // when
            ValueTask<EventHandlerResult> postWithBearerTokenTask =
                this.restService.PostWithBearerTokenAsync(
                    content: randomContent,
                    handlerParams: someHandlerParams,
                    cancellationToken: TestContext.Current.CancellationToken);

            RestServiceDependencyException actualRestServiceDependencyException =
                await Assert.ThrowsAsync<RestServiceDependencyException>(
                    postWithBearerTokenTask.AsTask);

            // then
            actualRestServiceDependencyException.Should()
                .BeEquivalentTo(expectedRestServiceDependencyException);

            this.apiBrokerMock.Verify(broker =>
                broker.PostWithBearerTokenAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                        Times.Once);

            this.apiBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnPostWithBearerTokenIfHttpUnprocessableErrorOccursAndLogItAsync()
        {
            // given
            string randomContent = GetRandomString();

            var someHandlerParams = CreateBearerTokenHandlerParams(
                url: GetRandomString(),
                clientId: GetRandomString(),
                clientSecret: GetRandomString(),
                scope: GetRandomString(),
                grantType: GetRandomString(),
                tokenUrl: GetRandomString());

            var httpUnprocessableEntityException =
                new HttpResponseUnprocessableEntityException();

            httpUnprocessableEntityException.Data.Add(
                "ErrorCode",
                new List<string> { "UnprocessableEntity" });

            var failedRequestRestServiceException =
                new FailedRequestRestServiceException(
                    message: "Failed rest service request error occurred, fix the errors and try again.",
                    innerException: httpUnprocessableEntityException,
                    data: httpUnprocessableEntityException.Data);

            var expectedRestServiceDependencyValidationException =
                new RestServiceDependencyValidationException(
                    message: "Rest service dependency validation error occurred, fix the errors and try again.",
                    innerException: failedRequestRestServiceException);

            this.apiBrokerMock.Setup(broker =>
                broker.PostWithBearerTokenAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                        .ThrowsAsync(httpUnprocessableEntityException);

            // when
            ValueTask<EventHandlerResult> postWithBearerTokenTask =
                this.restService.PostWithBearerTokenAsync(
                    content: randomContent,
                    handlerParams: someHandlerParams,
                    cancellationToken: TestContext.Current.CancellationToken);

            RestServiceDependencyValidationException actualRestServiceDependencyValidationException =
                await Assert.ThrowsAsync<RestServiceDependencyValidationException>(
                    postWithBearerTokenTask.AsTask);

            // then
            actualRestServiceDependencyValidationException.Should()
                .BeEquivalentTo(expectedRestServiceDependencyValidationException);

            this.apiBrokerMock.Verify(broker =>
                broker.PostWithBearerTokenAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                        Times.Once);

            this.apiBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnPostWithBearerTokenIfBadRequestErrorOccursAndLogItAsync()
        {
            // given
            string randomContent = GetRandomString();

            var someHandlerParams = CreateBearerTokenHandlerParams(
                url: GetRandomString(),
                clientId: GetRandomString(),
                clientSecret: GetRandomString(),
                scope: GetRandomString(),
                grantType: GetRandomString(),
                tokenUrl: GetRandomString());

            HttpResponseBadRequestException httpBadRequestException =
                CreateHttpBadRequestException();

            var invalidRestServiceException =
                new InvalidRestServiceException(
                    message: "Rest service request is invalid, fix the errors and try again.",
                    innerException: httpBadRequestException,
                    data: httpBadRequestException.Data);

            var expectedRestServiceDependencyValidationException =
                new RestServiceDependencyValidationException(
                    message: "Rest service dependency validation error occurred, fix the errors and try again.",
                    innerException: invalidRestServiceException);

            this.apiBrokerMock.Setup(broker =>
                broker.PostWithBearerTokenAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                        .ThrowsAsync(httpBadRequestException);

            // when
            ValueTask<EventHandlerResult> postWithBearerTokenTask =
                this.restService.PostWithBearerTokenAsync(
                    content: randomContent,
                    handlerParams: someHandlerParams,
                    cancellationToken: TestContext.Current.CancellationToken);

            RestServiceDependencyValidationException actualRestServiceDependencyValidationException =
                await Assert.ThrowsAsync<RestServiceDependencyValidationException>(
                    postWithBearerTokenTask.AsTask);

            // then
            actualRestServiceDependencyValidationException.Should()
                .BeEquivalentTo(expectedRestServiceDependencyValidationException);

            this.apiBrokerMock.Verify(broker =>
                broker.PostWithBearerTokenAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                        Times.Once);

            this.apiBrokerMock.VerifyNoOtherCalls();
        }
    }
}
