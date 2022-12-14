using System;
namespace bagend_ml.ML.Training
{
	public class ForcastingModelInput
	{

        public DateTime Date { get; set; }
		public decimal ClosingPrice { get; set; } = 0;
		public decimal AfterHoursClosingPrice { get; set; } = 0;
        public decimal High { get; set; } = 0;
        public decimal Low { get; set; } = 0;
        public int Year { get; set; } = 0;

        public ForcastingModelInput()
		{
		}
	}
}

