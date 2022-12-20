using System;
namespace bagend_ml.ML.MLModels
{
	public class CreateCollectiveModelRequest
	{
		public IList<string> Models { get; set; } = new List<string>();
		public string CollectiveModelName { get; set; } = null!;
        public string StockTicker { get; set; } = null!;
        public string Date { get; set; }


        public CreateCollectiveModelRequest()
		{
		}

        public CreateCollectiveModelRequest(IList<string> models, string collectiveModelName, string stockTicker, string date)
        {
            Models = models;
            CollectiveModelName = collectiveModelName;
            StockTicker = stockTicker;
            Date = date;
        }
    }
}

