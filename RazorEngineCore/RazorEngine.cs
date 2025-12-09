using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection.Metadata;

namespace RazorEngineCore
{
    public class RazorEngine : IRazorEngine
    {
        public IRazorEngineCompiledTemplate<T, M> Compile<T, M>(string content, Action<IRazorEngineCompilationOptionsBuilder>? builderAction = null, CancellationToken cancellationToken = default) where T : IRazorEngineTemplate<M>
        {
            var compilationOptionsBuilder = new RazorEngineCompilationOptionsBuilder();
            compilationOptionsBuilder.AddAssemblyReference(typeof(T).Assembly);
            compilationOptionsBuilder.Inherits(typeof(T));

            builderAction?.Invoke(compilationOptionsBuilder);

            var meta = CreateAndCompileToStream<M>(content, compilationOptionsBuilder, cancellationToken);

            return new RazorEngineCompiledTemplate<T, M>(meta);
        }

        public Task<IRazorEngineCompiledTemplate<T, M>> CompileAsync<T, M>(string content, Action<IRazorEngineCompilationOptionsBuilder>? builderAction = null, CancellationToken cancellationToken = default) where T : IRazorEngineTemplate<M>
        {
            return Task.Run(() => Compile<T, M>(content: content, builderAction: builderAction, cancellationToken: cancellationToken));
        }

        public IRazorEngineCompiledTemplate<M> Compile<M>(string content, Action<IRazorEngineCompilationOptionsBuilder>? builderAction = null, CancellationToken cancellationToken = default)
        {
            var compilationOptionsBuilder = new RazorEngineCompilationOptionsBuilder();
            compilationOptionsBuilder.Inherits(typeof(RazorEngineTemplateBase<M>));

            builderAction?.Invoke(compilationOptionsBuilder);
 
            var meta = CreateAndCompileToStream<M>(content, compilationOptionsBuilder, cancellationToken);
            return new RazorEngineCompiledTemplate<M>(meta);
        }

        public Task<IRazorEngineCompiledTemplate<M>> CompileAsync<M>(string content, Action<IRazorEngineCompilationOptionsBuilder>? builderAction = null, CancellationToken cancellationToken = default)
        {
            return Task.Run(() => Compile<M>(
                content, 
                builderAction, 
                cancellationToken));
        }

        protected virtual RazorEngineCompiledTemplateMeta CreateAndCompileToStream<M>(string templateSource, RazorEngineCompilationOptionsBuilder builder, CancellationToken cancellationToken)
        {
            builder.AddAssemblyReference(typeof(RazorEngineTemplateBase<M>).Assembly);

            var options = builder.Options;

            templateSource = WriteDirectives(templateSource, options);

            var projectPath = @".";
            var fileName = string.IsNullOrWhiteSpace(options.TemplateFilename) 
                ? Path.GetRandomFileName() + ".cshtml" 
                : options.TemplateFilename;

            var engine = RazorProjectEngine.Create(
                RazorConfiguration.Default,
                RazorProjectFileSystem.Create(projectPath),
                (builder) =>
                {
                    builder.SetNamespace(options.TemplateNamespace);
                    options.ProjectEngineBuilder?.Invoke(builder);
                });

            var document = RazorSourceDocument.Create(templateSource, fileName);

            var codeDocument = engine.Process(
                document,
                null,
                [],
                []);
            
            var razorCSharpDocument = codeDocument.GetCSharpDocument();
            var syntaxTree = CSharpSyntaxTree.ParseText(razorCSharpDocument.GeneratedCode, cancellationToken: cancellationToken);

            var compilation = CSharpCompilation.Create(
                fileName,
                [
                    syntaxTree
                ],
                [
                    .. options.ReferencedAssemblies
                       .Select(ass =>
                       {
                           unsafe
                           {
                               ass.TryGetRawMetadata(out byte* blob, out int length);
                               ModuleMetadata moduleMetadata = ModuleMetadata.CreateFromMetadata((IntPtr)blob, length);
                               AssemblyMetadata assemblyMetadata = AssemblyMetadata.Create(moduleMetadata);
                               PortableExecutableReference metadataReference = assemblyMetadata.GetReference();

                               return metadataReference;
                           }
                       })
,
                    .. options.MetadataReferences,
                ],
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                    .WithOptimizationLevel(OptimizationLevel.Release)
                    .WithOverflowChecks(true));

            var assemblyStream = new MemoryStream();
            var pdbStream = options.IncludeDebuggingInfo ? new MemoryStream() : null;

            var emitResult = compilation.Emit(assemblyStream, pdbStream, cancellationToken: cancellationToken);

            if (!emitResult.Success)
            {
                var exception = new RazorEngineCompilationException()
                {
                    Errors = [.. emitResult.Diagnostics],
                    GeneratedCode = razorCSharpDocument.GeneratedCode
                };

                throw exception;
            }

            return new RazorEngineCompiledTemplateMeta()
            {
                AssemblyByteCode = assemblyStream.ToArray(),
                PdbByteCode = pdbStream?.ToArray(),
                GeneratedSourceCode = options.IncludeDebuggingInfo ? razorCSharpDocument.GeneratedCode : null,
                TemplateSource = options.IncludeDebuggingInfo ? templateSource : null,
                TemplateNamespace = options.TemplateNamespace,
                TemplateFileName = fileName
            };
        }

        protected virtual string WriteDirectives(string content, RazorEngineCompilationOptions options)
        {
            var stringBuilder = new StringBuilder();

            // For dynamic object, replace System.Object with dynamic to pass the build.
            stringBuilder.AppendLine($"@inherits {options.Inherits.Replace("<System.Object>", "<dynamic>")}");

            foreach (var entry in options.DefaultUsings)
            {
                stringBuilder.AppendLine($"@using {entry}");
            }

            stringBuilder.Append(content);

            return stringBuilder.ToString();
        }
    }
}