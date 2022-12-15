using System;
namespace bagend_ml.ML.MLModels
{
	public class CollectivePrediction
	{

		public IList<Prediction> Predictions { get; set; } = new List<Prediction>();
		public string Date { get; set; } = null!;

		public CollectivePrediction()
		{
		}

        public CollectivePrediction(IList<Prediction> predictions, string date)
        {
            Predictions = predictions;
            Date = date;
        }
    }
}

