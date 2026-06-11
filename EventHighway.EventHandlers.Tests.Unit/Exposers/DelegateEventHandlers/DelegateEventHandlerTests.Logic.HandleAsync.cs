// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.EventHandlers.Tests.Unit.Exposers.DelegateEventHandlers
{
    public partial class DelegateEventHandlerTests
    {
        [Fact]
        public async Task ShouldHandleAsync()
        {
            // given
            string randomContent = GetRandomString();
            var randomHandlerParams = CreateRandomHandlerParams();

            EventHandlerResult randomEventHandlerResult =
                CreateRandomEventHandlerResult();

            EventHandlerResult expectedEventHandlerResult =
                randomEventHandlerResult.DeepClone();

            this.delegateServiceMock.Setup(service =>
                service.InvokeAsync(
                    randomContent,
                    randomHandlerParams,
                    TestContext.Current.CancellationToken))
                        .ReturnsAsync(randomEventHandlerResult);

            // when
            EventHandlerResult actualEventHandlerResult =
                await this.delegateEventHandler.HandleAsync(
                    content: randomContent,
                    handlerParams: randomHandlerParams,
                    cancellationToken: TestContext.Current.CancellationToken);

            // then
            actualEventHandlerResult.Should().BeEquivalentTo(expectedEventHandlerResult);

            this.delegateServiceMock.Verify(service =>
                service.InvokeAsync(
                    randomContent,
                    randomHandlerParams,
                    TestContext.Current.CancellationToken),
                        Times.Once);

            this.delegateServiceMock.VerifyNoOtherCalls();
        }
    }
}
