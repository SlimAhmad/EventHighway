// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using FluentAssertions;
using Moq;

namespace EventHighway.EventHandlers.Tests.Unit.Services.Delegates
{
    public partial class DelegateServiceTests
    {
        [Fact]
        public async Task ShouldInvokeAsync()
        {
            // given
            string randomContent = GetRandomString();
            IReadOnlyDictionary<string, string> inputHandlerParams = CreateHandlerParams();
            EventHandlerResult randomResult = CreateRandomEventHandlerResult();
            EventHandlerResult expectedResult = randomResult;

            this.handlerFuncMock
                .Setup(handler => handler(
                    randomContent,
                    inputHandlerParams,
                    It.IsAny<CancellationToken>()))
                .Returns(new ValueTask<EventHandlerResult>(randomResult));

            // when
            EventHandlerResult actualResult =
                await this.delegateService.InvokeAsync(
                    content: randomContent,
                    handlerParams: inputHandlerParams,
                    cancellationToken: TestContext.Current.CancellationToken);

            // then
            actualResult.Should().BeEquivalentTo(expectedResult);

            this.handlerFuncMock.Verify(handler =>
                handler(
                    randomContent,
                    inputHandlerParams,
                    It.IsAny<CancellationToken>()),
                Times.Once);

            this.handlerFuncMock.VerifyNoOtherCalls();
        }
    }
}
