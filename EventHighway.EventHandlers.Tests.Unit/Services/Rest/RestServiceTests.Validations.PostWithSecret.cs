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
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnPostWithSecretIfHandlerParamConfigIsInvalidAndLogItAsync(
            string invalidValue)
        {
            // given
            string randomContent = GetRandomString();

            var inputHandlerParams = invalidValue is null
                ? new Dictionary<string, string>()
                : new Dictionary<string, string>
                {
                    { "url", invalidValue },
                    { "secret", invalidValue }
                };

            var invalidRestServiceException =
                new InvalidRestServiceException(
                    message: "Rest service params are invalid, fix the errors and try again.");

            invalidRestServiceException.AddData(
                key: "url",
                values: invalidValue is null ? "Config item required" : "Value required");

            invalidRestServiceException.AddData(
                key: "secret",
                values: invalidValue is null ? "Config item required" : "Value required");

            var expectedRestServiceValidationException =
                new RestServiceValidationException(
                    message: "Rest service validation error occurred, fix the errors and try again.",
                    innerException: invalidRestServiceException);

            // when
            ValueTask<EventHandlerResult> postWithSecretTask =
                this.restService.PostWithSecretAsync(
                    content: randomContent,
                    handlerParams: inputHandlerParams,
                    cancellationToken: TestContext.Current.CancellationToken);

            RestServiceValidationException actualRestServiceValidationException =
                await Assert.ThrowsAsync<RestServiceValidationException>(
                    postWithSecretTask.AsTask);

            // then
            actualRestServiceValidationException.Should().BeEquivalentTo(
                expectedRestServiceValidationException);

            this.apiBrokerMock.Verify(broker =>
                broker.PostWithSecretAsync(
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
        public async Task ShouldThrowValidationExceptionOnPostWithSecretIfContentIsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            var someHandlerParams = CreateSecretHandlerParams(
                url: GetRandomString(),
                secret: GetRandomString());

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
            ValueTask<EventHandlerResult> postWithSecretTask =
                this.restService.PostWithSecretAsync(
                    content: invalidText,
                    handlerParams: someHandlerParams,
                    cancellationToken: TestContext.Current.CancellationToken);

            RestServiceValidationException actualRestServiceValidationException =
                await Assert.ThrowsAsync<RestServiceValidationException>(
                    postWithSecretTask.AsTask);

            // then
            actualRestServiceValidationException.Should().BeEquivalentTo(
                expectedRestServiceValidationException);

            this.apiBrokerMock.Verify(broker =>
                broker.PostWithSecretAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                        Times.Never);

            this.apiBrokerMock.VerifyNoOtherCalls();
        }
    }
}
