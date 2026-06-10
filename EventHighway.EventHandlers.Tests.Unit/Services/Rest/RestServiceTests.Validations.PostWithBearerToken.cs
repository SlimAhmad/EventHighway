// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.EventHandlers.Models.Foundations.Rest.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.EventHandlers.Tests.Unit.Services.Rest
{
    public partial class RestServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnPostWithBearerTokenIfHandlerParamsIsNullAndLogItAsync()
        {
            // given
            string randomContent = GetRandomString();

            var invalidRestServiceException =
                new InvalidRestServiceException(
                    message: "Rest service params are invalid, fix the errors and try again.");

            invalidRestServiceException.AddData(
                key: "handlerParams",
                values: "Params required");

            var expectedRestServiceValidationException =
                new RestServiceValidationException(
                    message: "Rest service validation error occurred, fix the errors and try again.",
                    innerException: invalidRestServiceException);

            // when
            ValueTask<EventHandlerResult> postWithBearerTokenTask =
                this.restService.PostWithBearerTokenAsync(
                    content: randomContent,
                    handlerParams: null,
                    cancellationToken: TestContext.Current.CancellationToken);

            RestServiceValidationException actualRestServiceValidationException =
                await Assert.ThrowsAsync<RestServiceValidationException>(
                    postWithBearerTokenTask.AsTask);

            // then
            actualRestServiceValidationException.Should().BeEquivalentTo(
                expectedRestServiceValidationException);

            this.apiBrokerMock.Verify(broker =>
                broker.PostWithBearerTokenAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                        Times.Never);

            this.apiBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnPostWithBearerTokenIfContentIsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            var someHandlerParams = CreateBearerTokenHandlerParams(
                url: GetRandomString(),
                clientId: GetRandomString(),
                clientSecret: GetRandomString(),
                scope: GetRandomString(),
                grantType: GetRandomString(),
                tokenUrl: GetRandomString());

            var invalidRestServiceException =
                new InvalidRestServiceException(
                    message: "Rest service params are invalid, fix the errors and try again.");

            invalidRestServiceException.AddData(
                key: "content",
                values: "Text required");

            var expectedRestServiceValidationException =
                new RestServiceValidationException(
                    message: "Rest service validation error occurred, fix the errors and try again.",
                    innerException: invalidRestServiceException);

            // when
            ValueTask<EventHandlerResult> postWithBearerTokenTask =
                this.restService.PostWithBearerTokenAsync(
                    content: invalidText,
                    handlerParams: someHandlerParams,
                    cancellationToken: TestContext.Current.CancellationToken);

            RestServiceValidationException actualRestServiceValidationException =
                await Assert.ThrowsAsync<RestServiceValidationException>(
                    postWithBearerTokenTask.AsTask);

            // then
            actualRestServiceValidationException.Should().BeEquivalentTo(
                expectedRestServiceValidationException);

            this.apiBrokerMock.Verify(broker =>
                broker.PostWithBearerTokenAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                        Times.Never);

            this.apiBrokerMock.VerifyNoOtherCalls();
        }
    }
}
