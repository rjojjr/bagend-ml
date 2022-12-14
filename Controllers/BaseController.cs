using System;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using bagend_ml.Exceptions;

namespace bagend_ml.Controllers
{
	public class BaseController : ControllerBase
    {

        internal IActionResult ExecuteWithExceptionHandler(Func<IActionResult> func)
        {
            return (IActionResult)ExecuteWithExceptionHandler(func);
        }

        internal T ExecuteWithExceptionHandler<T>(Func<T> func)
        {
            try
            {
                return func.Invoke();
            }
            catch (HttpException e)
            {
                throw e.GetHttpRequestException();
            }
            catch (Exception e)
            {
                throw new HttpRequestException(e.Message, e, HttpStatusCode.InternalServerError);
            }
        }

        internal void ExecuteWithExceptionHandler(Action func)
        {
            try
            {
                func.Invoke();
            }
            catch (HttpException e)
            {
                throw e.GetHttpRequestException();
            }
            catch (Exception e)
            {
                throw new HttpRequestException(e.Message, e, HttpStatusCode.InternalServerError);
            }
        }
    }
}

