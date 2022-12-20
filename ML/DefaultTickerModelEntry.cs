using System;
namespace bagend_ml.ML
{
	public class DefaultTickerModelEntry
	{

		public string TickerSymbol { get; set; } = null!;
        public string DefaultModel { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        public DefaultTickerModelEntry()
		{
		}

        public DefaultTickerModelEntry(string tickerSymbol, string defaultModel)
        {
            TickerSymbol = tickerSymbol;
            DefaultModel = defaultModel;
            CreatedAt = DateTime.UtcNow;
        }
    }
}

