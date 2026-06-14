// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System.Text.Json;

namespace EventHighway.Core.Brokers.Serializations.Jsons
{
    internal class JsonSerializationBroker : IJsonSerializationBroker
    {
        public string Serialize<T>(T value) =>
            JsonSerializer.Serialize(value);

        public T Deserialize<T>(string json) =>
            JsonSerializer.Deserialize<T>(json);

        public string GetJsonPropertyValue(string json, string propertyName)
        {
            using JsonDocument document = JsonDocument.Parse(json);

            return document.RootElement.TryGetProperty(propertyName, out JsonElement element)
                ? element.GetString()
                : null;
        }
    }
}
