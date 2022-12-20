using System;
using bagend_ml.Client.Model;
using bagend_ml.Config;
using Microsoft.Extensions.Options;
using RestSharp;

namespace bagend_ml.Client
{
	public class DataScraperApiRESTClient
	{
        private readonly ILogger<DataScraperApiRESTClient> _logger;
        private readonly IOptions<DataScraperApiConfig> _apiConfig;
        private readonly RestClient _restClient;

        public DataScraperApiRESTClient(IOptions<DataScraperApiConfig> apiConfig, ILogger<DataScraperApiRESTClient> logger)
        {
            _logger = logger;
            _apiConfig = apiConfig;
            _restClient = ApiClientFactory(apiConfig);
        }

        private static RestClient ApiClientFactory(IOptions<DataScraperApiConfig> apiConfig)
        {
            var options = new RestClientOptions(apiConfig.Value.Url)
            {
                ThrowOnAnyError = true
            };
            return new RestClient(options);
        }

        public TickerDataTargetResults GetTickerDataTargets()
        {
            _logger.LogInformation("fetching available ticker data targets");
            var timer = Timer.Timer.TimerFactory(true);
            var results = GetTickerTargetsAsync().Result;

            _logger.LogInformation("done fetching available ticker data targets found {} results, took {} millis", results.ResultsCount, timer.getTimeElasped());
            return results;
        }

        private async Task<TickerDataTargetResults> GetTickerTargetsAsync()
        {
            var request = new RestRequest("/data/target/api/v1");
            return await _restClient.GetAsync<TickerDataTargetResults> (request);
        }
    }
}

