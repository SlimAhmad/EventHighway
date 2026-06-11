// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using EventHighway.Abstractions.EventHandlers;
using FluentAssertions;
using Moq;

namespace EventHighway.EventHandlers.Tests.Unit.Services.Rest
{
    public partial class RestServiceTests
    {
        [Fact]
        public async Task ShouldPostWithBearerTokenAsync()
        {
            // given
            string randomContent = GetRandomString();
            string randomUrl = GetRandomString();
            string randomClientId = GetRandomString();
            string randomClientSecret = GetRandomString();
            string randomScope = GetRandomString();
            string randomGrantType = GetRandomString();
            string randomTokenUrl = GetRandomString();
            string randomResponseBody = GetRandomString();

            string inputContent = randomContent;

            var inputHandlerParams =
                CreateBearerTokenHandlerParams(
                    url: randomUrl,
                    clientId: randomClientId,
                    clientSecret: randomClientSecret,
                    scope: randomScope,
                    grantType: randomGrantType,
                    tokenUrl: randomTokenUrl);

            var returnedHttpResponseMessage =
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(randomResponseBody)
                };

            EventHandlerResult expectedResult =
                new EventHandlerResult
                {
                    Response = randomResponseBody,
                    ResponseCode = "200",
                    ResponseMessage = "OK",
                    IsSuccess = true
                };

            this.apiBrokerMock.Setup(broker =>
                broker.PostWithBearerTokenAsync(
                    inputContent,
                    randomUrl,
                    randomClientId,
                    randomClientSecret,
                    randomScope,
                    randomGrantType,
                    randomTokenUrl))
                        .ReturnsAsync(returnedHttpResponseMessage);

            // when
            EventHandlerResult actualResult =
                await this.restService.PostWithBearerTokenAsync(
                    inputContent,
                    inputHandlerParams,
                    TestContext.Current.CancellationToken);

            // then
            actualResult.Should().BeEquivalentTo(expectedResult);

            this.apiBrokerMock.Verify(broker =>
                broker.PostWithBearerTokenAsync(
                    inputContent,
                    randomUrl,
                    randomClientId,
                    randomClientSecret,
                    randomScope,
                    randomGrantType,
                    randomTokenUrl),
                        Times.Once);

            this.apiBrokerMock.VerifyNoOtherCalls();
        }
    }
}
