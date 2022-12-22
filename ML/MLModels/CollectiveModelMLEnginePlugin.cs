using System;
using System.Xml.Linq;
using bagend_ml.ML.Training;
using bagend_ml.ML.MLModels;
using bagend_ml.Util;
using System.CodeDom.Compiler;
using bagend_ml.ML.Predictions;

namespace bagend_ml.ML.MLModels
{
	public class CollectiveModelMLEnginePlugin
    { 
        private readonly OpenCloseMLEngine _mLEngine;
		private readonly ModelMetaFileManager _metaFileManager;
        private readonly Executor _executor;
        private readonly ILogger<CollectiveModelMLEnginePlugin> _logger;
        private readonly PredictionPersistenceService _predictionPersistenceService;

        public CollectiveModelMLEnginePlugin(OpenCloseMLEngine mLEngine,
            ModelMetaFileManager metaFileManager,
            Executor executor,
            ILogger<CollectiveModelMLEnginePlugin> logger,
            PredictionPersistenceService predictionPersistenceService)
        {
            _mLEngine = mLEngine;
            _metaFileManager = metaFileManager;
            _executor = executor;
            _logger = logger;
            _predictionPersistenceService = predictionPersistenceService;
        }


        public IList<CollectivePrediction> GetAndPersistPredictions(GetAndPersistPredictionRequest request)
        {
            var predictions = GetPredictions(request.StartDate, request.EndDate, request.ModelName);
            _executor.execute(new ActionRunnable(() =>
            {
                _predictionPersistenceService.PersistPrediction(request.PredictionName, request.StockTicker, request.ModelName, DateTime.UtcNow, predictions);
            }));

            return predictions;
        }

        public CollectiveMLModel DeepCreateCollectiveOpenCloseModel(string name, string stockTicker, string date)
        {
            _logger.LogInformation("creating collective ML model {} for stock ticker {}", name, stockTicker);
            var shallowCreateModelRequest = CollectiveModelRequestBuilder.Build(_executor, _mLEngine, name, stockTicker, date);
            return CreateCollectiveMLModel(shallowCreateModelRequest);
        }


        public IList<CollectivePrediction> GetPredictions(string startDate, string endDate, string modelName)
        {
            _logger.LogInformation("getting predictions from collective ML model {} for dates {} - {}", modelName, startDate, endDate);
            var predictions = new List<CollectivePrediction>();
            var model = LoadCollectiveMLModel(modelName);
            var predictionsForEachModel = PredictionBuilder.Build(model, _mLEngine, _executor, startDate, endDate);

            for (int i = 0; i < predictionsForEachModel[0].Count(); i++)
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
            _logger.LogInformation("created collective model {}", request.CollectiveModelName);
            return collective;
        }

        private CollectiveMLModel LoadCollectiveMLModel(string name)
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

        private class PredictionBuilder
        {
            volatile IList<IList<Prediction>> predictionsForEachModel = new List<IList<Prediction>>();
            private volatile int count = 0;

            private readonly CollectiveMLModel _collectiveMLModel;
            private readonly OpenCloseMLEngine _mLEngine;
            private readonly Executor _executor;

            private PredictionBuilder(CollectiveMLModel collectiveMLModel, OpenCloseMLEngine mLEngine, Executor executor)
            {
                _collectiveMLModel = collectiveMLModel;
                _mLEngine = mLEngine;
                _executor = executor;
            }

            public static IList<IList<Prediction>> Build(CollectiveMLModel collectiveMLModel, OpenCloseMLEngine mLEngine, Executor executor, string startDate, string endDate)
            {
                return new PredictionBuilder(collectiveMLModel, mLEngine, executor).BuildPredictions(startDate, endDate);
            }

            private IList<IList<Prediction>> BuildPredictions(string startDate, string endDate)
            {
                foreach (TrainedModel trainedModel in _collectiveMLModel.Models)
                {
                    _executor.execute(new ActionRunnable(() =>
                    {
                        var predictions = _mLEngine.GetPredictions(startDate, endDate, trainedModel);
                        lock (this)
                        {
                            predictionsForEachModel.Add(predictions);
                            count++;
                        }

                    }));
                }

                while (count < _collectiveMLModel.Models.Count())
                {
                    Thread.Sleep(10);
                }

                return predictionsForEachModel;
            }

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

            public static CreateCollectiveModelRequest Build(Executor executor, OpenCloseMLEngine mLEngine, string name, string stockTicker, string date)
            {
                return new CollectiveModelRequestBuilder(executor, mLEngine).BuildShallowCreateModelRequest(name, stockTicker, date);
            }

            private CreateCollectiveModelRequest BuildShallowCreateModelRequest(string name, string stockTicker, string date)
            {
                foreach (string property in ForcastingModelInput.PropertyList)
                {
                    _executor.execute(new ActionRunnable(() =>
                    {

                        var model = _mLEngine.BuildTrainAndEvaluateModel(stockTicker, property, $"{name}_open-close_{property}", 2, date);

                        lock (this)
                        {
                            if (model != null)
                            {
                                models.Add(model.GetModelName());
                                count++;
                            }
                            else
                            {
                                count++;
                            }
                        }
                    }));
                }

                while (count < ForcastingModelInput.PropertyList.Count())
                {
                    Thread.Sleep(10);
                }

                return new CreateCollectiveModelRequest(models, name, stockTicker, date);
            }
        }

    }
}

