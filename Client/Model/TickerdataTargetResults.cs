using System;
namespace bagend_ml.Client.Model
{
    public class TickerDataTargetResults
    {

        public int ResultsCount { get; set; }
        public IList<TickerDataTarget> Results { get; set; } = new List<TickerDataTarget>();

        public TickerDataTargetResults(int resultsCount, IList<TickerDataTarget> results)
        {
            ResultsCount = resultsCount;
            Results = results;
        }
    }
}

