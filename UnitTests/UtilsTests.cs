using Capstone.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests
{
    [TestClass]
    public class UtilsTests
    {
        private enum test
        {
            FIRST = 1,
            SECOND = 2,
            THIRD = 3
        }

        [TestMethod]
        public void TestJoinEnumProperlyJoinsElements()
        {
            Assert.AreEqual("FIRST - SECOND - THIRD", Utils.JoinEnum(typeof(test), " - "));
        }
    }
}
