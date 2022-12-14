using System;
using System.Text.Json.Serialization;

namespace bagend_ml.Client.Model
{
	public class GenericEvent
	{
        [JsonPropertyName("eventStream")]
        public string EventStream { get; set; } = null!;

        [JsonPropertyName("eventType")]
        public string EventType { get; set; } = null!;

        [JsonPropertyName("eventName")]
        public string EventName { get; set; } = null!;

        [JsonPropertyName("eventAttributes")]
        public IList<EventAttribute> EventAttributes { get; set; } = new List<EventAttribute>();

        public GenericEvent()
		{
		}

        public GenericEvent(string eventStream, string eventType, string eventName)
        {
            EventStream = eventStream;
            EventType = eventType;
            EventName = eventName;
        }
    }
}

