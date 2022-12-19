using System;
using System.Text.Json;
using bagend_ml.ML.MLModels;

namespace bagend_ml.ML
{
	public class CollectiveMLModelMeta : IMLMeta
    {
        public CollectiveMLModelMeta(IList<string> models, string collectiveModelName, string stockTicker, DateTime createdAt, DateTime lastUpdateAt)
        {
            Models = models;
            CollectiveModelName = collectiveModelName;
            StockTicker = stockTicker;
            CreatedAt = createdAt;
            LastUpdateAt = lastUpdateAt;
        }

		public CollectiveMLModelMeta() { }

        public IList<string> Models { get; set; } = new List<string>();
        public string? CollectiveModelName { get; set; }
        public string? StockTicker { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdateAt { get; set; }

        public string getName()
        {
            return CollectiveModelName!;
        }

        public string getStockTicker()
        {
            return StockTicker!;
        }

        public string toJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public static CollectiveMLModelMeta? FromJson(string json)
        {
            return JsonSerializer.Deserialize<CollectiveMLModelMeta>(json);
        }
    }
	public class CollectiveMLModel
	{

        public readonly IList<TrainedModel> Models;
		public string CollectiveModelName { get; set; }
        public string StockTicker { get; set; }
        public DateTime CreatedAt { get; set; }
		public DateTime LastUpdateAt { get; set; }

		public CollectiveMLModel(IList<TrainedModel> trainedModels,
			string collectiveModelName,
            string stockTicker,
			DateTime createdAt,
			DateTime lastUpdatedAt) 
		{
			Models = trainedModels;
			CollectiveModelName = collectiveModelName;
            StockTicker = stockTicker;
			CreatedAt = createdAt;
			LastUpdateAt = lastUpdatedAt;
		}

		public CollectiveMLModel() { }

        public CollectiveMLModelMeta GetMeta()
		{
			return new CollectiveMLModelMeta(GetModelNames(),
				CollectiveModelName,
                StockTicker,
				CreatedAt,
				LastUpdateAt);
		}

		private IList<string> GetModelNames()
		{
			var names = new List<string>();
            foreach (TrainedModel model in Models)
            {
				names.Add(model.GetModelName());
            }
			return names;
        }

		public TrainedModel? GetModelByForcastedProperty(string forcastedProperty)
		{
            return getModelByForcastedProperty(forcastedProperty, Models);
		}

		private static TrainedModel getModelByForcastedProperty(string forcastedProperty, IList<TrainedModel> trainedModels)
        {
            foreach (TrainedModel model in trainedModels)
            {
                if (model.ForcastedProperty.Equals(forcastedProperty))
                {
                    return model;
                }
            }
            return null;
        }

    }
}

