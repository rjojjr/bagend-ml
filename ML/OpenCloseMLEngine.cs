using System;
using System.Data;
using System.Reflection;
using bagend_ml.ML.Training;
using bagend_ml.Models.ExternalEvents;
using bagend_ml.Util;
using Microsoft.ML;
using Microsoft.ML.TimeSeries;
using Microsoft.ML.Transforms.TimeSeries;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace bagend_ml.ML
{
    public class OpenCloseMLEngine
    {

        private readonly StockOpenCloseDataLoader _stockOpenCloseDataLoader;
        private readonly MLContextHolder _mlContextHolder;
        private readonly ILogger<OpenCloseMLEngine> _logger;
        private readonly ModelMetaFileManager _modelMetaFileManager;
        private readonly EventPersistenceService _eventPersistenceService;

        public OpenCloseMLEngine(StockOpenCloseDataLoader stockOpenCloseDataLoader,
            MLContextHolder mlContextHolder,
            ModelMetaFileManager modelMetaFileManager,
            ILogger<OpenCloseMLEngine> logger,
            EventPersistenceService eventPersistenceService)
        {
            _stockOpenCloseDataLoader = stockOpenCloseDataLoader;
            _mlContextHolder = mlContextHolder;
            _logger = logger;
            _modelMetaFileManager = modelMetaFileManager;
            _eventPersistenceService = eventPersistenceService;
        }

        private TrainedModel UpdateModel(TrainedModel trainedModel, IList<ForcastingModelInput> trainingData)
        {
            _logger.LogInformation($"training model {trainedModel.GetModelName()} with {trainingData.Count()} data points");
            var timer = Timer.Timer.TimerFactory(true);
            var predictionEngine = GetForcastingEngine(trainedModel.GetModel());
            foreach(ForcastingModelInput dataPoint in trainingData)
            {
                predictionEngine.Predict(dataPoint);
            }

            _logger.LogInformation($"finished training model {trainedModel.GetModelName()} with {trainingData.Count()} data points, took {timer.getTimeElasped()} millis");
            _logger.LogInformation("evaluating newly trained model {}", trainedModel.GetModelName());

            EvaluateModel(_stockOpenCloseDataLoader.GetLatestTrainingData(trainedModel.StockTicker), trainedModel);

            return PersistModel(trainedModel, predictionEngine);
        }

        public IList<Prediction> GetPredictions(string startDate, string endDate, string modelName)
        {
            var dates = DateUtil.GetDatesBetween(startDate, endDate);
            return GetPredictions(dates, modelName);
        }

        private IList<Prediction> GetPredictions(IList<string> dates, string modelName)
        {
            _logger.LogInformation("getting forcast from model {} for dates between {} and {}", modelName, dates.First(), dates.Last());
            var timer = Timer.Timer.TimerFactory(true);
            var model = LoadTrainedModel(modelName);

            var predictionsNeeded = DateUtil.GetNumberOfDaysBetween(model.LastDate, dates.Last());
            var daysInRange = DateUtil.GetNumberOfDaysBetween(model.LastDate, dates.First());

            _logger.LogDebug("daysInRange: {}, predictionsNeeded: {}", daysInRange, predictionsNeeded);

            var inputs = new List<ForcastingModelInput>();
            foreach(string date in dates)
            {
                var input = new ForcastingModelInput();
                input.Date = DateUtil.GetDateTimeFromString(date);
                inputs.Add(input);
            }

            var predictions = new List<Prediction>();
            var prediction = new List<float>(GetPredictions(predictionsNeeded, model).ForecastedClosingPrice);
            var wantedPredictions = prediction.GetRange(daysInRange - 1, prediction.Count());

            _logger.LogWarning("dates: {}, wanted: {}", dates.Count(), wantedPredictions.Count());

            for(int i = 0; i < wantedPredictions.Count(); i++)
            {
                predictions.Add(new Prediction(DateUtil.GetDateTimeFromString(dates[i]), wantedPredictions[i]));
            }

            _logger.LogInformation("finished getting forcast from model {} for dates between {} and {}, took {} millis", modelName, dates.First(), dates.Last(), timer.getTimeElasped());
            return predictions;
        }

        private ForcastingModelOutput GetPredictions(int predictionCount, TrainedModel model)
        {
            var predictions = new List<ForcastingModelOutput>();
            var predictionEngine = GetForcastingEngine(model.GetModel());
            return predictionEngine.Predict(horizon: predictionCount);
        }

        private TrainedModel PersistModel(TrainedModel model, TimeSeriesPredictionEngine<ForcastingModelInput,
                ForcastingModelOutput> timeSeriesPredictionEngine)
        {
            _logger.LogInformation($"persisting model {model.GetModelName()}");
            var timer = Timer.Timer.TimerFactory(true);
            timeSeriesPredictionEngine.CheckPoint(_mlContextHolder.GetMLContext(), model.GetModelFilename());
            _logger.LogInformation($"persisted model {model.GetModelName()}, took {timer.getTimeElasped()} millis");
            UpdateModelMeta(model);

            return LoadTrainedModel(model.GetModelName());
        }

        private void UpdateModelMeta(TrainedModel model)
        {
            var meta = model.GetModelMeta();
            meta.LastUpdateTimestamp = DateTime.UtcNow;
            _modelMetaFileManager.WriteMeta(meta);
        }

        private TrainedModel LoadTrainedModel(string modelName)
        {
            _logger.LogInformation("loading model {}", modelName);
            var timer = Timer.Timer.TimerFactory(true);
            var modelMeta = _modelMetaFileManager.GetMeta(modelName);
            if(modelMeta == null)
            {
                _logger.LogWarning("failed to load model {} because there is no meta record for it", modelName);
                return null;
            }
            ITransformer modelCopy;
            using (var file = File.OpenRead(modelMeta.LatestModelFile))
                modelCopy = _mlContextHolder.GetMLContext().Model.Load(file, out DataViewSchema schema);

            _logger.LogInformation("loaded model {}, took {} mills", modelName, timer.getTimeElasped());
            return new TrainedModel(modelCopy,
                modelMeta.ModelEvalResults,
                modelName,
                modelMeta.ModelCreationTimestamp,
                modelMeta.LastDate,
                modelMeta.StockTicker);
        }

        private IList<ForcastingModelInput> BuildPredictionRequestInputs(IList<string> dates)
        {
            var results = new List<ForcastingModelInput>();
            foreach(string date in dates)
            {
                var dateTime = DateUtil.GetDateTimeFromString(date);
                var input = new ForcastingModelInput();
                input.Date = dateTime;
                input.Year = dateTime.Year;

                results.Add(input);
            }
            return results;
        }

        private TimeSeriesPredictionEngine<ForcastingModelInput,
                ForcastingModelOutput> GetForcastingEngine(ITransformer transformer)
        {
            var engine = transformer.CreateTimeSeriesEngine<ForcastingModelInput,
                ForcastingModelOutput>(_mlContextHolder.GetMLContext());

            return engine;
        }

        public TrainedModel BuildTrainAndEvaluateModel(string stockTicker, string modelName)
        {
            var config = new ForcastingPipelineConfig("ForecastedClosingPrice",
                    "ClosingPrice",
                    2,
                    7,
                    7,
                    0.95f,
                    "LowerBoundClosingPrice",
                    "UpperBoundClosingPrice"
                );
            var wholeData = _stockOpenCloseDataLoader.GetMasterDataView(stockTicker);

            var trainingData = _stockOpenCloseDataLoader.FilterDataViewByYear(2021, wholeData);
            var testData = _stockOpenCloseDataLoader.FilterDataViewByYear(2022, wholeData);


            var lastDate = _mlContextHolder.GetMLContext().Data.CreateEnumerable<ForcastingModelInput>(testData, false).Last().Date;

            ITransformer model = trainAndGetModel(trainingData, config);

            var trainedModel = EvaluateModel(testData, new TrainedModel(model, null, modelName, stockTicker, DateUtil.GetDateString(lastDate)));

            PersistModel(trainedModel, GetForcastingEngine(model));

            _eventPersistenceService.PostRecordedEvent(new OpenCloseMLModelCreateEvent(modelName,
                stockTicker,
                "ClosingPrice",
                trainedModel.GetCreationTimestamp()));

            return trainedModel;
        }

        private Microsoft.ML.Transforms.TimeSeries.SsaForecastingEstimator BuildClosePriceForcastingPipeline(int dataPointCount, ForcastingPipelineConfig config)
        {
            return _mlContextHolder.GetMLContext().Forecasting.ForecastBySsa(
                outputColumnName: config.OutputColumnName,
                inputColumnName: config.InputColumnName,
                windowSize: config.WindowSize,
                seriesLength: config.SeriesLength,
                trainSize: dataPointCount,
                horizon: config.Horizon,
                confidenceLevel: config.ConfidenceLevel,
                confidenceLowerBoundColumn: config.ConfidenceLowerBoundColumn,
                confidenceUpperBoundColumn: config.ConfidenceUpperBoundColumn);
        }

        private SsaForecastingTransformer trainAndGetModel(IDataView dataView, ForcastingPipelineConfig config)
        {
            var count = dataView.Preview().RowView.ToList().Count();
            return BuildClosePriceForcastingPipeline(count, config).Fit(dataView);
        }



        private TrainedModel EvaluateModel(IDataView testData, TrainedModel model)
        {
            _logger.LogInformation("Evaluating ML Model");
            IDataView predictions = model.GetModel().Transform(testData);
            IEnumerable<float> actual =
                    _mlContextHolder.GetMLContext().Data.CreateEnumerable<ForcastingModelInput>(testData, true)
                        .Select(observed => observed.ClosingPrice);

            IEnumerable<float> forecast =
                _mlContextHolder.GetMLContext().Data.CreateEnumerable<ForcastingModelOutput>(predictions, true)
                    .Select(prediction => prediction.ForecastedClosingPrice[0]);

            var metrics = actual.Zip(forecast, (actualValue, forecastValue) => actualValue - forecastValue);

            var MAE = metrics.Average(error => Math.Abs(error)); 
            var RMSE = Math.Sqrt(metrics.Average(error => Math.Pow(double.Parse(error.ToString()), 2)));

            _logger.LogInformation("Model Evaluation");
            _logger.LogInformation("Mean Absolute Error: {}", MAE);
            _logger.LogInformation("Root Mean Squared Error: {}", RMSE);

            return new TrainedModel(model.GetModel(),
                new ModelEvalResults((decimal)MAE, (decimal)RMSE),
                model.GetModelName(),
                model.GetCreationTimestamp(),
                model.LastDate,
                model.StockTicker);
        }
    }
}
