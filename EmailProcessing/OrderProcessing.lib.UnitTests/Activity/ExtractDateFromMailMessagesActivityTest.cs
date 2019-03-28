using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrderProcessing.Lib.Activities;

namespace OrderProcessing.lib.UnitTests.Activity
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ParseDate1()
        {
            //PrivateObject po = new PrivateObject(typeof(ExtractDateFromMailMessagesActivity));
            DateTime result = ExtractDateFromMailMessagesActivity.ParseDate("Date: 14 Mar 2019 11:38:54 - 0400");
            Assert.AreEqual(new DateTime(2019, 3, 14, 11, 38, 54), result);
        }
    }
}
