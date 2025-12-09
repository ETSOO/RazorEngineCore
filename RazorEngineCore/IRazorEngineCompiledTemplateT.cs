using System;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public interface IRazorEngineCompiledTemplate<T, M> : IRazorEngineCompiledTemplate<M> where T : IRazorEngineTemplate<M>
    {
        string Run(Action<T> initializer);

        Task<string> RunAsync(Action<T> initializer);
    }
}