// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
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

        [Theory]
        [MemberData(nameof(RestBearerDependencyExceptions))]
        public async Task ShouldThrowDependencyExceptionOnHandleIfDependencyErrorOccursAsync(
            Xeption dependencyException)
        {
            // given
            string randomContent = GetRandomString();
            var randomHandlerParams = CreateRandomHandlerParams();

            var expectedRestBearerEventHandlerDependencyException =
                new RestBearerEventHandlerDependencyException(
                    message: "Rest bearer event handler dependency error occurred, contact support.",
                    innerException: dependencyException.InnerException as Xeption,
                    data: (dependencyException.InnerException as Xeption).Data);

            this.restServiceMock.Setup(service =>
                service.PostWithBearerTokenAsync(
                    randomContent,
                    randomHandlerParams,
                    TestContext.Current.CancellationToken))
                        .ThrowsAsync(dependencyException);

            // when
            ValueTask<EventHandlerResult> handleTask =
                this.restBearerEventHandler.HandleAsync(
                    content: randomContent,
                    handlerParams: randomHandlerParams,
                    cancellationToken: TestContext.Current.CancellationToken);

            RestBearerEventHandlerDependencyException
                actualRestBearerEventHandlerDependencyException =
                    await Assert.ThrowsAsync<RestBearerEventHandlerDependencyException>(
                        handleTask.AsTask);

            // then
            actualRestBearerEventHandlerDependencyException.Should()
                .BeEquivalentTo(expectedRestBearerEventHandlerDependencyException);

            this.restServiceMock.Verify(service =>
                service.PostWithBearerTokenAsync(
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

            var expectedRestBearerEventHandlerServiceException =
                new RestBearerEventHandlerServiceException(
                    message: "Rest bearer event handler service error occurred, contact support.",
                    innerException: someInnerXeption,
                    data: someInnerXeption.Data);

            this.restServiceMock.Setup(service =>
                service.PostWithBearerTokenAsync(
                    randomContent,
                    randomHandlerParams,
                    TestContext.Current.CancellationToken))
                        .ThrowsAsync(serviceException);

            // when
            ValueTask<EventHandlerResult> handleTask =
                this.restBearerEventHandler.HandleAsync(
                    content: randomContent,
                    handlerParams: randomHandlerParams,
                    cancellationToken: TestContext.Current.CancellationToken);

            RestBearerEventHandlerServiceException
                actualRestBearerEventHandlerServiceException =
                    await Assert.ThrowsAsync<RestBearerEventHandlerServiceException>(
                        handleTask.AsTask);

            // then
            actualRestBearerEventHandlerServiceException.Should()
                .BeEquivalentTo(expectedRestBearerEventHandlerServiceException);

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
