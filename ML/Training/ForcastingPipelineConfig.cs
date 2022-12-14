using System;
namespace bagend_ml.ML.Training
{
	public class ForcastingPipelineConfig
	{

		public string OutputColumnName { get; set; } = null!;
        public string InputColumnName { get; set; } = null!;
        public int WindowSize { get; set; }
        public int SeriesLength { get; set; }
        public int Horizon { get; set; }
        public float ConfidenceLevel { get; set; }
        public String ConfidenceLowerBoundColumn { get; set; } = null!;
        public String ConfidenceUpperBoundColumn { get; set; } = null!;

        public ForcastingPipelineConfig()
		{
		}

        public ForcastingPipelineConfig(string outputColumnName,
            string inputColumnName,
            int windowSize,
            int seriesLength,
            int horizon,
            float confidenceLevel,
            string confidenceLowerBoundColumn,
            string confidenceUpperBoundColumn)
        {
            OutputColumnName = outputColumnName;
            InputColumnName = inputColumnName;
            WindowSize = windowSize;
            SeriesLength = seriesLength;
            Horizon = horizon;
            ConfidenceLevel = confidenceLevel;
            ConfidenceLowerBoundColumn = confidenceLowerBoundColumn;
            ConfidenceUpperBoundColumn = confidenceUpperBoundColumn;
        }
    }
}

