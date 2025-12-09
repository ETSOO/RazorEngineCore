using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorEngineCore.Tests.Models;

namespace RazorEngineCore.Tests
{
    [TestClass]
    public class TestTemplateModelNamespace
    {
        [TestMethod]
        public void TestModelNestedTypes()
        {
            var razorEngine = new RazorEngine();
            var content = "Hello @Model.Name";

            var template2 = razorEngine.Compile<RazorEngineTemplateBase<NestedTestModel.TestModelInnerClass1.TestModelInnerClass2>, NestedTestModel.TestModelInnerClass1.TestModelInnerClass2>(content, cancellationToken: TestContext.CancellationToken);

            var result = template2.Run(instance =>
            {
                instance.Model = new NestedTestModel.TestModelInnerClass1.TestModelInnerClass2()
                {
                    Name = "Hello",
                };
            });

            Assert.AreEqual("Hello Hello", result);
        }

        [TestMethod]
        public void TestModelNoNamespace()
        {
            var razorEngine = new RazorEngine();
            var content = "Hello @Model.Name";

            var template2 = razorEngine.Compile<RazorEngineTemplateBase<TestModelWithoutNamespace>, TestModelWithoutNamespace>(content, cancellationToken: TestContext.CancellationToken);

            var result = template2.Run(instance =>
            {
                instance.Model = new TestModelWithoutNamespace()
                {
                    Name = "Hello",
                };
            });

            Assert.AreEqual("Hello Hello", result);
        }

        public TestContext TestContext { get; set; }
    }
}