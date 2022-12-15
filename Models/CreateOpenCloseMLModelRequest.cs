using System;
namespace bagend_ml.Models
{
	public class CreateOpenCloseMLModelRequest
	{
		public string ModelName { get; set; } = null!;
		public string ForcastedProperty { get; set; } = "ClosingPrice";
        public string StockTicker { get; set; } = null!;
		public int WindowSize { get; set; } = 2;

        public CreateOpenCloseMLModelRequest()
		{
		}
	}
}

