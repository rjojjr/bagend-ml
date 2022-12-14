using System;
namespace bagend_ml.ML
{
	public class Prediction
	{

		public DateTime Date { get; set; }
		public float PredictedValue { get; set; }

		public Prediction(DateTime date, float predictedValue)
		{
			Date = date;
			PredictedValue = predictedValue;
		}

		public Prediction() { }
	}
}

