using System;
using bagend_ml.Client.Model;
using bagend_ml.Config;
using bagend_ml.Timer;
using Microsoft.Extensions.Options;
using RestSharp;

namespace bagend_ml.Client
{
	public class EventApiRESTClient
	{
        private readonly ILogger<EventApiRESTClient> _logger;
        private readonly IOptions<EventApiConfig> _apiConfig;
        private readonly RestClient _restClient;

        public EventApiRESTClient(IOptions<EventApiConfig> apiConfig, ILogger<EventApiRESTClient> logger)
        {
            _logger = logger;
            _apiConfig = apiConfig;
            _restClient = ApiClientFactory(apiConfig);
        }

        private static RestClient ApiClientFactory(IOptions<EventApiConfig> apiConfig)
        {
            var options = new RestClientOptions(apiConfig.Value.Url)
            {
                ThrowOnAnyError = true
            };
            return new RestClient(options);
        }

        public IList<GenericEvent> GetEventsByAttributeValue(string attributeName, string attributeValue)
        {
            _logger.LogInformation("fetching events with attribute value {} = {}", attributeName, attributeValue);
            return GetEventsByAttributeValueAsync(attributeName, attributeValue).Result.Results;
        }

        public void SubmitEvent(GenericEvent eventRequest)
        {
            _logger.LogInformation("submitting event to api stream {}", eventRequest.EventStream);
            var timer = Timer.Timer.TimerFactory(true);
            SubmitEventAsync(eventRequest).Wait();
            _logger.LogInformation("done submitting event to api stream {}, took {} millis", eventRequest.EventStream, timer.getTimeElasped());
        }

        private async Task<string> SubmitEventAsync(GenericEvent eventRequest)
        { 
            var request = new RestRequest("/generic/events/api/v1").AddBody(eventRequest);
            return await _restClient.PostAsync<string>(request);
        }

        private async Task<GetEventsResponse> GetEventsByAttributeValueAsync(string attributeName, string attributeValue)
        {
            var request = new RestRequest("/generic/events/api/v1/attribute?AttributeName=" + attributeName + "&AttributeValue=" + attributeValue);
            return await _restClient.GetAsync<GetEventsResponse>(request);
        }
    }
}

