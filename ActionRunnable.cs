using System;
namespace bagend_ml
{
	public class ActionRunnable : Runnable
	{
        private readonly Action _run;
		private string _threadName;

        public ActionRunnable(Action run)
		{
			_run = run;
			_threadName = "action-runnable-" + Guid.NewGuid().ToString();
		}

		public string GetThreadName()
		{
			return _threadName;
		}


        public string RegisterExecutor(string executorName)
		{
			_threadName = executorName + _threadName;
			return _threadName;
		}

		public void Run() => _run.Invoke();
	}
}

