// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

namespace EventHighway.Core.Brokers.Serializations.Jsons
{
    internal interface IJsonSerializationBroker
    {
        string Serialize<T>(T value);
        T Deserialize<T>(string json);
        string GetJsonPropertyValue(string json, string propertyName);
    }
}
