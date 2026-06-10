// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Net.Http;
using System.Threading.Tasks;

namespace EventHighway.EventHandlers.Brokers.Apis
{
    internal interface IApiBroker
    {
        ValueTask<HttpResponseMessage> PostWithSecretAsync(
            string content,
            string url,
            string secret);

        ValueTask<HttpResponseMessage> PostWithBearerTokenAsync(
            string content,
            string url,
            string clientId,
            string clientSecret,
            string scope,
            string grantType,
            string tokenUrl);
    }
}
