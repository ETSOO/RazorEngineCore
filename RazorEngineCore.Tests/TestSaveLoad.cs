using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorEngineCore.Tests.Models;

namespace RazorEngineCore.Tests
{
    [TestClass]
    public class TestSaveLoad
    {
        [TestMethod]
        public void TestSaveToStream()
        {
            var razorEngine = new RazorEngine();
            var initialTemplate = razorEngine.Compile<TestNameModel>("Hello @Model.Name", cancellationToken: TestContext.CancellationToken);
            
            using var memoryStream = new MemoryStream();
            initialTemplate.SaveToStream(memoryStream);
            memoryStream.Position = 0;

            var loadedTemplate = RazorEngineCompiledTemplate<TestNameModel>.LoadFromStream(memoryStream);

            var initialTemplateResult = initialTemplate.Run(new TestNameModel { Name = "Alex" });
            var loadedTemplateResult = loadedTemplate.Run(new TestNameModel { Name = "Alex" });
            
            Assert.AreEqual(initialTemplateResult, loadedTemplateResult);
        }

        [TestMethod]
        public async Task TestSaveToStreamAsync()
        {
            var razorEngine = new RazorEngine();
            var initialTemplate = await razorEngine.CompileAsync<TestNameModel>("Hello @Model.Name", cancellationToken: TestContext.CancellationToken);

            await using var memoryStream = new MemoryStream();
            await initialTemplate.SaveToStreamAsync(memoryStream);
            memoryStream.Position = 0;

            var loadedTemplate = await RazorEngineCompiledTemplate<TestNameModel>.LoadFromStreamAsync(memoryStream);

            var initialTemplateResult = await initialTemplate.RunAsync(new TestNameModel { Name = "Alex" });
            var loadedTemplateResult = await loadedTemplate.RunAsync(new TestNameModel { Name = "Alex" });
            
            Assert.AreEqual(initialTemplateResult, loadedTemplateResult);
        }
        
        [TestMethod]
        public void TestSaveToFile_Typed()
        {
            var razorEngine = new RazorEngine();
            var initialTemplate = razorEngine.Compile<RazorEngineTemplateBase<TestModel>, TestModel>("Hello @Model.A @Model.C", cancellationToken: TestContext.CancellationToken);

            initialTemplate.SaveToFile("testTemplate.dll");

            var loadedTemplate = RazorEngineCompiledTemplate<RazorEngineTemplateBase<TestModel>, TestModel>.LoadFromFile("testTemplate.dll");

            var model = new TestModel()
            {
                A = 12345,
                C = "Alex"
            };

            var initialTemplateResult = initialTemplate.Run(model);
            var loadedTemplateResult = loadedTemplate.Run(model);

            Assert.AreEqual(initialTemplateResult, loadedTemplateResult);
        }
        
        [TestMethod]
        public void TestSaveToFile_Anonymous()
        {
            var razorEngine = new RazorEngine();
            var initialTemplate = razorEngine.Compile<TestNameModel>("Hello @Model.Name", cancellationToken: TestContext.CancellationToken);

            initialTemplate.SaveToFile("testTemplate.dll");

            var loadedTemplate = RazorEngineCompiledTemplate<TestNameModel>.LoadFromFile("testTemplate.dll");

            string initialTemplateResult = initialTemplate.Run(new TestNameModel { Name = "Alex" });
            string loadedTemplateResult = loadedTemplate.Run(new TestNameModel { Name = "Alex" });
            
            Assert.AreEqual(initialTemplateResult, loadedTemplateResult);
        }
        
        [TestMethod]
        public async Task TestSaveToFileAsync()
        {
            var razorEngine = new RazorEngine();
            var initialTemplate = await razorEngine.CompileAsync<TestNameModel>("Hello @Model.Name", cancellationToken: TestContext.CancellationToken);
            
            await initialTemplate.SaveToFileAsync("testTemplate.dll");

            var loadedTemplate = await RazorEngineCompiledTemplate<TestNameModel>.LoadFromFileAsync("testTemplate.dll");

            string initialTemplateResult = await initialTemplate.RunAsync(new TestNameModel { Name = "Alex" });
            string loadedTemplateResult = await loadedTemplate.RunAsync(new TestNameModel { Name = "Alex" });
            
            Assert.AreEqual(initialTemplateResult, loadedTemplateResult);
        }
        
        [TestMethod]
        public async Task TestSave_RazorEngineCompiledTemplateMeta_1()
        {
            var meta1 = new RazorEngineCompiledTemplateMeta()
            {
                AssemblyByteCode = [1, 2, 3],
                TemplateFileName = "name1",
                TemplateNamespace = "namespace1"
            };

            await using var memoryStream = new MemoryStream();

            await meta1.Write(memoryStream);
            memoryStream.Position = 0;

            var meta2 = await RazorEngineCompiledTemplateMeta.Read(memoryStream);

            CollectionAssert.AreEqual(meta1.AssemblyByteCode, meta2.AssemblyByteCode);
            CollectionAssert.AreEqual(meta1.PdbByteCode, meta2.PdbByteCode);
            Assert.AreEqual(meta1.TemplateFileName, meta2.TemplateFileName);
            Assert.AreEqual(meta1.TemplateNamespace, meta2.TemplateNamespace);
            Assert.AreEqual(meta1.GeneratedSourceCode, meta2.GeneratedSourceCode);
            Assert.AreEqual(meta1.TemplateSource, meta2.TemplateSource);
        }
        
        [TestMethod]
        public async Task TestSave_RazorEngineCompiledTemplateMeta_2()
        {
            var meta1 = new RazorEngineCompiledTemplateMeta()
            {
                AssemblyByteCode = [1, 2, 3],
                PdbByteCode = [1, 2, 3],
                TemplateFileName = "111",
                TemplateNamespace = "222",
                GeneratedSourceCode = "33333",
                TemplateSource = "44444"

            };

            await using var memoryStream = new MemoryStream();

            await meta1.Write(memoryStream);
            memoryStream.Position = 0;

            var meta2 = await RazorEngineCompiledTemplateMeta.Read(memoryStream);

            CollectionAssert.AreEqual(meta1.AssemblyByteCode, meta2.AssemblyByteCode);
            CollectionAssert.AreEqual(meta1.PdbByteCode, meta2.PdbByteCode);
            Assert.AreEqual(meta1.TemplateFileName, meta2.TemplateFileName);
            Assert.AreEqual(meta1.TemplateNamespace, meta2.TemplateNamespace);
            Assert.AreEqual(meta1.GeneratedSourceCode, meta2.GeneratedSourceCode);
            Assert.AreEqual(meta1.TemplateSource, meta2.TemplateSource);
        }

        public TestContext TestContext { get; set; }
    }
}