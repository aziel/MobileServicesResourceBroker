using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.WindowsAzure.Mobile.Service.ResourceBroker
{
    public class TestBase
    {
        public T ExpectException<T>(Action a) where T : Exception
        {
            T exception = null;

            try
            {
                a();
            }
            catch (T ex)
            {
                exception = ex;
            }
            catch (AggregateException aggex)
            {
                exception = aggex.InnerException as T;
            }

            Assert.IsNotNull(exception, "Exception did not occur");

            return exception;
        }

        public HttpResponseException ExpectHttpResponseException(HttpStatusCode status, Action a)
        {
            HttpResponseException response = this.ExpectException<HttpResponseException>(a);
            Assert.AreEqual(status, response.Response.StatusCode);

            return response;
        }
    }
}