// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using FluentAssertions;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;

namespace EventHighway.EventHandlers.Tests.Acceptance.Exposers.RestBearerEventHandlers
{
    public partial class RestBearerEventHandlerTests
    {
        [Fact]
        public async Task ShouldHandleWithBearerTokenAsync()
        {
            // given
            string randomContent = GetRandomString();
            string randomAccessToken = GetRandomString();
            string randomResponseBody = GetRandomString();
            string inputClientId = GetRandomString();
            string inputClientSecret = GetRandomString();
            string inputScope = GetRandomString();
            string inputGrantType = GetRandomString();
            string inputTokenPath = "/token";
            string inputTargetPath = "/target";

            string inputTokenUrl = $"{this.wireMockServer.Url}{inputTokenPath}";
            string inputTargetUrl = $"{this.wireMockServer.Url}{inputTargetPath}";

            var inputHandlerParams = CreateHandlerParams(
                url: inputTargetUrl,
                tokenUrl: inputTokenUrl,
                clientId: inputClientId,
                clientSecret: inputClientSecret,
                scope: inputScope,
                grantType: inputGrantType);

            EventHandlerResult expectedEventHandlerResult =
                CreateExpectedEventHandlerResult(randomResponseBody);

            this.wireMockServer
                .Given(
                    Request.Create()
                        .UsingPost()
                        .WithPath(inputTokenPath))
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithBodyAsJson(new
                        {
                            access_token = randomAccessToken,
                            expires_in = 3600
                        }));

            this.wireMockServer
                .Given(
                    Request.Create()
                        .UsingPost()
                        .WithPath(inputTargetPath)
                        .WithHeader("Authorization", $"Bearer {randomAccessToken}"))
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithBody(randomResponseBody));

            // when
            EventHandlerResult actualEventHandlerResult =
                await this.restBearerEventHandler.HandleAsync(
                    content: randomContent,
                    handlerParams: inputHandlerParams,
                    cancellationToken: TestContext.Current.CancellationToken);

            // then
            actualEventHandlerResult.Should().BeEquivalentTo(expectedEventHandlerResult);
        }
    }
}
