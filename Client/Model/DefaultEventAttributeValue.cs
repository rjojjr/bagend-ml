using System;
using System.Text.Json.Serialization;

namespace bagend_ml.Client.Model
{
	public class DefaultEventAttributeValue
	{
        [JsonPropertyName("valueType")]
        public string ValueType { get; set; } = null!;
        [JsonPropertyName("eventAttributeValue")]
        public string EventAttributeValue { get; set; } = null!;

        public DefaultEventAttributeValue()
		{
			ValueType = "string";
		}

        public DefaultEventAttributeValue(string eventAttributeValue)
        {
            ValueType = "string";
            EventAttributeValue = eventAttributeValue;
        }

        public DefaultEventAttributeValue(string valueype, string eventAttributeValue)
        {
            ValueType = valueype;
            EventAttributeValue = eventAttributeValue;
        }
    }
}

