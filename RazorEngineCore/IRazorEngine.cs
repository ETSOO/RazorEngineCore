using System;
using System.Threading;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public interface IRazorEngine
    {
        IRazorEngineCompiledTemplate<T, M> Compile<T, M>(string content, Action<IRazorEngineCompilationOptionsBuilder>? builderAction = null, CancellationToken cancellationToken = default)
            where T : IRazorEngineTemplate<M>;

        Task<IRazorEngineCompiledTemplate<T, M>> CompileAsync<T, M>(string content, Action<IRazorEngineCompilationOptionsBuilder>? builderAction = null, CancellationToken cancellationToken = default)
            where T : IRazorEngineTemplate<M>;

        IRazorEngineCompiledTemplate<M> Compile<M>(string content, Action<IRazorEngineCompilationOptionsBuilder>? builderAction = null, CancellationToken cancellationToken = default);

        Task<IRazorEngineCompiledTemplate<M>> CompileAsync<M>(string content, Action<IRazorEngineCompilationOptionsBuilder>? builderAction = null, CancellationToken cancellationToken = default);
    }
}