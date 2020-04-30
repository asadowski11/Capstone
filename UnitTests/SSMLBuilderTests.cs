using Capstone.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class SSMLBuilderTests
    {
        [TestMethod]
        public void TestNoOptionsBuildsSpeakElement()
        {
            string builtText = new SSMLBuilder().Build();
            Assert.AreEqual("<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='en-US'></speak>", builtText);
        }

        [TestMethod]
        public void TestSentenceCreatesSentenceElement()
        {
            string builtText = new SSMLBuilder().Sentence("test").Build();
            Assert.IsTrue(builtText.Contains("<sentence>test</sentence>"));
        }

        [TestMethod]
        public void TestBreakCreatesBreakElement()
        {
            // essentially 2 different tests
            Assert.IsTrue(new SSMLBuilder().Break().Build().Contains("<break />"));
            Assert.IsTrue(new SSMLBuilder().Break(1).Build().Contains("<break time='1' />"));
        }

        [TestMethod]
        public void TestParagraphBuildsParagraphElement()
        {
            Assert.IsTrue(new SSMLBuilder().Paragraph("test").Build().Contains("<p>test</p>"));
        }

        [TestMethod]
        public void TestProsodyBuildsProsodyElement()
        {
            Assert.IsTrue(new SSMLBuilder().Prosody("test").Build().Contains("<prosody>test</prosody>"));
            Assert.IsTrue(new SSMLBuilder().Prosody("test", pitch: "pitch", contour: "contour", range: "range", rate: "rate").Build().Contains("<prosody pitch='pitch' contour='contour' range='range' rate='rate'>test</prosody>"));
        }

        [TestMethod]
        public void TestSayAsBuildsSayAsElement()
        {
            Assert.IsTrue(new SSMLBuilder().SayAs("123 main street, New York City, NY 12345", SSMLBuilder.SayAsTypes.ADDRESS).Build().Contains("<say-as interpret-as='address'>123 main street, New York City, NY 12345</say-as>"));
        }

        [TestMethod]
        public void TestSubBuidlsSubElement()
        {
            Assert.IsTrue(new SSMLBuilder().Sub("JVM", "Java Virtual Machine").Build().Contains("<sub alias='Java Virtual Machine'>JVM</sub>"));
        }
    }
}
