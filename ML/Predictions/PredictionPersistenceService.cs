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

        public PredictionPersistenceService(ILogger<PredictionPersistenceService> logger, EventApiRESTClient eventApiRESTClient)
        {
            _logger = logger;
            _eventApiRESTClient = eventApiRESTClient;
        }

        public void PersistPrediction(string predictionName, string ticker, string model, DateTime timeMade, IList<CollectivePrediction> predictions)
        {
            var predictionId = System.Guid.NewGuid().ToString();
            var events = new List<GenericEvent>();
            foreach(CollectivePrediction prediction in predictions)
            {
                _eventApiRESTClient.SubmitEvent(ParsePrediction(predictionId, predictionName, ticker, model, timeMade, prediction));
            }
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

