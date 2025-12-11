using System;
using System.IO;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public interface IRazorEngineCompiledTemplate<T, M> where T : IRazorEngineTemplate
    {
        void SaveToStream(Stream stream);

        Task SaveToStreamAsync(Stream stream);

        void SaveToFile(string fileName);

        Task SaveToFileAsync(string fileName);

        void EnableDebugging(string? debuggingOutputDirectory = null);

        string Run(M model);

        string Execute(Action<T> action);

        Task<string> RunAsync(M model);

        Task<string> ExecuteAsync(Action<T> action);
    }
}