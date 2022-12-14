﻿using System;
using bagend_ml.ML;
using bagend_ml.Models;
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
        /// Builds new open/close ML model.
        /// </summary>
        /// <remarks></remarks>
        /// <response code="201">Open/Close ML model created</response>
        /// <response code="500">Something went wrong</response>
        [HttpPost]
        public IActionResult BuildNewMLModel(CreateOpenCloseMLModelRequest request)
        {
            return ExecuteWithExceptionHandler(() =>
            {
                _logger.LogInformation("received request to build open/close ML model {} for ticker {}", request.ModelName, request.StockTicker);
                return Created(".", _openCloseMLEngine.BuildTrainAndEvaluateModel(request.StockTicker, request.ModelName).GetModelMeta());
            });
        }
    }
}

