using System;
using bagend_ml.Client.Model;
using bagend_ml.Util;

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
				var model = ExtractModelFromEvent(genericEvent);
				models.Add(model);
			}

            _logger.LogInformation("extracted {} forcasting training models, took {} millis", models.Count(), timer.getTimeElasped());

			models.Sort((e1, e2) => {
				return e1.Date.CompareTo(e2.Date);
            });
            return models;
		}

		private ForcastingModelInput ExtractModelFromEvent(GenericEvent genericEvent)
		{
			var date = DateUtil.GetDateTimeFromString(extractEventAttribute("Date", genericEvent));
            var model = new ForcastingModelInput();
			model.ClosingPrice = float.Parse(extractEventAttribute("Close", genericEvent));
			model.AfterHoursClosingPrice = float.Parse(extractEventAttribute("AfterHours", genericEvent));
            model.High = float.Parse(extractEventAttribute("High", genericEvent));
            model.Low = float.Parse(extractEventAttribute("Low", genericEvent));
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
    }
}

