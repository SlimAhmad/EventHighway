// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
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

        [Theory]
        [MemberData(nameof(RestSecretDependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnHandleIfDependencyErrorOccursAsync(
            Xeption dependencyException)
        {
            // given
            string randomContent = GetRandomString();
            var randomHandlerParams = CreateRandomHandlerParams();

            var expectedRestSecretEventHandlerDependencyException =
                new RestSecretEventHandlerDependencyException(
                    message: "Rest secret event handler dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption,
                    data: (dependencyException.InnerException as Xeption).Data);

            this.restServiceMock.Setup(service =>
                service.PostWithSecretAsync(
                    randomContent,
                    randomHandlerParams,
                    TestContext.Current.CancellationToken))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<EventHandlerResult> handleTask =
                this.restSecretEventHandler.HandleAsync(
                    content: randomContent,
                    handlerParams: randomHandlerParams,
                    cancellationToken: TestContext.Current.CancellationToken);

            RestSecretEventHandlerDependencyException
                actualRestSecretEventHandlerDependencyException =
                    await Assert.ThrowsAsync<RestSecretEventHandlerDependencyException>(
                        handleTask.AsTask);

            // then
            actualRestSecretEventHandlerDependencyException.Should()
                .BeEquivalentTo(expectedRestSecretEventHandlerDependencyException);

            this.restServiceMock.Verify(service =>
                service.PostWithSecretAsync(
                    randomContent,
                    randomHandlerParams,
                    TestContext.Current.CancellationToken),
                        Times.Once);

            this.restServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnHandleIfExceptionOccursAsync()
        {
            // given
            string randomContent = GetRandomString();
            var randomHandlerParams = CreateRandomHandlerParams();

            var someInnerXeption = new Xeption(message: GetRandomString());
            someInnerXeption.Data.Add("ErrorCode", new List<string> { "ServiceError" });
            var serviceException = new Exception(GetRandomString(), someInnerXeption);

            var expectedRestSecretEventHandlerServiceException =
                new RestSecretEventHandlerServiceException(
                    message: "Rest secret event handler service error occurred, contact support.",
                    innerException: someInnerXeption,
                    data: someInnerXeption.Data);

            this.restServiceMock.Setup(service =>
                service.PostWithSecretAsync(
                    randomContent,
                    randomHandlerParams,
                    TestContext.Current.CancellationToken))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<EventHandlerResult> handleTask =
                this.restSecretEventHandler.HandleAsync(
                    content: randomContent,
                    handlerParams: randomHandlerParams,
                    cancellationToken: TestContext.Current.CancellationToken);

            RestSecretEventHandlerServiceException
                actualRestSecretEventHandlerServiceException =
                    await Assert.ThrowsAsync<RestSecretEventHandlerServiceException>(
                        handleTask.AsTask);

            // then
            actualRestSecretEventHandlerServiceException.Should()
                .BeEquivalentTo(expectedRestSecretEventHandlerServiceException);

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
