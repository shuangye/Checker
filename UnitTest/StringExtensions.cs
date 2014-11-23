using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pkg_Checker.Helpers;

namespace UnitTest
{
    [TestClass]
    public class StringExtensions
    {
        [TestMethod]
        public void TestSubString()
        {
            String target = "123456789";

            // both start mark and end mark are found
            Assert.AreEqual("123456789", target.SubString("12", "89", true, true));
            Assert.AreEqual("3456789", target.SubString("12", "89", false, true));
            Assert.AreEqual("1234567", target.SubString("12", "89", true, false));
            Assert.AreEqual("34567", target.SubString("12", "89", false, false));

            // start mark is not found
            Assert.AreEqual("123456789", target.SubString("ab", "89", true, true));
            Assert.AreEqual("123456789", target.SubString("ab", "89", false, true));
            Assert.AreEqual("1234567", target.SubString("ab", "89", true, false));
            Assert.AreEqual("1234567", target.SubString("ab", "89", false, false));

            // end mark is not found
            Assert.AreEqual("123456789", target.SubString("12", "cd", true, true));
            Assert.AreEqual("3456789", target.SubString("12", "cd", false, true));
            Assert.AreEqual("123456789", target.SubString("12", "cd", true, false));
            Assert.AreEqual("3456789", target.SubString("12", "cd", false, false));

            // robust tests
            Assert.AreEqual(null, target.SubString("", "", true, true));
            Assert.AreEqual(null, target.SubString(null, null, true, true));
        }
    }
}
