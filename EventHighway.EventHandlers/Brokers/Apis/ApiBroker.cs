// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using EventHighway.EventHandlers.Models.Brokers.Apis;

namespace EventHighway.EventHandlers.Brokers.Apis
{
    internal class ApiBroker : IApiBroker
    {
        private readonly HttpClient tokenHttpClient = new HttpClient();
        private readonly SemaphoreSlim tokenGate = new SemaphoreSlim(1, 1);
        private string accessToken = string.Empty;
        private DateTimeOffset tokenExpiry = DateTimeOffset.MinValue;

        private static readonly JsonSerializerOptions jsonOptions =
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

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

        public async ValueTask<HttpResponseMessage> PostWithBearerTokenAsync(
            string content,
            string url,
            string clientId,
            string clientSecret,
            string scope,
            string grantType,
            string tokenUrl)
        {
            await EnsureAccessTokenAsync(
                clientId,
                clientSecret,
                scope,
                grantType,
                tokenUrl,
                CancellationToken.None)
                    .ConfigureAwait(false);

            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", this.accessToken);

            var stringContent =
                new StringContent(
                    content,
                    encoding: Encoding.UTF8,
                    mediaType: "application/json");

            return await httpClient.PostAsync(
                requestUri: url,
                content: stringContent)
                    .ConfigureAwait(false);
        }

        private async ValueTask EnsureAccessTokenAsync(
            string clientId,
            string clientSecret,
            string scope,
            string grantType,
            string tokenUrl,
            CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(this.accessToken)
                && DateTimeOffset.UtcNow < this.tokenExpiry)
            {
                return;
            }

            await this.tokenGate.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                if (string.IsNullOrEmpty(this.accessToken)
                    || DateTimeOffset.UtcNow >= this.tokenExpiry)
                {
                    await FetchAccessTokenAsync(
                        clientId,
                        clientSecret,
                        scope,
                        grantType,
                        tokenUrl,
                        cancellationToken)
                            .ConfigureAwait(false);
                }
            }
            finally
            {
                this.tokenGate.Release();
            }
        }

        private async ValueTask FetchAccessTokenAsync(
            string clientId,
            string clientSecret,
            string scope,
            string grantType,
            string tokenUrl,
            CancellationToken cancellationToken)
        {
            var formContent = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = clientId,
                ["client_secret"] = clientSecret,
                ["scope"] = scope,
                ["grant_type"] = grantType
            });

            HttpResponseMessage response =
                await this.tokenHttpClient.PostAsync(tokenUrl, formContent, cancellationToken)
                    .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            string json =
                await response.Content.ReadAsStringAsync(cancellationToken)
                    .ConfigureAwait(false);

            BearerTokenResponse token =
                JsonSerializer.Deserialize<BearerTokenResponse>(json, jsonOptions)
                    ?? throw new InvalidOperationException(
                        "Failed to deserialize access token response.");

            this.accessToken = token.AccessToken;
            this.tokenExpiry = DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn - 30);
        }
    }
}