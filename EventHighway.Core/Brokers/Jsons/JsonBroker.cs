// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

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

        public bool IsValidJson(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return false;

            try
            {
                JsonDocument.Parse(content);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }

        public string Canonicalize(string json)
        {
            JsonNode node = JsonNode.Parse(json);
            JsonNode sorted = SortNode(node);
            return sorted.ToJsonString();
        }

        public string RemoveNode(string json, string path) =>
            throw new NotImplementedException();

        private static JsonNode SortNode(JsonNode node)
        {
            if (node is JsonObject obj)
            {
                var sorted = new JsonObject();

                foreach (KeyValuePair<string, JsonNode> property in
                    obj.OrderBy(p => p.Key, StringComparer.Ordinal))
                {
                    sorted[property.Key] = SortNode(property.Value?.DeepClone());
                }

                return sorted;
            }

            if (node is JsonArray array)
            {
                var sortedArray = new JsonArray();

                foreach (JsonNode element in array)
                    sortedArray.Add(SortNode(element?.DeepClone()));

                return sortedArray;
            }

            return node?.DeepClone();
        }
    }
}
