using System;
namespace bagend_ml.ML
{
	public class Prediction
	{

		public DateTime Date { get; set; }
		public float PredictedValue { get; set; }
		public string ValueName { get; set; }

		public Prediction(DateTime date, float predictedValue, string valueName)
		{
			Date = date;
			PredictedValue = predictedValue;
			ValueName = valueName;
		}

		public Prediction() { }
	}
}

