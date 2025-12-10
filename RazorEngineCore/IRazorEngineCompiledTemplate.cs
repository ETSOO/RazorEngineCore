using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public interface IRazorEngineCompiledTemplate<M>
    {
        void SaveToStream(Stream stream);

        Task SaveToStreamAsync(Stream stream);

        void SaveToFile(string fileName);

        Task SaveToFileAsync(string fileName);

        void EnableDebugging(string? debuggingOutputDirectory = null);

        string Run(M model);

        string Execute(Action<IRazorEngineTemplate> action);

        Task<string> RunAsync(M model);

        Task<string> ExecuteAsync(Action<IRazorEngineTemplate> action);
    }
}