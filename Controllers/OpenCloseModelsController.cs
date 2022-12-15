using System;
using bagend_ml.ML;
using bagend_ml.ML.MLModels;
using bagend_ml.Models;
using bagend_ml.Util;
using Microsoft.AspNetCore.Mvc;

namespace bagend_ml.Controllers
{
    [ApiController]
    [Route("open/close/models/v1")]
    public class OpenCloseModelsController : BaseController
	{
		private readonly ModelMetaFileManager _modelMetaFileManager;
        private readonly OpenCloseMLEngine _openCloseMLEngine;
        private readonly CollectiveModelMLEnginePlugin _collectiveMlEngine;
        private readonly ILogger<OpenCloseModelsController> _logger;

        public OpenCloseModelsController(ModelMetaFileManager modelMetaFileManager,
            OpenCloseMLEngine openCloseMLEngine,
            CollectiveModelMLEnginePlugin mlModelManager,
            ILogger<OpenCloseModelsController> logger)
        {
            _modelMetaFileManager = modelMetaFileManager;
            _openCloseMLEngine = openCloseMLEngine;
            _collectiveMlEngine = mlModelManager;
            _logger = logger;
        }

        /// <summary>
        /// Fetches all current ML model metas.
        /// </summary>
        /// <remarks></remarks>
        /// <response code="200">Success</response>
        /// <response code="500">Something went wrong</response>
        [HttpGet]
        [Route("meta")]
        public IActionResult GetForcastingModelMetas()
        {
            return ExecuteWithExceptionHandler(() =>
            {
                _logger.LogInformation("received request to fetch all open/close ML model meta");
                var metas = _modelMetaFileManager.GetAllModelMeta<ForcastingModelMeta>();
                return Ok(new ModelMetaSearchResponse<ForcastingModelMeta>(metas.Count(), metas));
            });

        }

        /// <summary>
        /// Fetches all current collective ML model metas.
        /// </summary>
        /// <remarks>Collective models allow you to build more complex models.</remarks>
        /// <response code="200">Success</response>
        /// <response code="500">Something went wrong</response>
        [HttpGet]
        [Route("collective/meta")]
        public IActionResult GetCollectiveModelMetas()
        {
            return ExecuteWithExceptionHandler(() =>
            {
                _logger.LogInformation("received request to fetch all collective open/close ML model meta");
                var metas = _modelMetaFileManager.GetAllModelMeta<CollectiveMLModelMeta>();
                return Ok(new ModelMetaSearchResponse<CollectiveMLModelMeta>(metas.Count(), metas));
            });

        }


        /// <summary>
        /// Deep creates new collective ML model by automatically creating individual models.
        /// </summary>
        /// <remarks>Collective models allow you to build more complex models.</remarks>
        /// <response code="201">Success</response>
        /// <response code="500">Something went wrong</response>
        [HttpPost]
        [Route("collective/deep")]
        public IActionResult CreateCollectiveModel(DeepCreateCollectiveModelRequest request)
        {
            return ExecuteWithExceptionHandler(() =>
            {
                _logger.LogInformation("received request to create collective ML model {}", request.Name);
                return Created(".", _collectiveMlEngine.DeepCreateCollectiveOpenCloseModel(request.Name, request.StockTicker).GetMeta());
            });

        }

        /// <summary>
        /// Creates new collective ML model.
        /// </summary>
        /// <remarks>Collective models allow you to build more complex models.</remarks>
        /// <response code="201">Success</response>
        /// <response code="500">Something went wrong</response>
        [HttpPost]
        [Route("collective")]
        public IActionResult CreateCollectiveModel(CreateCollectiveModelRequest request)
        {
            return ExecuteWithExceptionHandler(() =>
            {
                _logger.LogInformation("received request to create collective ML model {}", request.CollectiveModelName);
                return Created(".", _collectiveMlEngine.CreateCollectiveMLModel(request).GetMeta());
            });

        }

        /// <summary>
        /// Get predictions for given model and date range.
        /// </summary>
        /// <remarks></remarks>
        /// <response code="200">Success</response>
        /// <response code="500">Something went wrong</response>
        [HttpGet]
        [Route("prediction")]
        public IActionResult GetForcastedPredictions([FromQuery] string modelName = "", [FromQuery] string startDate = "", [FromQuery] string endDate = "")
        {
            if(startDate.Equals(""))
            {
                startDate = DateUtil.GetToday();
            }
            if (endDate.Equals(""))
            {
                endDate = DateUtil.GetToday();
            }

            return ExecuteWithExceptionHandler(() =>
            {
                _logger.LogInformation("received request to fetch all open/close ML model meta");
                var predictions = _openCloseMLEngine.GetPredictions(startDate, endDate, modelName);
                return Ok(predictions);
            });

        }

        /// <summary>
        /// Get collective predictions for given model and date range.
        /// </summary>
        /// <remarks></remarks>
        /// <response code="200">Success</response>
        /// <response code="500">Something went wrong</response>
        [HttpGet]
        [Route("collective/prediction")]
        public IActionResult GetCollectiveForcastedPredictions([FromQuery] string modelName = "", [FromQuery] string startDate = "", [FromQuery] string endDate = "")
        {
            if (startDate.Equals(""))
            {
                startDate = DateUtil.GetToday();
            }
            if (endDate.Equals(""))
            {
                endDate = DateUtil.GetToday();
            }

            return ExecuteWithExceptionHandler(() =>
            {
                _logger.LogInformation("received request to fetch all open/close ML model meta");
                var predictions = _collectiveMlEngine.GetPredictions(startDate, endDate, modelName);
                return Ok(predictions);
            });

        }

        /// <summary>
        /// Builds and trains new open/close ML model.
        /// </summary>
        /// <remarks></remarks>
        /// <response code="201">Open/Close ML model created</response>
        /// <response code="500">Something went wrong</response>
        [HttpPost]
        public IActionResult BuildNewMLModel(CreateOpenCloseMLModelRequest request)
        {
            return ExecuteWithExceptionHandler(() =>
            {
                _logger.LogInformation("received request to build open/close ML model {} for ticker {} property {}", request.ModelName, request.StockTicker, request.ForcastedProperty);
                return Created(".", _openCloseMLEngine.BuildTrainAndEvaluateModel(request.StockTicker, request.ForcastedProperty, request.ModelName, request.WindowSize).GetModelMeta());
            });
        }
    }
}

