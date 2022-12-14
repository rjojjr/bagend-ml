using System;
using System.Text.Json;

namespace bagend_ml.ML
{
	public class ForcastingModelMeta
	{
		public DateTime LastUpdateTimestamp { get; set; }
        public DateTime ModelCreationTimestamp { get; set; }
        public string ModelName { get; set; } = null!;
		public string LatestModelFile { get; set; } = null!;
		public ModelEvalResults ModelEvalResults { get; set; } = null!;

        public ForcastingModelMeta()
		{
		}

        public ForcastingModelMeta(DateTime lastUpdateTimestamp,
			DateTime modelCreationTimestamp,
            string modelName,
			string latestModelFile,
			ModelEvalResults modelEvalResults)
        {
            LastUpdateTimestamp = lastUpdateTimestamp;
			ModelCreationTimestamp = modelCreationTimestamp;
            ModelName = modelName;
            LatestModelFile = latestModelFile;
			ModelEvalResults = modelEvalResults;
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

