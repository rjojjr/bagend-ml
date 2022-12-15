using System;
using Microsoft.ML.Data;

namespace bagend_ml.ML.Training
{
	public class ForcastingModelInput
	{

        public DateTime Date { get; set; }

		[ColumnName("ClosingPrice")]
		public float ClosingPrice { get; set; } = 0;
		public float AfterHoursClosingPrice { get; set; } = 0;
        public float High { get; set; } = 0;
        public float Low { get; set; } = 0;
        [ColumnName("Year")]
        public float Year { get; set; } = 0;

        public readonly static IList<string> PropertyList = new List<string>(new string[]
        {
            "ClosingPrice",
            "AfterHoursClosingPrice",
            "High",
            "Low"
        });

        public ForcastingModelInput()
		{
		}
	}
}

