using System;
namespace bagend_ml.Models
{
	public class CreateOpenCloseMLModelRequest
	{
		public string ModelName { get; set; } = null!;
        public string StockTicker { get; set; } = null!;

        public CreateOpenCloseMLModelRequest()
		{
		}
	}
}

