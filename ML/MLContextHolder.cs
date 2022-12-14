using System;
using Microsoft.ML;
namespace bagend_ml.ML
{
	public class MLContextHolder
	{

		private readonly MLContext _mLContext;

		public MLContextHolder()
		{
			_mLContext = new MLContext();
		}

		public MLContext GetMLContext()
		{
			return _mLContext;
		}
	}
}

