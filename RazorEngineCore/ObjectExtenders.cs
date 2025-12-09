using System;
using System.IO;
using System.Threading.Tasks;

namespace RazorEngineCore
{
    public static class ObjectExtenders
    {
        public static readonly Type ObjectType = typeof(object);

        public static bool IsAnonymous(this object obj)
        {
            var type = obj.GetType();
            return (type.IsGenericType && type.Name.Contains("AnonymousType") && type.Namespace == null) || type == ObjectType;
        }

        public static async Task<long> ReadLong(this Stream stream)
        {
            var buffer = new byte[8];
            _ = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length));
            return BitConverter.ToInt64(buffer, 0);
        }

        public static async Task WriteLong(this Stream stream, long value)
        {
            var buffer = BitConverter.GetBytes(value);
            await stream.WriteAsync(buffer);
        }
    }
}