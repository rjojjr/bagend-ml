using System;
using bagend_ml.Client.Model;

namespace bagend_ml.ML.Training
{
	public class TrainingModelExtractor
	{

		private readonly ILogger<TrainingModelExtractor> _logger;

		public TrainingModelExtractor(ILogger<TrainingModelExtractor> logger)
		{
			_logger = logger;
		}

		public IList<ForcastingModelInput> ExtractForcastingModelsFromEvents(IList<GenericEvent> genericEvents)
		{
			_logger.LogInformation("extracting forcasting training models for {} events", genericEvents.Count());
			var timer = Timer.Timer.TimerFactory(true);
			var models = new List<ForcastingModelInput>();
			foreach(GenericEvent genericEvent in genericEvents)
			{
				models.Add(ExtractModelFromEvent(genericEvent));
			}

            _logger.LogInformation("extracted {} forcasting training models, took {} millis", genericEvents.Count(), timer.getTimeElasped());
            return models;
		}

		private ForcastingModelInput ExtractModelFromEvent(GenericEvent genericEvent)
		{
			var date = GetDateTimeFromString(extractEventAttribute("Date", genericEvent));
            var model = new ForcastingModelInput();
			model.ClosingPrice = decimal.Parse(extractEventAttribute("Close", genericEvent));
			model.AfterHoursClosingPrice = decimal.Parse(extractEventAttribute("AfterHours", genericEvent));
            model.High = decimal.Parse(extractEventAttribute("High", genericEvent));
            model.Low = decimal.Parse(extractEventAttribute("Low", genericEvent));
			model.Date = date;
			model.Year = date.Year;

			return model;
        }

		private static string extractEventAttribute(string attributeName, GenericEvent genericEvent)
		{
			foreach(EventAttribute attribute in genericEvent.EventAttributes)
			{
				if(attribute.EventAttributeName.ToLower().Equals(attributeName.ToLower()))
				{
					return attribute.EventAttributeValue.EventAttributeValue;
				}
            }
            return null;
        }

        private
    }
}

