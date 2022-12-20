using System;
using System.Data;
using bagend_ml.Client;
using bagend_ml.Client.Model;
using bagend_ml.Util;
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

		public IDataView GetMasterDataView(string stockTicker, string endDate)
		{
			_logger.LogInformation("loading open/close training data");
			var timer = Timer.Timer.TimerFactory(true);

			loadData(stockTicker, endDate);

            IDataView dataView = _mlContextHolder.GetMLContext().Data.LoadFromEnumerable<ForcastingModelInput>(_trainingData);

            _logger.LogInformation("done loading open/close training data, took {} millis", timer.getTimeElasped());

            return dataView;
        }

		public IDataView GetLatestTrainingData(string stockTicker)
		{
			return FilterDataViewByYear(2022, 2022, GetMasterDataView(stockTicker, DateUtil.GetToday()));
		}

		public IDataView FilterDataViewByYear(int yearS, int yearE, IDataView dataView)
		{
            return _mlContextHolder.GetMLContext().Data.FilterRowsByColumn(dataView, "Year", upperBound: yearE + 1, lowerBound: yearS);
        }

		public IDataView BuildDataView(IList<ForcastingModelInput> inputs)
		{
			return _mlContextHolder.GetMLContext().Data.LoadFromEnumerable<ForcastingModelInput>(inputs);
        }

        private void loadData(string stockTicker, string endDate)
		{
			var events = getEvents(stockTicker, endDate);
			processAndLoadData(events);
		}

		private IList<GenericEvent> getEvents(string stockTicker, string endDate)
		{
			_logger.LogInformation("fetching generic events for training data loader");
			var events = _eventApiRESTClient.GetEventsByAttributeValue("Symbol", stockTicker);
			var results = new List<GenericEvent>();
			foreach(GenericEvent generic in events)
			{
				if(DateUtil.CompareDateStrings(extractEventAttribute("Date", generic), endDate) < 1)
				{
					results.Add(generic);
				}
			}
			return results;
		}

		private void processAndLoadData(IList<GenericEvent> events)
		{
			_logger.LogInformation("processing {} generic events for data loader", events.Count());
            var list = _trainingModelExtractor.ExtractForcastingModelsFromEvents(events);
			_trainingData = list.ToArray();
		}

        private static string extractEventAttribute(string attributeName, GenericEvent genericEvent)
        {
            foreach (EventAttribute attribute in genericEvent.EventAttributes)
            {
                if (attribute.EventAttributeName.ToLower().Equals(attributeName.ToLower()))
                {
                    return attribute.EventAttributeValue.EventAttributeValue;
                }
            }
            return null;
        }
    }
}

