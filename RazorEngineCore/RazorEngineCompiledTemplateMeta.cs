using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public class RazorEngineCompiledTemplateMeta
    {
        public byte[]? AssemblyByteCode { get; set; }

        public byte[]? PdbByteCode { get; set; }

        public string? GeneratedSourceCode { get; set; }

        public required string TemplateNamespace { get; set; }

        public string? TemplateSource { get; set; }

        public string? TemplateFileName { get; set; }

        public async Task Write(Stream stream)
        {
            await stream.WriteLong(10001);

            await WriteBuffer(stream, AssemblyByteCode);
            await WriteBuffer(stream, PdbByteCode);
            await WriteString(stream, GeneratedSourceCode);
            await WriteString(stream, TemplateSource);
            await WriteString(stream, TemplateNamespace);
            await WriteString(stream, TemplateFileName);
        }

        public static async Task<RazorEngineCompiledTemplateMeta> Read(Stream stream)
        {
            long version = await stream.ReadLong();

            if (version == 10001)
            {
                return await LoadVersion1(stream);
            }

            throw new RazorEngineException("Unable to load template: wrong version");
        }

        private static async Task<RazorEngineCompiledTemplateMeta> LoadVersion1(Stream stream)
        {
            return new RazorEngineCompiledTemplateMeta()
            {
                AssemblyByteCode = await ReadBuffer(stream),
                PdbByteCode = await ReadBuffer(stream),
                GeneratedSourceCode = await ReadString(stream),
                TemplateSource = await ReadString(stream),
                TemplateNamespace = await ReadString(stream) ?? "TemplateNamespace",
                TemplateFileName = await ReadString(stream),
            };
        }

        private Task WriteString(Stream stream, string? value)
        {
            var buffer = value == null ? null : Encoding.UTF8.GetBytes(value);
            return WriteBuffer(stream, buffer);
        }

        private async Task WriteBuffer(Stream stream, byte[]? buffer)
        {
            if (buffer == null)
            {
                await stream.WriteLong(0);
                return;
            }

            await stream.WriteLong(buffer.Length);
            await stream.WriteAsync(buffer);
        }

        private static async Task<string?> ReadString(Stream stream)
        {
            var buffer = await ReadBuffer(stream);
            return buffer == null ? null : Encoding.UTF8.GetString(buffer);
        }

        private static async Task<byte[]?> ReadBuffer(Stream stream)
        {
            long length = await stream.ReadLong();

            if (length == 0)
            {
                return null;
            }

            byte[] buffer = new byte[length];
            _ = await stream.ReadAsync(buffer);
            return buffer;
        }
    }
}
