namespace RazorEngineCore
{
    public class RazorEngineCompiledTemplate<M> : RazorEngineCompiledTemplate<RazorEngineTemplateBase<M>, M>
    {
        public RazorEngineCompiledTemplate(RazorEngineCompiledTemplateMeta meta) : base(meta)
        {
        }
    }
}