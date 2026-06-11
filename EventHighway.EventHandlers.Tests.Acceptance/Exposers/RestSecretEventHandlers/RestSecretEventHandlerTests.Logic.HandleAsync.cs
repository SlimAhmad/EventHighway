// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using FluentAssertions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace EventHighway.EventHandlers.Tests.Acceptance.Exposers.RestSecretEventHandlers
{
    public partial class RestSecretEventHandlerTests
    {
        [Fact]
        public async Task ShouldHandleWithSecretAsync()
        {
            // given
            string randomContent = GetRandomString();
            string randomSecret = GetRandomString();
            string randomResponseBody = GetRandomString();
            string inputUrl = this.wireMockServer.Url;

            var inputHandlerParams =
                CreateHandlerParams(url: inputUrl, secret: randomSecret);

            EventHandlerResult expectedEventHandlerResult =
                CreateExpectedEventHandlerResult(randomResponseBody);

            this.wireMockServer
                .Given(
                    Request.Create()
                        .UsingPost()
                        .WithHeader("X-Highway", randomSecret))
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithBody(randomResponseBody));

            // when
            EventHandlerResult actualEventHandlerResult =
                await this.restSecretEventHandler.HandleAsync(
                    content: randomContent,
                    handlerParams: inputHandlerParams,
                    cancellationToken: TestContext.Current.CancellationToken);

            // then
            actualEventHandlerResult.Should().BeEquivalentTo(expectedEventHandlerResult);
        }
    }
}
