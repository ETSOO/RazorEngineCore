using System;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RazorEngineCore
{
    class RawContent(object value)
    {
        public object Value { get; set; } = value;
    }

    /// <summary>
    /// Razor HTML safe template HTML utilities
    /// Razor HTML安全模板HTML工具
    /// </summary>
    public class RazorEngineHtmlSafeTemplateHtml
    {
        /// <summary>
        /// Raw content
        /// </summary>
        /// <param name="value">Input value</param>
        /// <returns>Result</returns>
        public object Raw(object value)
        {
            return new RawContent(value);
        }
    }

    public abstract class RazorEngineTemplateBase<M> : IRazorEngineTemplate<M>
    {
        /// <summary>
        /// Simulate the Html object in Razor
        /// 模仿Razor中的Html对象
        /// </summary>
        public RazorEngineHtmlSafeTemplateHtml Html { get; } = new();

        private readonly StringBuilder stringBuilder = new();

        private string? attributeSuffix = null;

        /// <summary>
        /// Data model
        /// 数据模型
        /// </summary>
        public required M Model { get; set; }

        public Action Breakpoint { get; set; } = () => { };

        public virtual void WriteLiteral(string? literal = null)
        {
            stringBuilder.Append(literal);
        }

        public virtual void Write(object? obj = null)
        {
            var value = obj is RawContent rawContent
                ? rawContent.Value
                : HttpUtility.HtmlEncode(obj);

            stringBuilder.Append(value);
        }

        public virtual void BeginWriteAttribute(string name, string prefix, int prefixOffset, string suffix, int suffixOffset, int attributeValuesCount)
        {
            attributeSuffix = suffix;
            stringBuilder.Append(prefix);
        }

        public virtual void WriteAttributeValue(string prefix, int prefixOffset, object value, int valueOffset, int valueLength, bool isLiteral)
        {
            stringBuilder.Append(prefix);

            object? fvalue = value is RawContent rawContent
                ? rawContent.Value
                : HttpUtility.HtmlAttributeEncode(value?.ToString());

            stringBuilder.Append(fvalue);
        }

        public virtual void EndWriteAttribute()
        {
            stringBuilder.Append(attributeSuffix);
            attributeSuffix = null;
        }

        public virtual Task ExecuteAsync()
        {
            return Task.CompletedTask;
        }

        public virtual Task<string> ResultAsync()
        {
            return Task.FromResult(stringBuilder.ToString());
        }
	}
}