using System;
namespace bagend_ml.Client.Model
{
	public class TickerDataTarget
	{
        public string Id { get; set; } = null!;

        public int Priority { get; set; } = 100;

        public string TickerSymbol { get; set; } = null!;

        public string CompanyName { get; set; } = null!;

        public string BusinessSector { get; set; } = null!;

        public bool IsStarted { get; set; } = false;

        public bool IsCompleted { get; set; } = false;

        public bool IsActive { get; set; } = false;

        public string LastDatapointTimeValue { get; set; } = null!;

        public DateTime TargetCreatedAt { get; set; }

        public DateTime LastUpdatedAt { get; set; }
    }
}

