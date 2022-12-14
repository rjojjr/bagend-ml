using System;
using bagend_ml.Client.Model;

namespace bagend_ml.Models.ExternalEvents
{
	public class OpenCloseMLModelCreateEvent : ExternallyRecordedEvent
	{

		public string ModelName { get; set; } = null!;
		public string StockTicker { get; set; } = null!;
        public string PredictedValue { get; set; } = null!;
        public DateTime ModelCreationTimestamp { get; set; }

        public OpenCloseMLModelCreateEvent()
		{
		}

        public OpenCloseMLModelCreateEvent(string modelName,
			string stockTicker,
			string predictedValue,
			DateTime modelCreationTimestamp)
        {
            ModelName = modelName;
            StockTicker = stockTicker;
			PredictedValue = predictedValue;
            ModelCreationTimestamp = modelCreationTimestamp;
        }

        public GenericEvent GetGenericEvent()
		{
			var genericEvent = new GenericEvent("bagend_ml_open_close",
				"ml_model_change",
				"new_ml_model_built");

			var attributes = new List<EventAttribute>(new EventAttribute[]
			{
				new EventAttribute("ModelName", ModelName),
				new EventAttribute("StockTicker", StockTicker),
                new EventAttribute("PredictedValue", PredictedValue),
                new EventAttribute("ModelCreationTimestamp", ModelCreationTimestamp.ToLongDateString())
			});
			genericEvent.EventAttributes = attributes;

			return genericEvent;
		}
	}
}

