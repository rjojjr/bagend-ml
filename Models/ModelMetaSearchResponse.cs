using System;
using bagend_ml.ML;

namespace bagend_ml.Models
{
	public class ModelMetaSearchResponse<T>
	{
		public int ResultsCount { get; set; }
		public IList<T> Results { get; set; } = new List<T>();

		public ModelMetaSearchResponse()
		{
		}

        public ModelMetaSearchResponse(int resultsCount, IList<T> results)
        {
            ResultsCount = resultsCount;
            Results = results;
        }
    }
}

