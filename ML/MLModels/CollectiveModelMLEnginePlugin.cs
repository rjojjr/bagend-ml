using System;
using System.Xml.Linq;
using bagend_ml.ML.Training;
using bagend_ml.Util;

namespace bagend_ml.ML.MLModels
{
	public class CollectiveModelMLEnginePlugin
	{

		private readonly OpenCloseMLEngine _mLEngine;
		private readonly ModelMetaFileManager _metaFileManager;

        public CollectiveModelMLEnginePlugin(OpenCloseMLEngine mLEngine, ModelMetaFileManager metaFileManager)
        {
            this._mLEngine = mLEngine;
            this._metaFileManager = metaFileManager;
        }

        public CollectiveMLModel DeepCreateCollectiveOpenCloseModel(string name, string stockTicker)
        {
            var models = new List<string>();
            foreach(string property in ForcastingModelInput.PropertyList)
            {
                models.Add(_mLEngine.BuildTrainAndEvaluateModel(stockTicker, property, $"{name}_open-close_{property}", 2).GetModelName());
            }

            return CreateCollectiveMLModel(new CreateCollectiveModelRequest(models, name, stockTicker));
        }


        public IList<CollectivePrediction> GetPredictions(string startDate, string endDate, string modelName)
        {
            var predictions = new List<CollectivePrediction>();
            var model = LoadCollectiveMLModel(modelName);
            var predictionsForEachModel = new List<IList<Prediction>>();
            foreach(TrainedModel trainedModel in model.Models)
            {
                var currentPredictions = _mLEngine.GetPredictions(startDate, endDate, trainedModel);
                predictionsForEachModel.Add(currentPredictions);
            }

            
            for(int i = 0; i < predictionsForEachModel[0].Count(); i++)
            {
                var predictionsForPoint = new List<Prediction>();
                foreach(List<Prediction> predictions1 in predictionsForEachModel)
                {
                    predictionsForPoint.Add(predictions1[i]);
                }

                predictions.Add(new CollectivePrediction(predictionsForPoint, DateUtil.GetDateString(predictionsForPoint[0].Date)));
            }

            return predictions;
        }

        public CollectiveMLModel CreateCollectiveMLModel(CreateCollectiveModelRequest request)
        {
            var now = DateTime.UtcNow;
            var models = GetTrainedModels(request.Models);
            if (models.Contains(null))
            {
                throw new Exception($"cannot create collective ML model {request.CollectiveModelName} because one more provided model names does not exist");
            }

            var collective = new CollectiveMLModel(models, request.CollectiveModelName, request.StockTicker, now, now);
            var meta = collective.GetMeta();
            _metaFileManager.WriteMeta(meta);

            return collective;
        }

        public CollectiveMLModel LoadCollectiveMLModel(string name)
        {
            var meta = getMeta(name)!;
            var models = GetTrainedModels(meta.Models);
            return new CollectiveMLModel(models, name, meta.StockTicker, meta.CreatedAt, meta.LastUpdateAt);
        }

        private CollectiveMLModelMeta? getMeta(string name)
        {
            var maybeMeta = _metaFileManager.GetMeta<CollectiveMLModelMeta>(name);
            var isEmpty = maybeMeta!.Equals(default(CollectiveMLModelMeta));
            return isEmpty ? null : maybeMeta;
        }

        private IList<TrainedModel> GetTrainedModels(IList<string> names)
        {
            var models = new List<TrainedModel>();
            foreach(string name in names)
            {
                models.Add(loadModel(name));
            }
            return models;
        }

        private TrainedModel loadModel(string name)
        {
            return _mLEngine.LoadTrainedModel(name);
        }


    }
}

