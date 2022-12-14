using System;
using bagend_ml.ML;

namespace bagend_ml.Models
{
	public class ModelMetaSearchResponse
	{
		public int ResultsCount { get; set; }
		public IList<ForcastingModelMeta> Results { get; set; } = new List<ForcastingModelMeta>();

		public ModelMetaSearchResponse()
		{
		}

        public ModelMetaSearchResponse(int resultsCount, IList<ForcastingModelMeta> results)
        {
            ResultsCount = resultsCount;
            Results = results;
        }
    }
}

