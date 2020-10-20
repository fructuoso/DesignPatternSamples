using Microsoft.Extensions.Caching.Distributed;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Abstractions = Microsoft.Extensions.Caching.Distributed;

namespace Workbench.IDistributedCache.Extensions
{
    public static class IDistributedCacheExtensions
    {
        public static TEntity GetOrCreate<TEntity>(this Abstractions.IDistributedCache cache, string key, Func<TEntity> predicate)
        {
            TEntity entity = cache.Get<TEntity>(key);

            if (entity != null) { return entity; }

            entity = predicate();

            cache.Set(key, entity);

            return entity;
        }

        public static TEntity Get<TEntity>(this Abstractions.IDistributedCache cache, string key)
        {
            TEntity result = default;

            var binary = cache.Get(key);

            if (binary != null) { result = binary.FromByteArray<TEntity>(); }

            return result;
        }

        public static void Set<TEntity>(this Abstractions.IDistributedCache cache, string key, TEntity entity)
        {
            var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(20));
            var binary = entity.ToByteArray();
            cache.Set(key, binary, options);
        }

        private static byte[] ToByteArray<TEntity>(this TEntity entity)
        {
            if (entity == null) { return default; }

            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, entity);
                return stream.ToArray();
            }
        }

        private static TEntity FromByteArray<TEntity>(this byte[] data)
        {
            if (data == null) return default;

            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream(data))
            {
                var obj = formatter.Deserialize(stream);
                return (TEntity)obj;
            }
        }

    }
}
