using System;
namespace bagend_ml.ML.MLModels
{
	public class TickerModelDefaultsManager
	{

        private readonly ModelMetaFileManager _modelMetaFileManager;

        public TickerModelDefaultsManager(ModelMetaFileManager modelMetaFileManager)
        {
            _modelMetaFileManager = modelMetaFileManager;
        }

        public IList<DefaultTickerModelEntry> GetDefaults()
        {
            return _modelMetaFileManager.GetDefaults();
        }

        public DefaultTickerModelEntry? GetDefault(string tickerSymbol)
        {
            foreach(DefaultTickerModelEntry entry in _modelMetaFileManager.GetDefaults())
            {
                if(entry.TickerSymbol.Equals(tickerSymbol))
                {
                    return entry;
                }
            }

            return null;
        }

        public void AddOrUpdateDefault(DefaultTickerModelEntry defaultEntry)
        {
            lock (this)
            {
                var newDefaults = new List<DefaultTickerModelEntry>();
                bool added = false;
                foreach (DefaultTickerModelEntry entry in _modelMetaFileManager.GetDefaults())
                {
                    if (!entry.TickerSymbol.Equals(defaultEntry.TickerSymbol))
                    {
                        newDefaults.Add(entry);
                    }
                    else
                    {
                        added = true;
                        newDefaults.Add(defaultEntry);
                    }
                }

                if(!added)
                {
                    newDefaults.Add(defaultEntry);
                }

                _modelMetaFileManager.SaveDefaults(newDefaults);
            }
        }

    }
}

