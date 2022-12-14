using System;
using bagend_ml.Client;
using bagend_ml.Client.Model;

namespace bagend_ml
{
	public class Executor
	{

		private readonly ILogger<Executor> _logger;
		private string _name;


		public Executor(ILogger<Executor> logger)
		{
            _logger = logger;
			_name ="executor-" + DateTime.UtcNow.Microsecond;
        }

		public string GetName()
		{
			return _name;
		}

		public void SetName(string name)
		{
            _logger.LogInformation("executor {} - changing name to {}", _name, name);
            _name = name;
		}

		public void execute(Runnable action)
		{
			_logger.LogInformation("executor {} - submitting new thread {} to threadpool", _name, action.GetThreadName());
			WaitCallback waitCallback = (state) => action.Run();
            ThreadPool.QueueUserWorkItem(waitCallback);
        }

        public void executeImmediately(Runnable action)
        {
            _logger.LogInformation("executor {} - non-concurrently running new thread {]", _name, action.GetThreadName());
            WaitCallback waitCallback = (state) => action.Run();
            ThreadPool.QueueUserWorkItem(waitCallback);
        }
    }
}

