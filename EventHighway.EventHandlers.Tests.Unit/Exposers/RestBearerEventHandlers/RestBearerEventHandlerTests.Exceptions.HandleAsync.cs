// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.EventHandlers.Models.Exposers.RestBearerEventHandlers.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.EventHandlers.Tests.Unit.Exposers.RestBearerEventHandlers
{
    public partial class RestBearerEventHandlerTests
    {
        [Theory]
        [MemberData(nameof(RestBearerValidationExceptions))]
        public async Task ShouldThrowDependencyValidationExceptionOnHandleIfDependencyValidationErrorOccursAsync(
            Xeption validationException)
        {
            // given
            string randomContent = GetRandomString();
            var randomHandlerParams = CreateRandomHandlerParams();

            var expectedRestBearerEventHandlerValidationException =
                new RestBearerEventHandlerValidationException(
                    message: "Rest bearer event handler validation error occurred, fix errors and try again.",
                    innerException: validationException.InnerException as Xeption,
                    data: (validationException.InnerException as Xeption).Data);

            this.restServiceMock.Setup(service =>
                service.PostWithBearerTokenAsync(
                    randomContent,
                    randomHandlerParams,
                    TestContext.Current.CancellationToken))
                        .ThrowsAsync(validationException);

            // when
            ValueTask<EventHandlerResult> handleTask =
                this.restBearerEventHandler.HandleAsync(
                    content: randomContent,
                    handlerParams: randomHandlerParams,
                    cancellationToken: TestContext.Current.CancellationToken);

            RestBearerEventHandlerValidationException
                actualRestBearerEventHandlerValidationException =
                    await Assert.ThrowsAsync<RestBearerEventHandlerValidationException>(
                        handleTask.AsTask);

            // then
            actualRestBearerEventHandlerValidationException.Should()
                .BeEquivalentTo(expectedRestBearerEventHandlerValidationException);

            this.restServiceMock.Verify(service =>
                service.PostWithBearerTokenAsync(
                    randomContent,
                    randomHandlerParams,
                    TestContext.Current.CancellationToken),
                        Times.Once);

            this.restServiceMock.VerifyNoOtherCalls();
        }
    }
}
