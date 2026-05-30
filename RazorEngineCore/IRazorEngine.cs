using System;
using System.Threading;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public interface IRazorEngine
    {
        IRazorEngineCompiledTemplate<M> Compile<M>(string content, Action<IRazorEngineCompilationOptionsBuilder>? builderAction = null, CancellationToken cancellationToken = default);

        Task<IRazorEngineCompiledTemplate<M>> CompileAsync<M>(string content, Action<IRazorEngineCompilationOptionsBuilder>? builderAction = null, CancellationToken cancellationToken = default);
    }
}