// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.EventHandlers.Models.Exposers.RestSecretEventHandlers.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.EventHandlers.Tests.Unit.Exposers.RestSecretEventHandlers
{
    public partial class RestSecretEventHandlerTests
    {
        [Theory]
        [MemberData(nameof(RestSecretValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnHandleIfDependencyValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            string randomContent = GetRandomString();
            var randomHandlerParams = CreateRandomHandlerParams();

            var expectedRestSecretEventHandlerValidationException =
                new RestSecretEventHandlerValidationException(
                    message: "Rest secret event handler validation error occurred, fix errors and try again.",
                    innerException: validationException.InnerException as Xeption,
                    data: (validationException.InnerException as Xeption).Data);

            this.restServiceMock.Setup(service =>
                service.PostWithSecretAsync(
                    randomContent,
                    randomHandlerParams,
                    TestContext.Current.CancellationToken))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<EventHandlerResult> handleTask =
                this.restSecretEventHandler.HandleAsync(
                    content: randomContent,
                    handlerParams: randomHandlerParams,
                    cancellationToken: TestContext.Current.CancellationToken);

            RestSecretEventHandlerValidationException
                actualRestSecretEventHandlerValidationException =
                    await Assert.ThrowsAsync<RestSecretEventHandlerValidationException>(
                        handleTask.AsTask);

            // then
            actualRestSecretEventHandlerValidationException.Should()
                .BeEquivalentTo(expectedRestSecretEventHandlerValidationException);

            this.restServiceMock.Verify(service =>
                service.PostWithSecretAsync(
                    randomContent,
                    randomHandlerParams,
                    TestContext.Current.CancellationToken),
                        Times.Once);

            this.restServiceMock.VerifyNoOtherCalls();
        }
    }
}
