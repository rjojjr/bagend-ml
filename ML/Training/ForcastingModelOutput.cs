using System;
namespace bagend_ml.ML.Training
{
	public class ForcastingModelOutput
	{
        public float[] Forcasts { get; set; } = new float[0];
        //public float[] ForecastedAfterHoursClosingPrice { get; set; }
        //public float[] ForecastedHigh { get; set; }
        //public float[] ForecastedLow { get; set; }

        public float[] ForcastLowerBound { get; set; } = new float[0];

        public float[] ForcastUpperBound { get; set; } = new float[0];
    }
}

