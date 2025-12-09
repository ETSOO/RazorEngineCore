using System;
using System.IO;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public class RazorEngineCompiledTemplate<T, M> : RazorEngineCompiledTemplateBase, IRazorEngineCompiledTemplate<T, M> where T : IRazorEngineTemplate<M>
    {
        public RazorEngineCompiledTemplate(RazorEngineCompiledTemplateMeta meta) : base(meta)
        {
        }

        public static RazorEngineCompiledTemplate<T, M> LoadFromFile(string fileName)
        {
            return LoadFromFileAsync(fileName).GetAwaiter().GetResult();
        }
        
        public static async Task<RazorEngineCompiledTemplate<T, M>> LoadFromFileAsync(string fileName)
        {
            await using var fileStream = new FileStream(
                             path: fileName,
                             mode: FileMode.Open,
                             access: FileAccess.Read,
                             share: FileShare.None,
                             bufferSize: 4096,
                             useAsync: true);

            return await LoadFromStreamAsync(fileStream);
        }

        public static IRazorEngineCompiledTemplate<T, M> LoadFromStream(Stream stream)
        {
            return LoadFromStreamAsync(stream).GetAwaiter().GetResult();
        }
        
        public static async Task<RazorEngineCompiledTemplate<T, M>> LoadFromStreamAsync(Stream stream)
        {
            return new RazorEngineCompiledTemplate<T, M>(await RazorEngineCompiledTemplateMeta.Read(stream));
        }

        public string Run(M model)
        {
            return Run((obj) => obj.Model = model);
        }

        public string Run(Action<T> initializer)
        {
            return RunAsync(initializer).GetAwaiter().GetResult();
        }

        public Task<string> RunAsync(M model)
        {
            return RunAsync((obj) => obj.Model = model);
        }

        public async Task<string> RunAsync(Action<T> initializer)
        {
            var instance = (T)(Activator.CreateInstance(TemplateType) ?? throw new Exception("Failed to create template instance"));
            initializer(instance);

            if (instance.Model != null && typeof(M) == ObjectExtenders.ObjectType)
            {
                instance.Model = (dynamic)(new AnonymousTypeWrapper(instance.Model));
            }

            if (IsDebuggerEnabled && instance is T instance2)
            {
                instance2.Breakpoint = System.Diagnostics.Debugger.Break;
            }

            await instance.ExecuteAsync();

            return await instance.ResultAsync();
		}
    }
}