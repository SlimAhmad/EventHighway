// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.EventHandlers.Models.Foundations.Delegates.Exceptions;
using FluentAssertions;
using Moq;

namespace EventHighway.EventHandlers.Tests.Unit.Services.Delegates
{
    public partial class DelegateServiceTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnInvokeIfContentIsInvalidAndLogItAsync(
            string invalidContent)
        {
            // given
            var inputHandlerParams = CreateHandlerParams();

            var invalidDelegateServiceException =
                new InvalidDelegateServiceException(
                    message: "Delegate service params are invalid, fix the errors and try again.");

            invalidDelegateServiceException.AddData(
                key: "content",
                values: "Text required");

            var expectedDelegateServiceValidationException =
                new DelegateServiceValidationException(
                    message: "Delegate service validation error occurred, fix the errors and try again.",
                    innerException: invalidDelegateServiceException);

            // when
            ValueTask<EventHandlerResult> invokeTask =
                this.delegateService.InvokeAsync(
                    content: invalidContent,
                    handlerParams: inputHandlerParams,
                    cancellationToken: TestContext.Current.CancellationToken);

            DelegateServiceValidationException actualDelegateServiceValidationException =
                await Assert.ThrowsAsync<DelegateServiceValidationException>(
                    invokeTask.AsTask);

            // then
            actualDelegateServiceValidationException.Should()
                .BeEquivalentTo(expectedDelegateServiceValidationException);

            this.handlerFuncMock.Verify(handler =>
                handler(
                    It.IsAny<string>(),
                    It.IsAny<System.Collections.Generic.IReadOnlyDictionary<string, string>>(),
                    It.IsAny<System.Threading.CancellationToken>()),
                Times.Never);

            this.handlerFuncMock.VerifyNoOtherCalls();
        }
    }
}
