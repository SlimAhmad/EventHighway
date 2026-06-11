// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.EventHandlers.Tests.Unit.Exposers.RestBearerEventHandlers
{
    public partial class RestBearerEventHandlerTests
    {
        [Fact]
        public async Task ShouldHandleWithBearerTokenAsync()
        {
            // given
            string randomContent = GetRandomString();
            var randomHandlerParams = CreateRandomHandlerParams();

            EventHandlerResult randomEventHandlerResult =
                CreateRandomEventHandlerResult();

            EventHandlerResult expectedEventHandlerResult =
                randomEventHandlerResult.DeepClone();

            this.restServiceMock.Setup(service =>
                service.PostWithBearerTokenAsync(
                    randomContent,
                    randomHandlerParams,
                    TestContext.Current.CancellationToken))
                        .ReturnsAsync(randomEventHandlerResult);

            // when
            EventHandlerResult actualEventHandlerResult =
                await this.restBearerEventHandler.HandleAsync(
                    content: randomContent,
                    handlerParams: randomHandlerParams,
                    cancellationToken: TestContext.Current.CancellationToken);

            // then
            actualEventHandlerResult.Should().BeEquivalentTo(expectedEventHandlerResult);

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
