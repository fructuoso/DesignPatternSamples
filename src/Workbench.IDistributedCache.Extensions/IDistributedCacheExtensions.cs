using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading.Tasks;
using Workbench.IFormatter.Extensions;
using Abstractions = Microsoft.Extensions.Caching.Distributed;

namespace Workbench.IDistributedCache.Extensions
{
    public static class IDistributedCacheExtensions
    {
        public static TEntity GetOrCreate<TEntity>(this Abstractions.IDistributedCache cache, string key, Func<TEntity> predicate, int ttl)
        {
            TEntity entity = cache.Get<TEntity>(key);

            if (entity != null) { return entity; }

            entity = predicate();

            cache.Set(key, entity, ttl);

            return entity;
        }
        public static async Task<TEntity> GetOrCreateAsync<TEntity>(this Abstractions.IDistributedCache cache, string key, Func<Task<TEntity>> predicate, int ttl)
        {
            TEntity entity = await cache.GetAsync<TEntity>(key);

            if (entity != null) { return entity; }

            entity = await predicate();

            await cache.SetAsync(key, entity, ttl);

            return entity;
        }

        public static TEntity Get<TEntity>(this Abstractions.IDistributedCache cache, string key)
        {
            TEntity result = default;

            var binary = cache.Get(key);

            if (binary != null) result = binary.Deserialize<TEntity>();

            return result;
        }
        public static async Task<TEntity> GetAsync<TEntity>(this Abstractions.IDistributedCache cache, string key)
        {
            TEntity result = default;

            var binary = await cache.GetAsync(key);

            if (binary != null) result = binary.Deserialize<TEntity>();

            return result;
        }

        public static void Set<TEntity>(this Abstractions.IDistributedCache cache, string key, TEntity entity, int ttl)
        {
            var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(ttl));
            var binary = entity.Serialize();
            cache.Set(key, binary, options);
        }
        public static Task SetAsync<TEntity>(this Abstractions.IDistributedCache cache, string key, TEntity entity, int ttl)
        {
            var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(ttl));
            var binary = entity.Serialize();
            return cache.SetAsync(key, binary, options);
        }
    }
}
