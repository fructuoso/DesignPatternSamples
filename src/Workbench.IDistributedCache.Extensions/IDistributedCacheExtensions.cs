using Microsoft.Extensions.Caching.Distributed;
using System;
using Workbench.IFormatter.Extensions;
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

            if (binary != null) { result = binary.Deserialize<TEntity>(); }

            return result;
        }
        public static void Set<TEntity>(this Abstractions.IDistributedCache cache, string key, TEntity entity)
        {
            var options = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(20));
            var binary = entity.Serialize();
            cache.Set(key, binary, options);
        }
    }
}
