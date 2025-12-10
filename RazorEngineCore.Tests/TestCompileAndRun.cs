using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RazorEngineCore.Tests.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace RazorEngineCore.Tests
{
    using System.Runtime.InteropServices;
    using System.Threading;

    [TestClass]
    public class TestCompileAndRun
    {
        [TestMethod]
        public void TestCompile()
        {
            var razorEngine = new RazorEngine();
            razorEngine.Compile<TestNameModel>("Hello @Model.Name", cancellationToken: TestContext.CancellationToken);
        }

        [TestMethod]
        public Task TestCompileAsync()
        {
            var razorEngine = new RazorEngine();
            return razorEngine.CompileAsync<TestNameModel>("Hello @Model.Name", cancellationToken: TestContext.CancellationToken);
        }

        [TestMethod]
        public void TestCompileAndRun_HtmlLiteral()
        {
            var data = new TestNameModel { Name = "Alex" };

            var razorEngine = new RazorEngine();
            var template = razorEngine.Compile<TestNameModel>("<h1>Hello @Model.Name</h1>", cancellationToken: TestContext.CancellationToken);

            var actual = template.Run(data);

            Assert.AreEqual("<h1>Hello Alex</h1>", actual);
        }

        [TestMethod]
        public async Task TestCompileAndRun_HtmlLiteralAsync()
        {
            var razorEngine = new RazorEngine();
            var template = await razorEngine.CompileAsync<TestNameModel>("<h1>Hello @Model.Name</h1>", cancellationToken: TestContext.CancellationToken);

            var actual = await template.RunAsync(new TestNameModel
            {
                Name = "Alex"
            });

            Assert.AreEqual("<h1>Hello Alex</h1>", actual);
        }

        [TestMethod]
        public void TestCompileAndRun_InAttributeVariables()
        {
            var razorEngine = new RazorEngine();
            var template = razorEngine.Compile<dynamic>("<div class=\"circle\" style=\"background-color: hsla(@Model.Colour, 70%,   80%,1);\">", cancellationToken: TestContext.CancellationToken);

            var actual = template.Run(new
            {
                Colour = 88
            });

            Assert.AreEqual("<div class=\"circle\" style=\"background-color: hsla(88, 70%,   80%,1);\">", actual);
        }

        [TestMethod]
        public void TestCompileAndRun_InAttributeVariables2()
        {
            var razorEngine = new RazorEngine();
            var template = razorEngine.Compile<dynamic>("<img src='@(\"test\")'>", cancellationToken: TestContext.CancellationToken);

            string actual = template.Run(new
            {
                Colour = 88
            });

            Assert.AreEqual("<img src='test'>", actual);
        }

        [TestMethod]
        public async Task TestCompileAndRun_InAttributeVariablesAsync()
        {
            var razorEngine = new RazorEngine();
            var template = await razorEngine.CompileAsync<dynamic>("<div class=\"circle\" style=\"background-color: hsla(@Model.Colour, 70%,   80%,1);\">", cancellationToken: TestContext.CancellationToken);

            string actual = await template.RunAsync(new
            {
                Colour = 88
            });

            Assert.AreEqual("<div class=\"circle\" style=\"background-color: hsla(88, 70%,   80%,1);\">", actual);
        }

        [TestMethod]
        public void TestCompileAndRun_HtmlAttribute()
        {
            var razorEngine = new RazorEngine();
            var template = razorEngine.Compile<TestNameModel>("<div title=\"@Model.Name\">Hello</div>", cancellationToken: TestContext.CancellationToken);

            string actual = template.Run(new TestNameModel
            {
                Name = "Alex"
            });

            Assert.AreEqual("<div title=\"Alex\">Hello</div>", actual);
        }

        [TestMethod]
        public async Task TestCompileAndRun_HtmlAttributeAsync()
        {
            var razorEngine = new RazorEngine();
            var template = await razorEngine.CompileAsync<TestNameModel>("<div title=\"@Model.Name\">Hello</div>", cancellationToken: TestContext.CancellationToken);

            string actual = await template.RunAsync(new TestNameModel
            {
                Name = "Alex"
            });

            Assert.AreEqual("<div title=\"Alex\">Hello</div>", actual);
        }

        [TestMethod]
        public void TestCompileAndRun_DynamicModel_Plain()
        {
            var razorEngine = new RazorEngine();
            var template = razorEngine.Compile<TestNameModel>("Hello @Model.Name", cancellationToken: TestContext.CancellationToken);

            string actual = template.Run(new TestNameModel
            {
                Name = "Alex"
            });

            Assert.AreEqual("Hello Alex", actual);
        }

        [TestMethod]
        public async Task TestCompileAndRun_DynamicModel_PlainAsync()
        {
            var razorEngine = new RazorEngine();
            var template = await razorEngine.CompileAsync<TestNameModel>("Hello @Model.Name", cancellationToken: TestContext.CancellationToken);

            string actual = await template.RunAsync(new TestNameModel
            {
                Name = "Alex"
            });

            Assert.AreEqual("Hello Alex", actual);
        }

        public struct Item
        {
            public string Name { get; set; }
        }

        [TestMethod]
        public void TestCompileAndRun_StructList()
        {
            var eng = new RazorEngine();
            var model = new
            {
                Items = new[] { new Item { Name = "Bob" }, new Item { Name = "Alice" } }
            };
            var temp = eng.Compile<dynamic>("@foreach(var item in Model.Items) { @item.Name }", cancellationToken: TestContext.CancellationToken);
            var result = temp.Run(model);
            Assert.AreEqual("BobAlice", result);
        }

        [TestMethod]
        public void TestCompileAndRun_DynamicModel_Nested()
        {
            var razorEngine = new RazorEngine();

            var model = new
            {
                Name = "Alex",
                Membership = new
                {
                    Level = "Gold"
                }
            };

            var template = razorEngine.Compile<dynamic>("Name: @Model.Name, Membership: @Model.Membership.Level", cancellationToken: TestContext.CancellationToken);

            string actual = template.Run(model);

            Assert.AreEqual("Name: Alex, Membership: Gold", actual);
        }

        [TestMethod]
        public async Task TestCompileAndRun_DynamicModel_NestedAsync()
        {
            var razorEngine = new RazorEngine();

            var model = new
            {
                Name = "Alex",
                Membership = new
                {
                    Level = "Gold"
                }
            };

            var template = await razorEngine.CompileAsync<dynamic>("Name: @Model.Name, Membership: @Model.Membership.Level", cancellationToken: TestContext.CancellationToken);

            string actual = await template.RunAsync(model);

            Assert.AreEqual("Name: Alex, Membership: Gold", actual);
        }

        [TestMethod]
        public void TestCompileAndRun_NullModel()
        {
            var razorEngine = new RazorEngine();

            var template = razorEngine.Compile<object?>("Name: @Model", cancellationToken: TestContext.CancellationToken);

            string actual = template.Run(null);

            Assert.AreEqual("Name: ", actual);
        }

        [TestMethod]
        public void TestCompileAndRun_NullablePropertyWithValue()
        {
            var razorEngine = new RazorEngine();

            DateTime? dateTime = DateTime.Now;

            var template = razorEngine.Compile<TestTemplate2, TestModel>("DateTime: @Model.DateTime.Value.ToString()", cancellationToken: TestContext.CancellationToken);

            var actual = template.Run(new TestModel()
            {
                DateTime = dateTime
            });

            Assert.AreEqual("DateTime: " + dateTime, actual);
        }

        [TestMethod]
        public void TestCompileAndRun_NullablePropertyWithoutValue()
        {
            var razorEngine = new RazorEngine();

            DateTime? dateTime = null;

            var template = razorEngine.Compile<TestTemplate2, TestModel>("DateTime: @Model.DateTime", cancellationToken: TestContext.CancellationToken);

            var actual = template.Run(new TestModel()
            {
                DateTime = dateTime
            });

            Assert.AreEqual("DateTime: " + dateTime, actual);
        }

        [TestMethod]
        public async Task TestCompileAndRun_NullModelAsync()
        {
            var razorEngine = new RazorEngine();

            var template = await razorEngine.CompileAsync<object?>("Name: @Model", cancellationToken: TestContext.CancellationToken);

            string actual = await template.RunAsync(null);

            Assert.AreEqual("Name: ", actual);
        }

        [TestMethod]
        public void TestCompileAndRun_NullNestedObject()
        {
            var razorEngine = new RazorEngine();

            var template = razorEngine.Compile<dynamic>("Name: @Model.user", cancellationToken: TestContext.CancellationToken);

            string actual = template.Run(new
            {
                user = (object?)null
            });

            Assert.AreEqual("Name: ", actual);
        }

        [TestMethod]
        public async Task TestCompileAndRun_NullNestedObjectAsync()
        {
            var razorEngine = new RazorEngine();

            var template = await razorEngine.CompileAsync<dynamic>("Name: @Model.user", cancellationToken: TestContext.CancellationToken);

            string actual = await template.RunAsync(new
            {
                user = (object?)null
            });

            Assert.AreEqual("Name: ", actual);
        }

        [TestMethod]
        public void TestCompileAndRun_DynamicModel_Lists()
        {
            var razorEngine = new RazorEngine();

            var model = new
            {
                Items = new[]
                {
                    new
                    {
                        Key = "K1"
                    },
                    new
                    {
                        Key = "K2"
                    }
                }
            };

            var template = razorEngine.Compile<dynamic>(@"
@foreach (var item in Model.Items)
{
<div>@item.Key</div>
}
", cancellationToken: TestContext.CancellationToken);

            string actual = template.Run(model);
            string expected = @"
<div>K1</div>
<div>K2</div>
";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task TestCompileAndRun_DynamicModel_ListsAsync()
        {
            var razorEngine = new RazorEngine();

            var model = new
            {
                Items = new[]
                {
                    new
                    {
                        Key = "K1"
                    },
                    new
                    {
                        Key = "K2"
                    }
                }
            };

            var template = await razorEngine.CompileAsync<dynamic>(@"
@foreach (var item in Model.Items)
{
<div>@item.Key</div>
}
", cancellationToken: TestContext.CancellationToken);

            string actual = await template.RunAsync(model);
            string expected = @"
<div>K1</div>
<div>K2</div>
";
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void TestCompileAndRun_DynamicModel_Dictionary1()
        {
            var razorEngine = new RazorEngine();

            var model = new
            {
                Dictionary = new Dictionary<string, object>()
                {
                    { "K1", "V1"},
                    { "K2", "V2"},
                }
            };

            var template = razorEngine.Compile<object>(@"
@foreach (var key in Model.Dictionary.Keys)
{
<div>@key</div>
}
<div>@Model.Dictionary[""K1""]</div>
<div>@Model.Dictionary[""K2""]</div>
", cancellationToken: TestContext.CancellationToken);

            string actual = template.Run(model);
            string expected = @"
<div>K1</div>
<div>K2</div>
<div>V1</div>
<div>V2</div>
";
            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void TestCompileAndRun_DynamicModel_Dictionary2()
        {
            var razorEngine = new RazorEngine();

            var model = new
            {
                Dictionary = new Dictionary<string, object>()
                {
                    { "K1", new { x = 1 } },
                    { "K2", new { x = 2 } },
                }
            };

            var template = razorEngine.Compile<dynamic>(@"
<div>@Model.Dictionary[""K1""].x</div>
<div>@Model.Dictionary[""K2""].x</div>
", cancellationToken: TestContext.CancellationToken);

            string actual = template.Run(model);
            string expected = @"
<div>1</div>
<div>2</div>
";
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCompileAndRun_TestFunction()
        {
            var razorEngine = new RazorEngine();

            var template = razorEngine.Compile<object?>(@"
@<area>
    @{ RecursionTest(3); }
</area>

@{

void RecursionTest(int level)
{
	if (level <= 0)
	{
		return;
	}
		
    <div>LEVEL: @level</div>
	@{ RecursionTest(level - 1); }
}

}", cancellationToken: TestContext.CancellationToken);

            string actual = template.Run(null);
            string expected = @"
<area>
    <div>LEVEL: 3</div>
    <div>LEVEL: 2</div>
    <div>LEVEL: 1</div>
</area>
";
            Assert.AreEqual(expected.Trim(), actual.Trim());
        }

        [TestMethod]
        public void TestCompileAndRun_TypedModel1()
        {
            var razorEngine = new RazorEngine();
            var template = razorEngine.Compile<TestTemplate1, object>("Hello @A @B @(A + B) @C @Decorator(\"777\")", cancellationToken: TestContext.CancellationToken);

            var actual = template.Execute((template) =>
            {
                var instance = (TestTemplate1)template;
                instance.A = 1;
                instance.B = 2;
                instance.C = "Alex";
            });

            Assert.AreEqual("Hello 1 2 3 Alex -=777=-", actual);
        }

        [TestMethod]
        public async Task TestCompileAndRun_TypedModel1Async()
        {
            var razorEngine = new RazorEngine();
            var template = await razorEngine.CompileAsync<TestTemplate1, object>("Hello @A @B @(A + B) @C @Decorator(\"777\")", cancellationToken: TestContext.CancellationToken);

            var actual = await template.ExecuteAsync((template) =>
            {
                var instance = (TestTemplate1)template;
                instance.A = 1;
                instance.B = 2;
                instance.C = "Alex";
            });

            Assert.AreEqual("Hello 1 2 3 Alex -=777=-", actual);
        }

        [TestMethod]
        public void TestCompileAndRun_TypedModel2()
        {
            var razorEngine = new RazorEngine();
            var template = razorEngine.Compile<TestTemplate2, TestModel>("Hello @Model.Decorator(Model.C)", cancellationToken: TestContext.CancellationToken);

            var actual = template.Run(new TestModel
            {
                C = "Alex"
            });

            Assert.AreEqual("Hello -=Alex=-", actual);
        }

        [TestMethod]
        public void TestCompileAndRun_TypedModel3()
        {
            string templateText = @"
@inherits RazorEngineCore.RazorEngineTemplateBase<RazorEngineCore.Tests.Models.TestModel>
Hello @Model.Decorator(Model.C)
";

            var razorEngine = new RazorEngine();
            var template = razorEngine.Compile<RazorEngineTemplateBase<TestModel>, TestModel>(templateText, cancellationToken: TestContext.CancellationToken);

            var actual = template.Run(new TestModel
            {
                C = "Alex"
            });

            Assert.AreEqual("Hello -=Alex=-", actual.Trim());
        }

        [TestMethod]
        public async Task TestCompileAndRun_TypedModel2Async()
        {
            var razorEngine = new RazorEngine();
            var template = await razorEngine.CompileAsync<TestTemplate2, TestModel>("Hello @Model.Decorator(Model.C)", cancellationToken: TestContext.CancellationToken);

            var actual = await template.RunAsync(new TestModel
            {
                C = "Alex"
            });

            Assert.AreEqual("Hello -=Alex=-", actual);
        }

        [TestMethod]
        public void TestCompileAndRun_AnonymousModelWithArrayOfObjects()
        {
            var razorEngine = new RazorEngine();
            var template = razorEngine.Compile<TestTemplate2, TestModel>(
@"
@foreach (var item in Model.Numbers.OrderByDescending(x => x))
{
    <p>@item</p>
}
", cancellationToken: TestContext.CancellationToken);
            string expected = @"
    <p>3</p>
    <p>2</p>
    <p>1</p>
";
            var actual = template.Run(new TestModel
            {
                Numbers = [2, 1, 3]
            });

            Assert.AreEqual(expected, actual);
        }


        [TestMethod]
        public void TestCompileAndRun_StronglyTypedModelLinq()
        {
            var razorEngine = new RazorEngine();
            var template = razorEngine.Compile<TestTemplate2, TestModel>(
@"
@foreach (var item in Model.Numbers.OrderByDescending(x => x))
{
    <p>@item</p>
}
", cancellationToken: TestContext.CancellationToken);
            string expected = @"
    <p>3</p>
    <p>2</p>
    <p>1</p>
";
            var actual = template.Run(new TestModel
            {
                Numbers = [2, 1, 3]
            });

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCompileAndRun_DynamicModelLinq()
        {
            var razorEngine = new RazorEngine();
            var template = razorEngine.Compile<TestModel>(
@"
@foreach (var item in Model.Objects.OrderByDescending(x => x))
{
    <p>@item</p>
}
", cancellationToken: TestContext.CancellationToken);
            string expected = @"
    <p>3</p>
    <p>2</p>
    <p>1</p>
";
            string actual = template.Run(new TestModel
            {
                Objects = [2, 1, 3]
            });

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task TestCompileAndRun_LinqAsync()
        {
            var razorEngine = new RazorEngine();
            var template = await razorEngine.CompileAsync<TestTemplate2, TestModel>(
@"
@foreach (var item in Model.Numbers.OrderByDescending(x => x))
{
    <p>@item</p>
}
", cancellationToken: TestContext.CancellationToken);
            string expected = @"
    <p>3</p>
    <p>2</p>
    <p>1</p>
";
            var actual = await template.RunAsync(new TestModel
            {
                Numbers = [2, 1, 3]
            });

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public async Task TestCompileAndRun_MetadataReference()
        {
            string greetingClass = @"
namespace TestAssembly
{
    public static class Greeting
    {
        public static string GetGreeting(string name)
        {
            return ""Hello, "" + name + ""!"";
        }
    }
}
";
            // This needs to be done in the builder to have access to all of the assemblies added through
            // the various AddAssemblyReference options
            CSharpCompilation compilation = CSharpCompilation.Create(
                    "TestAssembly",
                    [
                            CSharpSyntaxTree.ParseText(greetingClass, cancellationToken: TestContext.CancellationToken)
                    ],
                    GetMetadataReferences(),
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );

            var memoryStream = new MemoryStream();
            EmitResult emitResult = compilation.Emit(memoryStream, cancellationToken: TestContext.CancellationToken);

            if (!emitResult.Success)
            {
                Assert.Fail("Unable to compile test assembly");
            }

            memoryStream.Position = 0;

            // Add an assembly resolver so the assembly can be found
            AppDomain.CurrentDomain.AssemblyResolve += (sender, eventArgs) =>
                    new AssemblyName(eventArgs.Name ?? string.Empty).Name == "TestAssembly"
                            ? Assembly.Load(memoryStream.ToArray())
                            : null;

            var razorEngine = new RazorEngine();
            var template = await razorEngine.CompileAsync<TestNameModel>(@"
@using TestAssembly
<p>@Greeting.GetGreeting(""Name"")</p>
", builder =>
            {
                builder.AddMetadataReference(MetadataReference.CreateFromStream(memoryStream));

            }, TestContext.CancellationToken);

            string expected = @"
<p>Hello, Name!</p>
";
            string actual = await template.RunAsync(new TestNameModel { Name = "Name" });

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void TestCompileCancellation_DynamicModel()
        {
            var razorEngine = new RazorEngine();

            using CancellationTokenSource cancellationSource = new CancellationTokenSource();
            cancellationSource.Cancel();

            Assert.Throws<OperationCanceledException>(() =>
            {
                _ = razorEngine.Compile<TestNameModel>("Hello @Model.Name", cancellationToken: cancellationSource.Token);
            });
        }

        [TestMethod]
        public async Task TestCompileCancellation_DynamicModelAsync()
        {
            var razorEngine = new RazorEngine();

            using var cancellationSource = new CancellationTokenSource();
            cancellationSource.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                _ = await razorEngine.CompileAsync<TestNameModel>("Hello @Model.Name", cancellationToken: cancellationSource.Token);
            });
        }

        [TestMethod]
        public void TestCompileCancellation_TypedModel1()
        {
            var razorEngine = new RazorEngine();

            using var cancellationSource = new CancellationTokenSource();
            cancellationSource.Cancel();

            Assert.Throws<OperationCanceledException>(() =>
            {
                _ = razorEngine.Compile<TestTemplate1>("Hello @A @B @(A + B) @C @Decorator(\"777\")", cancellationToken: cancellationSource.Token);
            });
        }

        [TestMethod]
        public async Task TestCompileCancellation_TypedModel1Async()
        {
            var razorEngine = new RazorEngine();

            using var cancellationSource = new CancellationTokenSource();
            cancellationSource.Cancel();

            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                _ = await razorEngine.CompileAsync<TestTemplate1>("Hello @A @B @(A + B) @C @Decorator(\"777\")", cancellationToken: cancellationSource.Token);
            });
        }

        private static List<MetadataReference> GetMetadataReferences()
        {
            if (RuntimeInformation.FrameworkDescription.StartsWith(
                ".NET Framework",
                StringComparison.OrdinalIgnoreCase))
            {
                return
                    [
                        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                        MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.CSharp, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")).Location),
                        MetadataReference.CreateFromFile(Assembly.Load(
                            new AssemblyName(
                                "netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51")).Location),
                        MetadataReference.CreateFromFile(typeof(System.Runtime.GCSettings).Assembly.Location)
                    ];
            }

            return
                [
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("Microsoft.CSharp")).Location),
                    MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("netstandard")).Location),
                    MetadataReference.CreateFromFile(Assembly.Load(new AssemblyName("System.Runtime")).Location)
                ];
        }



        [TestMethod]
        public void TestCompileAndRun_IncludeDebuggingForTypedMode_DisabledDebugging()
        {
            string templateText = @"
@inherits RazorEngineCore.RazorEngineTemplateBase<RazorEngineCore.Tests.Models.TestModel>
Hello @Model.Decorator(Model.C)
";

            var razorEngine = new RazorEngine();
            var template = razorEngine.Compile<RazorEngineTemplateBase<TestModel>, TestModel>(templateText, builder =>
            {
                builder.IncludeDebuggingInfo();
            }, TestContext.CancellationToken);

            var actual = template.Run(new TestModel
            {
                C = "Alex"
            });

            Assert.AreEqual("Hello -=Alex=-", actual.Trim());
        }

        [TestMethod]
        public void TestCompileAndRun_IncludeDebuggingForTypedAnonymous_DisabledDebugging()
        {
            var razorEngine = new RazorEngine();
            var template = razorEngine.Compile<TestNameModel>("<h1>Hello @Model.Name</h1>", builder =>
            {
                builder.IncludeDebuggingInfo();
            }, TestContext.CancellationToken);

            string actual = template.Run(new TestNameModel
            {
                Name = "Alex"
            });

            Assert.AreEqual("<h1>Hello Alex</h1>", actual);
        }

        [TestMethod]
        public void TestCompileAndRun_IncludeDebuggingForTypedMode_EnabledDebugging()
        {
            string templateText = @"
@inherits RazorEngineCore.RazorEngineTemplateBase<RazorEngineCore.Tests.Models.TestModel>
Hello @Model.Decorator(Model.C)
";

            var razorEngine = new RazorEngine();
            var template = razorEngine.Compile<RazorEngineTemplateBase<TestModel>, TestModel>(templateText, builder =>
            {
                builder.IncludeDebuggingInfo();
            }, TestContext.CancellationToken);

            template.EnableDebugging();

            var actual = template.Run(new TestModel
            {
                C = "Alex"
            });

            Assert.AreEqual("Hello -=Alex=-", actual.Trim());
        }

        [TestMethod]
        public void TestCompileAndRun_IncludeDebuggingForTypedAnonymous_EnabledDebugging()
        {
            var razorEngine = new RazorEngine();
            var template = razorEngine.Compile<TestNameModel>("<h1>Hello @Model.Name</h1>", builder =>
            {
                builder.IncludeDebuggingInfo();
            }, TestContext.CancellationToken);

            template.EnableDebugging();

            string actual = template.Run(new TestNameModel
            {
                Name = "Alex"
            });

            Assert.AreEqual("<h1>Hello Alex</h1>", actual);
        }

        [TestMethod]
        public void TestCompileAndRun_ProjectEngineBuilderAction_IsInvoked()
        {
            var builderActionIsInvoked = false;
            var razorEngine = new RazorEngine();
            var template = razorEngine.Compile<TestNameModel>("<h1>Hello @Model.Name</h1>", builder =>
            {
                builder.Options.TryCache = false;
                builder.IncludeDebuggingInfo();
                builder.ConfigureRazorEngineProject(engineBuilder =>
                {
                    builderActionIsInvoked = true;
                });
            }, TestContext.CancellationToken);

            template.EnableDebugging();

            string actual = template.Run(new TestNameModel
            {
                Name = "Alex"
            });

            Assert.IsTrue(builderActionIsInvoked);
        }

        [TestMethod]
        public void TestCompileAndRun_Typed_EnabledDebuggingThrowsException()
        {
            string templateText = @"
Hello @Model.Decorator(Model.C)
";

            var razorEngine = new RazorEngine();
            var template = razorEngine.Compile<TestModel>(templateText, cancellationToken: TestContext.CancellationToken);

            Assert.Throws<RazorEngineException>(() =>
            {
                template.EnableDebugging();
            });
        }

        [TestMethod]
        public void TestCompileAndRun_Anonymous_EnabledDebuggingThrowsException()
        {
            var razorEngine = new RazorEngine();
            var template = razorEngine.Compile<TestNameModel>("<h1>Hello @Model.Name</h1>", (builder) =>
            {
                builder.Options.TemplateFilename = nameof(TestCompileAndRun_Anonymous_EnabledDebuggingThrowsException);
            }, cancellationToken: TestContext.CancellationToken);

            Assert.Throws<RazorEngineException>(() =>
            {
                template.EnableDebugging();
            });

        }

        [TestMethod]
        public async Task HtmlSafeRenderAsync_Test()
        {
            // Arrange
            var template = """
                <p>@Model.Name, @Html.Raw(Model.Name)</p>
            """;

            var model = new TestNameModel
            {
                Name = "<b>ETSOO</b>"
            };

            // Act
            var razorEngine = new RazorEngine();

            var compiledTemplate = await razorEngine.CompileAsync<TestNameModel>(template, (builder) => {
                builder.Options.IncludeDebuggingInfo = true;
            }, cancellationToken: TestContext.CancellationToken);

            var result = (await compiledTemplate.RunAsync(model)).Trim();

            // Assert
            Assert.AreEqual("<p>&lt;b&gt;ETSOO&lt;/b&gt;, <b>ETSOO</b></p>", result);
        }

        public TestContext TestContext { get; set; }
    }
}
