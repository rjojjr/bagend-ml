using System;
namespace bagend_ml.ML.Training
{
	public class ForcastingModelOutput
	{
        public decimal[] ForecastedClosingPrice { get; set; }
        public decimal[] ForecastedAfterHoursClosingPrice { get; set; }
        public decimal[] ForecastedHigh { get; set; }
        public decimal[] ForecastedLow { get; set; }

        public decimal[] LowerBoundClosingPrice { get; set; }

        public decimal[] UpperBoundClosingPrice { get; set; }
	}
}

