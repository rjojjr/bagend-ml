using System;
namespace bagend_ml.ML.MLModels
{
	public class DeepCreateCollectiveModelRequest
	{
		public string Name { get; set; }
		public string StockTicker { get; set; }
        public string Date { get; set; }
        public DeepCreateCollectiveModelRequest()
		{
		}

        public DeepCreateCollectiveModelRequest(string name, string stockTicker, string date)
        {
            Name = name;
            StockTicker = stockTicker;
            Date = date;
        }
    }
}

