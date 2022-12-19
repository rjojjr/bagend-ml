using System;
namespace bagend_ml.Config
{
	public class CollectiveModelBuilderServiceConfig
	{
        public bool BuildDailyModels { get; set; } = false;
        public int MaxThreads { get; set; } = 4;
    }
}

