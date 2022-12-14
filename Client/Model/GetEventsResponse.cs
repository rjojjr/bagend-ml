using System;
using System.Text.Json.Serialization;

namespace bagend_ml.Client.Model
{
    public class GetEventsResponse
    {

        public GetEventsResponse()
        {

        }

        public GetEventsResponse(int resultCount, IList<GenericEvent> results)
        {
            ResultCount = resultCount;
            Results = results;
        }

        [JsonPropertyName("resultCount")]
        public int ResultCount { get; set; }

        [JsonPropertyName("results")]
        public IList<GenericEvent> Results { get; set; } = new List<GenericEvent>();
    }
}

