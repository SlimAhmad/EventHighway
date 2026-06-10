// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using FluentAssertions;
using Force.DeepCloner;
using Moq;

namespace EventHighway.EventHandlers.Tests.Unit.Exposers.RestSecretEventHandlers
{
    public partial class RestSecretEventHandlerTests
    {
        [Fact]
        public async Task ShouldHandleWithSecretAsync()
        {
            // given
            string randomContent = GetRandomString();
            var randomHandlerParams = CreateRandomHandlerParams();

            EventHandlerResult randomEventHandlerResult =
                CreateRandomEventHandlerResult();

            EventHandlerResult expectedEventHandlerResult =
                randomEventHandlerResult.DeepClone();

            this.restServiceMock.Setup(service =>
                service.PostWithSecretAsync(
                    randomContent,
                    randomHandlerParams,
                    TestContext.Current.CancellationToken))
                        .ReturnsAsync(randomEventHandlerResult);

            // when
            EventHandlerResult actualEventHandlerResult =
                await this.restSecretEventHandler.HandleAsync(
                    content: randomContent,
                    handlerParams: randomHandlerParams,
                    cancellationToken: TestContext.Current.CancellationToken);

            // then
            actualEventHandlerResult.Should().BeEquivalentTo(expectedEventHandlerResult);

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
