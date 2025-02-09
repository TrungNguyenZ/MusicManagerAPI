using MusicManager.Models;
using StackExchange.Redis;

namespace MusicManager.Services.Redis
{
    public interface IRedisService
    {
        Task SetAsync(string key, string value, TimeSpan? expiry = null);
        Task<string?> GetAsync(string key);
        Task<bool> DeleteAsync(string key);
        void ClearCacheContaining(string pattern);
    }
}
 