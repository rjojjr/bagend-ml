using System;
namespace bagend_ml.ML.Training
{
	public class ForcastingModelOutput
	{
        public float[] ForecastedClosingPrice { get; set; }
        //public float[] ForecastedAfterHoursClosingPrice { get; set; }
        //public float[] ForecastedHigh { get; set; }
        //public float[] ForecastedLow { get; set; }

        public float[] LowerBoundClosingPrice { get; set; }

        public float[] UpperBoundClosingPrice { get; set; }
	}
}

