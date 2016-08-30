using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Edreamer.Framework.Helpers;

namespace Edreamer.Framework.Test.Helpers.Exception
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class ThrowEvaluationTest
    {
        private ThrowEvaluation _throwEvaluation;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            var throwExcpetion = !TestContext.Properties.Contains("ThrowException") ||
                                 TestContext.Properties["ThrowException"].ToString().EqualsIgnoreCase("true");
            _throwEvaluation = new ThrowEvaluation(throwExcpetion);
        }
        
        [TestMethod]
        [ExpectedException(typeof(TestExceptionWithConstructor))]
        public void AMethodThrow()
        {
            _throwEvaluation.A<TestExceptionWithConstructor>("This exception is for test. Time: {0}.".FormatWith(DateTime.Now));
        }

        [TestMethod]
        [TestProperty("ThrowException", "false")]
        public void AMethodNotThrow()
        {
            _throwEvaluation.A<TestExceptionWithConstructor>("This exception is for test. Time: {0}.".FormatWith(DateTime.Now));
        }

        [TestMethod]
        [ExpectedException(typeof(ThrowingException))]
        public void AMethodFailure()
        {
            _throwEvaluation.A<TestExceptionWithoutConstructor>("This exception is for test. Time: {0}.".FormatWith(DateTime.Now));
        }
    }

    class TestExceptionWithConstructor : System.Exception
    {
        public TestExceptionWithConstructor(string message)
            : base(message)
        {
        }
    }

    class TestExceptionWithoutConstructor : System.Exception
    {

    }
}


