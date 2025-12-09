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

        Task<string> RunAsync(M model);
    }
}