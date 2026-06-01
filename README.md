# RazorEngineCore

.NET 10 Razor Template Engine - A powerful and flexible template engine for compiling and rendering Razor templates at runtime.

Originally forked from [adoconnection/RazorEngineCore](https://github.com/adoconnection/RazorEngineCore).

## Overview

RazorEngineCore is a modern .NET library that enables dynamic compilation and execution of Razor templates at runtime. It provides a simple yet powerful API for generating dynamic content using the familiar Razor syntax.

## Key Features

### 🚀 **Dynamic Template Compilation**

- **Runtime Compilation**: Compile Razor templates at runtime without pre-compilation
- **Async/Sync Support**: Both synchronous (`Compile`) and asynchronous (`CompileAsync`) compilation methods
- **Strongly-Typed Models**: Full IntelliSense support with generic type parameters
- **Dynamic Models**: Flexible runtime type support with `dynamic` keyword

### ⚡ **High Performance**

- **Built-in Caching**: Automatic template caching mechanism for improved performance
- **Roslyn-Powered**: Efficient compilation using Microsoft.CodeAnalysis
- **Optimized Execution**: Minimal memory footprint with streamlined template rendering
- **Cache Control**: Manual cache management with `ClearCache()` method

### 🎯 **Model Support**

- **Generic Models**: Strongly-typed models with `Compile<TModel>(template)`
- **Dynamic Models**: Runtime flexibility with dynamic type support
- **Complex Types**: Support for nested models, collections, and complex objects
- **Anonymous Types**: Works seamlessly with anonymous objects
- **@model Directives**: Standard Razor `@model` directive support

### 🔧 **Advanced Customization**

- **Custom Assembly References**: Add external assemblies via `AddAssemblyReference()` and `AddAssemblyReferenceByName()`
- **Configurable Namespaces**: Custom template namespace configuration
- **Template Metadata**: Control template filename and metadata
- **Debugging Support**: Optional debugging information inclusion
- **Razor Engine Builder**: Extensible project engine configuration
- **Cancellation Support**: CancellationToken support for long-running operations

### 💾 **Template Serialization**

- **Save to Disk**: Persist compiled templates to files for reuse
- **Load from Stream/File**: Load pre-compiled templates from disk or streams
- **Faster Startup**: Reduce initialization time by loading pre-compiled templates
- **Portable Templates**: Share compiled templates across applications

### 🎨 **Razor Features**

- **HTML Encoding by Default**: Automatic HTML encoding for security
- **@Html.Raw Support**: Output unencoded HTML when needed
- **Major Razor Syntax**: Major Razor syntax support (loops, conditionals, helpers)
- **Custom Directives**: Add custom Razor directives as needed

### 🛠️ **MSBuild Integration**

- **Build Tasks**: Includes MSBuild tasks (`CompileTemplatesTask`)
- **Build Targets**: Custom targets file for automated template processing

## Installation

Install via NuGet Package Manager:

```bash
Install-Package Etsoo.RazorEngineCore
```

Or via .NET CLI:

```bash
dotnet add package Etsoo.RazorEngineCore
```

## Quick Start

### Basic Usage

```csharp
using RazorEngineCore;

// Create engine instance
var razorEngine = new RazorEngine();

// Define your model
var model = new { Name = "World" };

// Compile template
var template = razorEngine.Compile<dynamic>("Hello @Model.Name!");

// Run template
string result = template.Run(model);
// Output: "Hello World!"
```

### Strongly-Typed Models

```csharp
public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }
}

var razorEngine = new RazorEngine();
var template = razorEngine.Compile<Person>(@"
    <div>
        <h1>@Model.Name</h1>
        <p>Age: @Model.Age</p>
    </div>
");

var result = template.Run(new Person { Name = "John", Age = 30 });
```

### Async Compilation and Execution

```csharp
var razorEngine = new RazorEngine();

// Async compilation
var template = await razorEngine.CompileAsync<Person>("Hello @Model.Name!");

// Async execution
var result = await template.RunAsync(new Person { Name = "Alice" });
```

### HTML Encoding and Raw Output

```csharp
var razorEngine = new RazorEngine();
var template = razorEngine.Compile<dynamic>(@"
    <div>@Model.Content</div>
    <div>@Html.Raw(Model.HtmlContent)</div>
");

var result = template.Run(new
{
    Content = "<script>alert('xss')</script>",  // Encoded automatically
    HtmlContent = "<strong>Safe HTML</strong>"   // Rendered as-is with @Html.Raw
});
```

### Custom Assembly References

```csharp
var razorEngine = new RazorEngine();
var template = razorEngine.Compile<MyModel>("@Model.CustomMethod()", builder =>
{
    builder.AddAssemblyReferenceByName("MyCustomLibrary");
    builder.AddAssemblyReference(typeof(MyCustomType));
});
```

### Template Caching

```csharp
// Caching is enabled by default
var razorEngine = new RazorEngine();

// First compilation - template is cached
var template1 = razorEngine.Compile<Person>("Hello @Model.Name!");

// Second compilation - retrieved from cache (much faster)
var template2 = razorEngine.Compile<Person>("Hello @Model.Name!");

// Clear cache if needed
RazorEngine.ClearCache();
```

### Save and Load Compiled Templates

```csharp
// Compile and save
var razorEngine = new RazorEngine();
var compiledMeta = razorEngine.CompileMeta<Person>("Hello @Model.Name!");
await compiledMeta.SaveToFileAsync("template.dll");

// Load pre-compiled template (fast startup)
var template = await RazorEngineCompiledTemplate<Person>.LoadFromFileAsync("template.dll");
var result = template.Run(new Person { Name = "Bob" });
```

## Advanced Configuration

```csharp
var razorEngine = new RazorEngine();
var template = razorEngine.Compile<MyModel>(templateContent, builder =>
{
    // Add custom assembly references
    builder.AddAssemblyReference(typeof(MyType));
    builder.AddAssemblyReferenceByName("System.Linq");

    // Configure template options
    builder.Options.TemplateNamespace = "MyApp.Templates";
    builder.Options.TemplateFilename = "MyTemplate.cshtml";
    builder.Options.IncludeDebuggingInfo = true;
    builder.Options.TryCache = true; // Enable caching
});
```

## Enable MSBuild Integration

To enable MSBuild integration, make sure the project configuration looks like:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

    <PropertyGroup>
        <OutputType>Library</OutputType>
    </PropertyGroup>

    <ItemGroup>
        <!-- Install the package -->
        <PackageReference Include="Etsoo.RazorEngineCore" Version="x.y.z" />
    </ItemGroup>

    <PropertyGroup>
        <!-- Enable automatic template compilation before build -->
        <RazorEngineCore_EnableCompileTemplates>true</RazorEngineCore_EnableCompileTemplates>
        <RazorEngineCore_TemplateDir>$(MSBuildProjectDirectory)\Templates</RazorEngineCore_TemplateDir>
    </PropertyGroup>

</Project>
```

Then you can preview \*.cshtml files but output a dll file including all compiled templates.

Add template '\*.cshtml' files under "Templates" directory. A utility class to access the compiled template in the project looks like:

```csharp
public static class TemplateUtils
{
    private static readonly ConcurrentDictionary<string, RazorEngineCompiledTemplate<object>?> _cache = new();

    public static RazorEngineCompiledTemplate<object>? Get(string template)
    {
        return _cache.GetOrAdd(template, k =>
        {
            var assembly = Assembly.GetExecutingAssembly();

            var names = assembly.GetManifestResourceNames();

            // WebTemplates is the project's name
            var resourceName = $"WebTemplates.Templates.{template.Replace('/', '.')}.bin";
            if (!names.Contains(resourceName))
            {
                return null;
            }

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                return null;
            }

            return RazorEngineCompiledTemplate<object>.LoadFromStream(stream);
        });
    }
}
```

## Requirements

- **.NET 10.0** or later
- **Microsoft.AspNetCore.Razor.Language** 6.0.36+
- **Microsoft.CodeAnalysis.CSharp** 5.3.0+

## Support

For issues, questions, or contributions, please visit the [GitHub repository](https://github.com/ETSOO/RazorEngineCore).
