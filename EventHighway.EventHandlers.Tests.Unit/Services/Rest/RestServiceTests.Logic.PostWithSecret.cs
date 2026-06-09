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
        public async Task ShouldPostWithSecretAsync()
        {
            // given
            string randomContent = GetRandomString();
            string randomUrl = GetRandomString();
            string randomSecret = GetRandomString();

            string inputContent = randomContent;

            var inputHandlerParams =
                CreateSecretHandlerParams(
                    url: randomUrl,
                    secret: randomSecret);

            var returnedHttpResponseMessage =
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(GetRandomString())
                };

            EventHandlerResult expectedResult =
                CreateRandomEventHandlerResult();

            this.apiBrokerMock.Setup(broker =>
                broker.PostWithSecretAsync(
                    inputContent,
                    randomUrl,
                    randomSecret))
                        .ReturnsAsync(returnedHttpResponseMessage);

            // when
            EventHandlerResult actualResult =
                await this.restService.PostWithSecretAsync(
                    inputContent,
                    inputHandlerParams,
                    TestContext.Current.CancellationToken);

            // then
            actualResult.Should().BeEquivalentTo(expectedResult);

            this.apiBrokerMock.Verify(broker =>
                broker.PostWithSecretAsync(
                    inputContent,
                    randomUrl,
                    randomSecret),
                        Times.Once);

            this.apiBrokerMock.VerifyNoOtherCalls();
        }
    }
}
