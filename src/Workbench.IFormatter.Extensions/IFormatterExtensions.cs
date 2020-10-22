using System.IO;
using Serialization = System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Workbench.IFormatter.Extensions
{

    public static class IFormatterExtensions
    {
        public static Serialization.IFormatter DefaultFormatter { get; set; } = new BinaryFormatter();

        public static byte[] Serialize<TEntity>(this TEntity entity) => entity.Serialize(DefaultFormatter);
        public static byte[] Serialize<TEntity>(this TEntity entity, Serialization.IFormatter formatter)
        {
            if (entity == null) { return default; }

            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, entity);
                return stream.ToArray();
            }
        }

        public static TEntity Deserialize<TEntity>(this byte[] data) => data.Deserialize<TEntity>(DefaultFormatter);
        public static TEntity Deserialize<TEntity>(this byte[] data, Serialization.IFormatter formatter)
        {
            if (data == null) return default;

            using (MemoryStream stream = new MemoryStream(data))
            {
                var obj = formatter.Deserialize(stream);
                return (TEntity)obj;
            }
        }
    }
}