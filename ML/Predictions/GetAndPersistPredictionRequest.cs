using System;
namespace bagend_ml.ML.Predictions
{
	public class GetAndPersistPredictionRequest
	{

		public string PredictionName { get; set; }
		public string ModelName { get; set; }
		public string StockTicker { get; set; }
		public string StartDate { get; set; }
        public string EndDate { get; set; }

        public GetAndPersistPredictionRequest()
		{
		}
	}
}

