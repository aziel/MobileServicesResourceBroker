using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.WindowsAzure.Mobile.Service.ResourceBroker
{
    public class TestBase
    {
        public Exception ExpectException<T>(Action a) where T : Exception
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

            Assert.IsNotNull(exception, "Exception did not occur");

            return exception;
        }
    }
}