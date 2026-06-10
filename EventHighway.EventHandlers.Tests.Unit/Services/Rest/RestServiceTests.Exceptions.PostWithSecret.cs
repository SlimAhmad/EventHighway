// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using EventHighway.EventHandlers.Models.Foundations.Rest.Exceptions;
using FluentAssertions;
using Moq;
using Xeptions;

namespace EventHighway.EventHandlers.Tests.Unit.Services.Rest
{
    public partial class RestServiceTests
    {
        [Theory]
        [MemberData(nameof(CriticalDependencyExceptions))]
        public async Task
            ShouldThrowCriticalDependencyExceptionOnPostWithSecretIfCriticalDependencyErrorOccursAndLogItAsync(
                Xeption criticalDependencyException)
        {
            // given
            string randomContent = GetRandomString();

            var someHandlerParams = CreateSecretHandlerParams(
                url: GetRandomString(),
                secret: GetRandomString());

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
                broker.PostWithSecretAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                        .ThrowsAsync(criticalDependencyException);

            // when
            ValueTask<EventHandlerResult> postWithSecretTask =
                this.restService.PostWithSecretAsync(
                    content: randomContent,
                    handlerParams: someHandlerParams,
                    cancellationToken: TestContext.Current.CancellationToken);

            RestServiceDependencyException actualRestServiceDependencyException =
                await Assert.ThrowsAsync<RestServiceDependencyException>(
                    postWithSecretTask.AsTask);

            // then
            actualRestServiceDependencyException.Should()
                .BeEquivalentTo(expectedRestServiceDependencyException);

            this.apiBrokerMock.Verify(broker =>
                broker.PostWithSecretAsync(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                        Times.Once);

            this.apiBrokerMock.VerifyNoOtherCalls();
        }
    }
}
