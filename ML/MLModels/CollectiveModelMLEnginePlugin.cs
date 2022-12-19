using System;
using System.Xml.Linq;
using bagend_ml.ML.Training;
using bagend_ml.ML.MLModels;
using bagend_ml.Util;
using System.CodeDom.Compiler;

namespace bagend_ml.ML.MLModels
{
	public class CollectiveModelMLEnginePlugin
    { 
        private readonly OpenCloseMLEngine _mLEngine;
		private readonly ModelMetaFileManager _metaFileManager;
        private readonly Executor _executor;

        public CollectiveModelMLEnginePlugin(OpenCloseMLEngine mLEngine,
            ModelMetaFileManager metaFileManager,
            Executor executor)
        {
            this._mLEngine = mLEngine;
            this._metaFileManager = metaFileManager;
            _executor = executor;
        }

        public CollectiveMLModel DeepCreateCollectiveOpenCloseModel(string name, string stockTicker)
        {
            var shallowCreateModelRequest = CollectiveModelRequestBuilder.Build(_executor, _mLEngine, name, stockTicker);
            return CreateCollectiveMLModel(shallowCreateModelRequest);
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

        private class CollectiveModelRequestBuilder
        {
            volatile int count = 0;
            volatile IList<string> models = new List<string>();
            private readonly Executor _executor;
            private readonly OpenCloseMLEngine _mLEngine;

            private CollectiveModelRequestBuilder(Executor executor, OpenCloseMLEngine mLEngine)
            {
                _executor = executor;
                _mLEngine = mLEngine;
            }

            public static CreateCollectiveModelRequest Build(Executor executor, OpenCloseMLEngine mLEngine, string name, string stockTicker)
            {
                return new CollectiveModelRequestBuilder(executor, mLEngine).BuildShallowCreateModelRequest(name, stockTicker);
            }

            private CreateCollectiveModelRequest BuildShallowCreateModelRequest(string name, string stockTicker)
            {
                foreach (string property in ForcastingModelInput.PropertyList)
                {
                    _executor.execute(new ActionRunnable(() =>
                    {

                        var model = _mLEngine.BuildTrainAndEvaluateModel(stockTicker, property, $"{name}_open-close_{property}", 2);
                        lock (this)
                        {
                            models.Add(model.GetModelName());
                            count++;
                        }

                    }));
                }

                while (count < ForcastingModelInput.PropertyList.Count())
                {
                    Thread.Sleep(10);
                }

                return new CreateCollectiveModelRequest(models, name, stockTicker);
            }
        }

    }
}

