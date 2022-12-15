using System;
namespace bagend_ml.ML
{
	public class ModelEvalResults
	{
        public decimal MeanAbsoluteError { get; set; }
        public decimal RootMeanSquaredError { get; set; }

        public ModelEvalResults(decimal MeanAbsoluteError, decimal RootMeanSquaredError)
        {
            this.MeanAbsoluteError = MeanAbsoluteError;
            this.RootMeanSquaredError = RootMeanSquaredError;
        }

        public ModelEvalResults() { }
    }
}

