using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorEngineCore.Tests.Models;

namespace RazorEngineCore.Tests
{
    [TestClass]
    public class TestTemplateNamespace
    {
        [TestMethod]
        public void TestSettingTemplateNamespace()
        {
            var razorEngine = new RazorEngine();

            var initialTemplate = razorEngine.Compile<object?>("@{ var message = \"OK\"; }@message",
                builder => { builder.Options.TemplateNamespace = "Test.Namespace"; }, TestContext.CancellationToken);

            var result = initialTemplate.Run(null);

            Assert.AreEqual("OK", result);
        }

        [TestMethod]
        public void TestSettingTemplateNamespaceT()
        {
            var razorEngine = new RazorEngine();

            var initialTemplate = razorEngine.Compile<TestTemplate2, TestModel>("@{ var message = \"OK\"; }@message",
                builder => { builder.Options.TemplateNamespace = "Test.Namespace"; }, TestContext.CancellationToken);

            var result = initialTemplate.Run(a => { });

            Assert.AreEqual("OK", result);
        }

        public TestContext TestContext { get; set; }
    }
}
