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
		public string StockTicker { get; set; } = null!;

		public TrainedModel(ITransformer model,
            ModelEvalResults modelEvalResults,
			string modelName,
			DateTime creationTimestamp)
		{
			_model = model;
			_modelEvalResults = modelEvalResults;
			_modelName = modelName;
			_creationTimestamp = creationTimestamp;
			_lastInitalizedTimestamp = DateTime.UtcNow;
		}

        public TrainedModel(ITransformer model,
            ModelEvalResults modelEvalResults,
            string modelName,
            DateTime creationTimestamp,
			string stockTicker)
        {
            _model = model;
            _modelEvalResults = modelEvalResults;
            _modelName = modelName;
            _creationTimestamp = creationTimestamp;
            _lastInitalizedTimestamp = DateTime.UtcNow;
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
			string stockTicker)
        {
            var now = DateTime.UtcNow;
            _model = model;
            _modelEvalResults = modelEvalResults;
            _modelName = modelName;
            _creationTimestamp = now;
            _lastInitalizedTimestamp = now;
			StockTicker = stockTicker;
        }

		public ForcastingModelMeta GetModelMeta()
		{
			return new ForcastingModelMeta(_lastInitalizedTimestamp,
				_creationTimestamp,
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
			return "/var/bagend-ml/models/trained/" + _modelName + "_" + _creationTimestamp.ToFileTime() + ".zip";
		}

        public string GetModelFilename()
        {
            return "/var/bagend-ml/models/trained/" + _modelName + ".zip";
        }
    }
}

