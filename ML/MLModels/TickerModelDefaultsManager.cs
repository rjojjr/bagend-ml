using System;
namespace bagend_ml.ML.MLModels
{
	public class TickerModelDefaultsManager
	{

        private readonly ModelMetaFileManager _modelMetaFileManager;
        private IList<DefaultTickerModelEntry> _defaults;

        public TickerModelDefaultsManager(ModelMetaFileManager modelMetaFileManager)
        {
            _modelMetaFileManager = modelMetaFileManager;
            _defaults = _modelMetaFileManager.GetDefaults();
        }

        public void AddOrUpdateDefault(DefaultTickerModelEntry defaultEntry)
        {
            lock (_defaults)
            {
                var newDefaults = new List<DefaultTickerModelEntry>();
                foreach (DefaultTickerModelEntry entry in _defaults)
                {
                    if (!entry.TickerSymbol.Equals(defaultEntry.TickerSymbol))
                    {
                        newDefaults.Add(entry);
                    }
                    else
                    {
                        newDefaults.Add(defaultEntry);
                    }
                }

                _modelMetaFileManager.SaveDefaults(newDefaults);
                _defaults = newDefaults;
            }
        }

    }
}

