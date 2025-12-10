using System;
using System.Threading;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public interface IRazorEngine
    {
        IRazorEngineCompiledTemplate<M> Compile<T, M>(string content, Action<IRazorEngineCompilationOptionsBuilder>? builderAction = null, CancellationToken cancellationToken = default)
            where T : IRazorEngineTemplate;

        Task<IRazorEngineCompiledTemplate<M>> CompileAsync<T, M>(string content, Action<IRazorEngineCompilationOptionsBuilder>? builderAction = null, CancellationToken cancellationToken = default)
            where T : IRazorEngineTemplate;

        IRazorEngineCompiledTemplate<M> Compile<M>(string content, Action<IRazorEngineCompilationOptionsBuilder>? builderAction = null, CancellationToken cancellationToken = default);

        Task<IRazorEngineCompiledTemplate<M>> CompileAsync<M>(string content, Action<IRazorEngineCompilationOptionsBuilder>? builderAction = null, CancellationToken cancellationToken = default);
    }
}