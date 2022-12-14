using System;
using Microsoft.ML;
namespace bagend_ml.ML
{
	public class MLContextHolder
	{

		private readonly MLContext _mLContext;

		public MLContextHolder(MLContext mLContext)
		{
			_mLContext = mLContext;
		}

		public MLContext GetMLContext()
		{
			return _mLContext;
		}
	}
}

