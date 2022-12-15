using System;
using bagend_ml.ML;
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
        private readonly ILogger<OpenCloseModelsController> _logger;

        public OpenCloseModelsController(ModelMetaFileManager modelMetaFileManager, OpenCloseMLEngine openCloseMLEngine, ILogger<OpenCloseModelsController> logger)
        {
            _modelMetaFileManager = modelMetaFileManager;
            _openCloseMLEngine = openCloseMLEngine;
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
                var metas = _modelMetaFileManager.GetAllModelMeta();
                return Ok(new ModelMetaSearchResponse(metas.Count(), metas));
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

