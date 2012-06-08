using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NStatsD.Tests
{
    [TestClass]
    public class ClientTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void TestCleanup()
        {
            NStatsDClient.GlobalBucketPrefix = null;
        }
        #endregion

        [TestMethod]
        public void With_NoPrefix_SingleString()
        {
            var bucket = NStatsDClient.With("test.gauge");
            Assert.AreEqual("test.gauge", bucket.Name);
        }

        [TestMethod]
        public void With_NoPrefix_StringArray()
        {
            var bucket = NStatsDClient.With("test", "gauge");
            Assert.AreEqual("test.gauge", bucket.Name);
        }

        [TestMethod]
        public void With_Prefix_StringArray()
        {
            NStatsDClient.GlobalBucketPrefix = NStatsDClient.GetBucketName("machineName", "applicationName");
            var bucket = NStatsDClient.With("test", "gauge");
            Assert.AreEqual("machineName.applicationName.test.gauge", bucket.Name);
        }

        [TestMethod]
        public void With_Prefix_String()
        {
            NStatsDClient.GlobalBucketPrefix = NStatsDClient.GetBucketName("machineName", "applicationName");
            var bucket = NStatsDClient.With("test.gauge");
            Assert.AreEqual("machineName.applicationName.test.gauge", bucket.Name);
        }


        [TestMethod]
        public void WithoutPrefix_IgnoresPrefix()
        {
            NStatsDClient.GlobalBucketPrefix = NStatsDClient.GetBucketName("machineName", "applicationName");
            var bucket = NStatsDClient.WithoutPrefix("test.gauge");
            Assert.AreEqual("test.gauge", bucket.Name);
        }
    }
}
