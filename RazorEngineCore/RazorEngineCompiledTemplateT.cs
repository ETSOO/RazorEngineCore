using System;
using System.IO;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public class RazorEngineCompiledTemplate<T, M> : RazorEngineCompiledTemplateBase, IRazorEngineCompiledTemplate<M> where T : IRazorEngineTemplate
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

        public static IRazorEngineCompiledTemplate<M> LoadFromStream(Stream stream)
        {
            return LoadFromStreamAsync(stream).GetAwaiter().GetResult();
        }
        
        public static async Task<RazorEngineCompiledTemplate<T, M>> LoadFromStreamAsync(Stream stream)
        {
            return new RazorEngineCompiledTemplate<T, M>(await RazorEngineCompiledTemplateMeta.Read(stream));
        }

        public string Run(M model)
        {
            return Execute((template) => template.Model = model);
        }

        public string Execute(Action<IRazorEngineTemplate> action)
        {
            return ExecuteAsync(action).GetAwaiter().GetResult();
        }

        public Task<string> RunAsync(M model)
        {
            return ExecuteAsync((template) => template.Model = model);
        }

        public async Task<string> ExecuteAsync(Action<IRazorEngineTemplate> action)
        {
            var instance = (IRazorEngineTemplate)(Activator.CreateInstance(TemplateType) ?? throw new Exception("Failed to create template instance"));

            action(instance);

            var model = instance.Model;
            if(model != null)
            {
                if (typeof(M) == ObjectExtenders.ObjectType)
                {
                    instance.Model = new AnonymousTypeWrapper(model);
                }
                else
                {
                    instance.Model = model;
                }
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