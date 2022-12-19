using System;
using System.Text.Json;
using bagend_ml.ML.MLModels;

namespace bagend_ml.ML
{
	public class ForcastingModelMeta : IMLMeta
    {
		public DateTime LastUpdateTimestamp { get; set; }
        public DateTime ModelCreationTimestamp { get; set; }
		public string LastDate { get; set; } = null!;
        public string StockTicker { get; set; } = null!;
        public string ModelName { get; set; } = null!;
		public string LatestModelFile { get; set; } = null!;
        public string ForcastedProperty { get; set; } = null!;
		public int WindowSize { get; set; }
        public ModelEvalResults ModelEvalResults { get; set; } = null!;

        public ForcastingModelMeta()
		{
		}

        public ForcastingModelMeta(DateTime lastUpdateTimestamp,
			DateTime modelCreationTimestamp,
			string lastDate,
            string stockTicker,
            string modelName,
			string latestModelFile,
			ModelEvalResults modelEvalResults,
            string forcastedProperty,
			int windowSize)
        {
            LastUpdateTimestamp = lastUpdateTimestamp;
			ModelCreationTimestamp = modelCreationTimestamp;
			LastDate = lastDate;
			StockTicker = stockTicker;
            ModelName = modelName;
            LatestModelFile = latestModelFile;
			ModelEvalResults = modelEvalResults;
            ForcastedProperty = forcastedProperty;
			WindowSize = windowSize;
        }

		public string getStockTicker()
        {
            return StockTicker!;
        }

        public string getName()
		{
			return ModelName;
		}

        public string toJson()
		{
            return JsonSerializer.Serialize(this);
        }

		public static ForcastingModelMeta FromJson(string json)
		{
			return JsonSerializer.Deserialize<ForcastingModelMeta>(json);
		}
	}
}

