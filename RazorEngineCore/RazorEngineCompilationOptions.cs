using System.Collections.Generic;
using System.Reflection;
using Microsoft.CodeAnalysis;
using System;
using Microsoft.AspNetCore.Razor.Language;

namespace RazorEngineCore
{
    public class RazorEngineCompilationOptions
    {
        private static readonly HashSet<Assembly> DefaultAssemblies = [
                    typeof(object).Assembly,
                    Assembly.Load(new AssemblyName("Microsoft.CSharp")),
                    typeof(IRazorEngineTemplate).Assembly,
                    typeof(RazorEngineTemplateBase<>).Assembly,
                    Assembly.Load(new AssemblyName("System.Runtime")),
                    typeof(System.Collections.IList).Assembly,
                    typeof(IEnumerable<>).Assembly,
                    Assembly.Load(new AssemblyName("System.Linq")),
                    Assembly.Load(new AssemblyName("System.Linq.Expressions"))
                ];

        public HashSet<Assembly> ReferencedAssemblies { get; init; }

        public HashSet<MetadataReference> MetadataReferences { get; set; } = [];

        public string TemplateNamespace { get; set; } = "TemplateNamespace";

        public string TemplateFilename { get; set; } = "";

        public string Inherits { get; set; } = "RazorEngineCore.RazorEngineTemplateBase";

        ///Set to true to generate PDB symbols information along with the assembly for debugging support
        public bool IncludeDebuggingInfo { get; set; } = false;

        public bool TryCache { get; set; } = true;

        public HashSet<string> DefaultUsings { get; set; } =
        [
            "System.Linq",
            "System.Collections",
            "System.Collections.Generic"
        ];

        public Action<RazorProjectEngineBuilder>? ProjectEngineBuilder { get; set; }

        public RazorEngineCompilationOptions()
        {
            ReferencedAssemblies = [.. DefaultAssemblies];
        }
    }
}