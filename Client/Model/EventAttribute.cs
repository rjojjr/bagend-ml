using System;
using System.Text.Json.Serialization;

namespace bagend_ml.Client.Model
{
	public class EventAttribute
	{
        [JsonPropertyName("eventAttributeName")]
        public string EventAttributeName { get; set; } = null!;
        [JsonPropertyName("eventAttributeValue")]
        public DefaultEventAttributeValue EventAttributeValue { get; set; } = null!;
        
        public EventAttribute(string eventAttributeName, string eventAttributeValue)
		{
			this.EventAttributeName = eventAttributeName;
            this.EventAttributeValue = new DefaultEventAttributeValue(eventAttributeValue);
		}

        public EventAttribute()
        {

        }
	}
}

