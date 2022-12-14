using System;
using System.Security.Cryptography.Xml;
using Microsoft.ML;

namespace bagend_ml.ML
{

	public class TrainedModel
	{
		private readonly ITransformer _model;
		private readonly ModelEvalResults _modelEvalResults;
		private readonly string _modelName;
		private readonly DateTime _creationTimestamp;
		private readonly DateTime _lastInitalizedTimestamp;
		public string LastDate { get; set; } = null!;
		public string StockTicker { get; set; } = null!;

		public TrainedModel(ITransformer model,
            ModelEvalResults modelEvalResults,
			string modelName,
			DateTime creationTimestamp,
            string lastDate,
			string stockTicker)
		{
			_model = model;
			_modelEvalResults = modelEvalResults;
			_modelName = modelName;
			_creationTimestamp = creationTimestamp;
			_lastInitalizedTimestamp = DateTime.UtcNow;
			LastDate = lastDate;
			StockTicker = stockTicker;
		}

        public TrainedModel(ITransformer model,
            ModelEvalResults modelEvalResults,
            string modelName)
        {
			var now = DateTime.UtcNow;
            _model = model;
            _modelEvalResults = modelEvalResults;
            _modelName = modelName;
			_creationTimestamp = now;
            _lastInitalizedTimestamp = now;
        }

        public TrainedModel(ITransformer model,
            ModelEvalResults modelEvalResults,
            string modelName,
			string stockTicker,
			string lastDate)
        {
            var now = DateTime.UtcNow;
            _model = model;
            _modelEvalResults = modelEvalResults;
            _modelName = modelName;
            _creationTimestamp = now;
            _lastInitalizedTimestamp = now;
			StockTicker = stockTicker;
			LastDate = lastDate;
        }

		public ForcastingModelMeta GetModelMeta()
		{
			return new ForcastingModelMeta(_lastInitalizedTimestamp,
				_creationTimestamp,
				LastDate,
				StockTicker,
				_modelName,
				GetModelFilename(),
				_modelEvalResults);
		}


        public ModelEvalResults GetEvalResults()
		{
			return _modelEvalResults;
		}

		public ITransformer GetModel()
		{
			return _model;
		}

		public string GetModelName()
		{
			return _modelName;
		}

		public DateTime GetCreationTimestamp()
		{
			return _creationTimestamp;
		}

		public string GetUniqueModelFilename()
		{
			return "/data/bagend-ml/models/trained/" + _modelName + "_" + _creationTimestamp.ToFileTime() + ".zip";
		}

        public string GetModelFilename()
        {
            return "/data/bagend-ml/models/trained/" + _modelName + ".zip";
        }
    }
}

