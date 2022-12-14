using System;
namespace bagend_ml.Timer
{
	public class Timer
	{
		private long start = 0;

		public void startTimer()
		{
			start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

		public long getTimeElasped()
		{
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - start;
        }

		public bool isStarted()
		{
			return start != 0;
		}

        public Timer()
		{
		}

		public static Timer TimerFactory(bool start)
		{
			var timer = new Timer();
			if (start)
			{
				timer.startTimer();
			}

			return timer;
		}
	}
}

