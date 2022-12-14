using System;
using System.Data;
using bagend_ml.Client;
using bagend_ml.Client.Model;
using Microsoft.ML;

namespace bagend_ml.ML.Training
{
	public class StockOpenCloseDataLoader
	{

		private readonly EventApiRESTClient _eventApiRESTClient;
		private readonly TrainingModelExtractor _trainingModelExtractor;
		private readonly ILogger<StockOpenCloseDataLoader> _logger;

		private readonly MLContextHolder _mlContextHolder;


        private ForcastingModelInput[] _trainingData { get; set; }

		public StockOpenCloseDataLoader(EventApiRESTClient eventApiRESTClient,
            TrainingModelExtractor trainingModelExtractor,
			MLContextHolder mlContextHolder,
            ILogger<StockOpenCloseDataLoader> logger)
		{
			_eventApiRESTClient = eventApiRESTClient;
			_trainingModelExtractor = trainingModelExtractor;
			_trainingData = new ForcastingModelInput[0];
			_logger = logger;

			_mlContextHolder = mlContextHolder;
        }

		public IDataView GetMasterDataView(string stockTicker)
		{
			_logger.LogInformation("loading open/close training data");
			var timer = Timer.Timer.TimerFactory(true);

			loadData(stockTicker);

            IDataView dataView = _mlContextHolder.GetMLContext().Data.LoadFromEnumerable<ForcastingModelInput>(_trainingData);

            _logger.LogInformation("done loading open/close training data, took {} millis", timer.getTimeElasped());

            return dataView;
        }

		public IDataView GetLatestTrainingData(string stockTicker)
		{
			return FilterDataViewByYear(2022, GetMasterDataView(stockTicker));
		}

		public IDataView FilterDataViewByYear(int year, IDataView dataView)
		{
            return _mlContextHolder.GetMLContext().Data.FilterRowsByColumn(dataView, "Year", upperBound: year + 1, lowerBound: year);
        }

		public IDataView BuildDataView(IList<ForcastingModelInput> inputs)
		{
			return _mlContextHolder.GetMLContext().Data.LoadFromEnumerable<ForcastingModelInput>(inputs);
        }

        private void loadData(string stockTicker)
		{
			var events = getEvents(stockTicker);
			processAndLoadData(events);
		}

		private IList<GenericEvent> getEvents(string stockTicker)
		{
			_logger.LogInformation("fetching generic events for training data loader");
			return _eventApiRESTClient.GetEventsByAttributeValue("Symbol", stockTicker);
		}

		private void processAndLoadData(IList<GenericEvent> events)
		{
			_logger.LogInformation("processing {} generic events for data loader", events.Count());
            var list = _trainingModelExtractor.ExtractForcastingModelsFromEvents(events);
			_trainingData = list.ToArray();
		}
	}
}

