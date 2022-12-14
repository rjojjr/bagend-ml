using System;
namespace bagend_ml
{
	public interface Runnable
	{
		string GetThreadName()
		{
			return "runnable-" + Guid.NewGuid().ToString();
		}

		string RegisterExecutor(string executorName)
		{
			return "runnable-" + Guid.NewGuid().ToString();
		}

		void Run();
	}
}

