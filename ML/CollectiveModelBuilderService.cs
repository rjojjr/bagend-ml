using System;
using bagend_ml.Client;
using bagend_ml.Config;
using bagend_ml.ML.MLModels;
using Microsoft.Extensions.Options;

namespace bagend_ml.ML
{
	public class CollectiveModelBuilderService
	{

		private readonly ILogger<CollectiveModelBuilderService> _logger;
		private readonly DataScraperApiRESTClient _dataScraperApiClient;
		private readonly CollectiveModelMLEnginePlugin _collectiveModelMLEnginePlugin;
        private readonly IOptions<CollectiveModelBuilderServiceConfig> _config;
        private readonly Executor _executor;

        public CollectiveModelBuilderService(ILogger<CollectiveModelBuilderService> logger,
            DataScraperApiRESTClient dataScraperApiClient,
            CollectiveModelMLEnginePlugin collectiveModelMLEnginePlugin,
            IOptions<CollectiveModelBuilderServiceConfig> config,
            Executor executor)
        {
            _logger = logger;
            _dataScraperApiClient = dataScraperApiClient;
            _collectiveModelMLEnginePlugin = collectiveModelMLEnginePlugin;
            _config = config;
            _executor = executor;
        }


        //private IList<string> 
    }
}

