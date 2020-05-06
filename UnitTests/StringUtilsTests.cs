using BobTheDigitalAssistant.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class StringUtilsTests
    {
        [TestMethod]
        public void TestIsBlankReturnsTrueForNull()
        {
            Assert.IsTrue(StringUtils.IsBlank(null));
        }

        [TestMethod]
        public void TestIsBlankReturnsTrueForEmpty()
        {
            Assert.IsTrue(StringUtils.IsBlank(""));
        }

        [TestMethod]
        public void TestIsBlankReturnsTrueForOnlyWhitespace()
        {
            Assert.IsTrue(StringUtils.IsBlank("   \t\n"));
        }

        [TestMethod]
        public void TestIsBlankReturnsFalseForNonBlankString()
        {
            Assert.IsFalse(StringUtils.IsBlank("null hi   "));
        }
    }
}
