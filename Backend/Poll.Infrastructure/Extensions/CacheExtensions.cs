using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Poll.Infrastructure.Extensions;

public static class CacheExtensions
{
    public static async Task<T?> GetValue<T>(this IDistributedCache cache, string key, CancellationToken ct)
    {
        try
        {
            var value = await cache.GetStringAsync(key, ct);
            _ = cache.RefreshAsync(key, ct);
            if (string.IsNullOrEmpty(value))
            {
                return default;
            }

            if (typeof(T) == typeof(string) && value is T val)
            {
                return val;
            }

            var obj = JsonConvert.DeserializeObject<T>(value);
            return obj;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return default;
    }

    public static async Task SetValue<T>(this IDistributedCache cache, string key, T value, uint minutesLifeTime, CancellationToken ct)
    {
        try
        {
            if (typeof(T) != typeof(string))
            {
                await cache.SetStringAsync(key, JsonConvert.SerializeObject(value),
                    new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(minutesLifeTime) },
                    ct);
            }
            else
            {
                await cache.SetStringAsync(key, value.ToString(), ct);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}