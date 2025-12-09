using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public abstract class RazorEngineCompiledTemplateBase
    {
        protected RazorEngineCompiledTemplateMeta Meta { get; init; }

        protected Type TemplateType { get; init; }

        protected bool IsDebuggerEnabled { get; set; }

        internal RazorEngineCompiledTemplateBase(RazorEngineCompiledTemplateMeta meta)
        {
            Meta = meta;

            var assembly = Assembly.Load(meta.AssemblyByteCode, meta.PdbByteCode);
            TemplateType = assembly.GetType(meta.TemplateNamespace + ".Template") ?? throw new Exception("Failed to load template type");
        }

        public void SaveToFile(string fileName)
        {
            SaveToFileAsync(fileName).GetAwaiter().GetResult();
        }

        public async Task SaveToFileAsync(string fileName)
        {
            await using var fileStream = new FileStream(
                       path: fileName,
                       mode: FileMode.OpenOrCreate,
                       access: FileAccess.Write,
                       share: FileShare.None,
                       bufferSize: 4096,
                       useAsync: true);

            await SaveToStreamAsync(fileStream);
        }

        public void SaveToStream(Stream stream)
        {
            SaveToStreamAsync(stream).GetAwaiter().GetResult();
        }

        public Task SaveToStreamAsync(Stream stream)
        {
            return Meta.Write(stream);
        }

        public void EnableDebugging(string? debuggingOutputDirectory = null)
        {
            if (Meta.PdbByteCode == null || Meta.PdbByteCode.Length == 0 || string.IsNullOrWhiteSpace(this.Meta.TemplateSource))
            {
                throw new RazorEngineException("No debugging info available, compile template with builder.IncludeDebuggingInfo(); option");
            }

            File.WriteAllText(Path.Combine(debuggingOutputDirectory ?? ".", Meta.TemplateFileName), Meta.TemplateSource);

            IsDebuggerEnabled = true;
        }
    }
}