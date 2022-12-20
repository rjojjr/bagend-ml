using System;
using System.Collections.Concurrent;
using bagend_ml.Client;
using bagend_ml.Client.Model;
using bagend_ml.Config;
using bagend_ml.ML.MLModels;
using bagend_ml.Util;
using Microsoft.Extensions.Logging;
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
        private readonly TickerModelDefaultsManager _tickerModelDefaultsManager;

        private ConcurrentQueue<ThreadStart> _operationQueue;

        private long total = 0;
        private long done = 0;

        public CollectiveModelBuilderService(ILogger<CollectiveModelBuilderService> logger,
            DataScraperApiRESTClient dataScraperApiClient,
            CollectiveModelMLEnginePlugin collectiveModelMLEnginePlugin,
            IOptions<CollectiveModelBuilderServiceConfig> config,
            Executor executor,
            TickerModelDefaultsManager tickerModelDefaultsManager)
        {
            _logger = logger;
            _dataScraperApiClient = dataScraperApiClient;
            _collectiveModelMLEnginePlugin = collectiveModelMLEnginePlugin;
            _config = config;
            _executor = executor;
            _tickerModelDefaultsManager = tickerModelDefaultsManager;
            _operationQueue = new ConcurrentQueue<ThreadStart>();
        }

        public void BuildModelsForDay(string date)
        {
            var day = date != null && date != "" ? date : DateUtil.GetToday();
            var millis = DateTime.UtcNow.Millisecond;
            _logger.LogInformation("submitting instructions to build collective ML models for day {}", day);
            var timer = Timer.Timer.TimerFactory(true);
            var tickers = GetTickers();
            Interlocked.Increment(ref total);
            foreach(string ticker in tickers)
            {
                _operationQueue.Enqueue(() => BuildModelThread(ticker, day));
            }
            var threads = new int[_config.Value.MaxThreads];
            foreach(int i in threads)
            {
                RunNext();
            }
            _logger.LogInformation("finished submitting instructions to build collective ML models for day {}, took {} millis", day, timer.getTimeElasped());
        }

        public long[] GetStatus()
        {
            return new long[]
            {
                total,
                done
            };
        }

        private void RunNext()
        {
            ThreadStart operation = null;
            if (_operationQueue.TryDequeue(out operation))
            {
                operation.Invoke();
            }
        }

        private void BuildModelThread(string ticker, string date)
        {
            _logger.LogInformation("starting builder thread for target {}", ticker);
            _executor.execute(new ActionRunnable(() =>
            {
                try
                {
                    BuildModelForTicker(ticker, date);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                }
                Interlocked.Increment(ref done);
                RunNext();
            }));
        }

        private void BuildModelForTicker(string ticker, string date)
        {
            var name = $"builder-{ticker}-{date}";
            _logger.LogInformation("building model {}", name);
            _collectiveModelMLEnginePlugin.DeepCreateCollectiveOpenCloseModel(name, ticker, date);
            _tickerModelDefaultsManager.AddOrUpdateDefault(new DefaultTickerModelEntry(ticker, name));
        }


        private IList<string> GetTickers()
        {
            var tickerDataTargets = _dataScraperApiClient.GetTickerDataTargets().Results;
            var tickers = new List<string>();

            foreach(TickerDataTarget target in tickerDataTargets)
            {
                tickers.Add(target.TickerSymbol);
            }

            return tickers;
        }


    }
}

