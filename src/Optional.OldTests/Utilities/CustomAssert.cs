using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Optional.Tests.Utilities
{
    public static class CustomAssert
    {
        public static void Throws<TException>(Action action)
            where TException : Exception
        {
            var success = false;
            try
            {
                action();
                success = true;
            }
            catch (TException)
            {
            }
            catch (Exception)
            {
                Assert.Fail();
            }

            if (success)
            {
                Assert.Fail();
            }
        }
    }
}