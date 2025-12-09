using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace RazorEngineCore.Tests
{
    [TestClass]
    public class TestTemplateFilename
    {
        [TestMethod]
        public void TestSettingTemplateFilename()
        {
            var razorEngine = new RazorEngine();
            var errorThrown = false;
            try
            {
                var initialTemplate = razorEngine.Compile<object>("@{ this is a syntaxerror }", 
                    builder => { builder.Options.TemplateFilename = "templatefilenameset.txt"; }, TestContext.CancellationToken);
            }
            catch (Exception e)
            {
                Assert.Contains("templatefilenameset.txt", e.Message);
                errorThrown = true;
            }

            Assert.IsTrue(errorThrown);
        }

        public TestContext TestContext { get; set; }
    }
}
