// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Text.Json;

namespace EventHighway.ClientV2.SubstrateApp.Brokers.Serializations
{
    public sealed class JsonSerializationBroker : IJsonSerializationBroker
    {
        public ValueTask<string> SerializeAsync<T>(T value) =>
            ValueTask.FromResult(JsonSerializer.Serialize(value));
    }
}
