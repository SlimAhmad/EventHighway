// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EventHighway.EventHandlers.Brokers.Apis
{
    internal class ApiBroker : IApiBroker
    {
        public async ValueTask<HttpResponseMessage> PostWithSecretAsync(
            string content,
            string url,
            string secret)
        {
            var httpClient = new HttpClient();

            var stringContent =
               new StringContent(
                   content,
                   encoding: Encoding.UTF8,
                   mediaType: "application/json");

            httpClient.DefaultRequestHeaders.Add(
                name: "X-Highway",
                value: secret);

            HttpResponseMessage httpResponseMessage =
                await httpClient.PostAsync(
                    requestUri: url,
                    content: stringContent);

            return httpResponseMessage;
        }

        public ValueTask<HttpResponseMessage> PostWithBearerTokenAsync(
            string content,
            string url,
            string clientId,
            string clientSecret,
            string scope,
            string grantType,
            string tokenUrl)
        {
            throw new System.NotImplementedException();
        }
    }
}