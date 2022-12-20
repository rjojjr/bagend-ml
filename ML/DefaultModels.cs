using System;
namespace bagend_ml.ML
{
	public class DefaultModels
	{

		public IList<DefaultTickerModelEntry> Defaults { get; set; } = new List<DefaultTickerModelEntry>();

        public DefaultModels(IList<DefaultTickerModelEntry> defaults)
        {
            Defaults = defaults;
        }

        public DefaultModels()
		{
		}
	}
}

