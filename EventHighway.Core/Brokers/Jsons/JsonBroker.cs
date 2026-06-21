// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Text.Json;

namespace EventHighway.Core.Brokers.Jsons
{
    internal class JsonBroker : IJsonBroker
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

        public bool CheckIfPropertyExist(string json, string propertyName)
        {
            using JsonDocument document = JsonDocument.Parse(json);
            return document.RootElement.TryGetProperty(propertyName, out _);
        }

        public bool IsValidJson(string content) =>
            throw new NotImplementedException();

        public string Canonicalize(string json) =>
            throw new NotImplementedException();

        public string RemoveNode(string json, string path) =>
            throw new NotImplementedException();
    }
}
