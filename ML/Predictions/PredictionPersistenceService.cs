using System;
using bagend_ml.Client;
using bagend_ml.Client.Model;
using bagend_ml.ML.MLModels;
using bagend_ml.Util;

namespace bagend_ml.ML.Predictions
{
	public class PredictionPersistenceService
	{

		private readonly ILogger<PredictionPersistenceService> _logger;
		private readonly EventApiRESTClient _eventApiRESTClient;
        private readonly Executor _executor;

        public PredictionPersistenceService(ILogger<PredictionPersistenceService> logger, EventApiRESTClient eventApiRESTClient, Executor executor)
        {
            _logger = logger;
            _eventApiRESTClient = eventApiRESTClient;
            _executor = executor;
        }

        public void PersistPredictionAsync(string predictionName, string ticker, string modelName, DateTime madeAt, IList<CollectivePrediction> predictions)
        {
            _executor.execute(new ActionRunnable(() =>
            {
                PersistPrediction(predictionName, ticker, modelName, madeAt, predictions);
            }));
        }

        public void PersistPrediction(string predictionName, string ticker, string modelName, DateTime madeAt, IList<CollectivePrediction> predictions)
        {
            var predictionId = System.Guid.NewGuid().ToString();
            var events = new List<GenericEvent>();
            events.Add(CreatePredictionPersistedEvent(predictionId, predictionName, ticker, modelName, madeAt, predictions));
            foreach(CollectivePrediction prediction in predictions)
            {
                _eventApiRESTClient.SubmitEvent(ParsePrediction(predictionId, predictionName, ticker, modelName, madeAt, prediction));
            }
        }

		private static GenericEvent CreatePredictionPersistedEvent(string predictionId, string predictionName, string ticker, string model, DateTime timeMade, IList<CollectivePrediction> predictions)
		{
            var eventRequest = new GenericEvent();
            eventRequest.EventStream = "stock-data-predictions";
            eventRequest.EventName = "open-close-stock-prediction-persisted";
            eventRequest.EventType = "open-close-prediction-submission";

            var attributes = new List<EventAttribute>();
            attributes.Add(new EventAttribute("PredictionId", predictionId));
            attributes.Add(new EventAttribute("TimeMade", timeMade.ToString()));
            attributes.Add(new EventAttribute("PredictionNamde", predictionName));
            attributes.Add(new EventAttribute("TickerSymbol", ticker));
            attributes.Add(new EventAttribute("CollectiveModelName", model));
            attributes.Add(new EventAttribute("NumberOfPredictions", predictions.Count().ToString()));

            return eventRequest;

        }

        private static GenericEvent ParsePrediction(string predictionId, string predictionName, string ticker, string model, DateTime timeMade, CollectivePrediction prediction)
        {
            var eventRequest = new GenericEvent();
            eventRequest.EventStream = "stock-data-predictions";
            eventRequest.EventName = "open-close-stock-prediction-for-day";
            eventRequest.EventType = "open-close-prediction-submission";
            eventRequest.EventAttributes = BuildEventAttributes(predictionId, predictionName, ticker, model, timeMade, prediction);

            return eventRequest;

        }

        private static IList<EventAttribute> BuildEventAttributes(string predictionId, string predictionName, string ticker, string model, DateTime timeMade, CollectivePrediction collectivePrediction)
        {
            var attributes = new List<EventAttribute>();
            attributes.Add(new EventAttribute("PredictionId", predictionId));
            attributes.Add(new EventAttribute("PredictionNamde", predictionName));
            attributes.Add(new EventAttribute("TickerSymbol", ticker));
            attributes.Add(new EventAttribute("CollectiveModelName", model));
            foreach (Prediction prediction1 in collectivePrediction.Predictions)
            {
                attributes.Add(new EventAttribute(prediction1.ValueName, prediction1.PredictedValue.ToString()));
            }
            attributes.Add(new EventAttribute("TimeMade", timeMade.ToString()));
            attributes.Add(new EventAttribute("DayOfWeek", DateUtil.GetDayOfWeek(collectivePrediction.Date)));

            return attributes;
        }
    }
}

