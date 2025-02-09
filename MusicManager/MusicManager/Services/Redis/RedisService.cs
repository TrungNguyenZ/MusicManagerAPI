using MusicManager.Models;
using MusicManager.Repositories;
using MusicManager.Repositories.Data;
using StackExchange.Redis;

namespace MusicManager.Services.Redis
{
    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;

        public RedisService(IConnectionMultiplexer redis)
        {
            _redis = redis;
            _database = _redis.GetDatabase();
        }

        public async Task SetAsync(string key, string value, TimeSpan? expiry = null)
        {
            await _database.StringSetAsync(key, value, expiry);
        }

        public async Task<string?> GetAsync(string key)
        {
            return await _database.StringGetAsync(key);
        }

        public async Task<bool> DeleteAsync(string key)
        {
            return await _database.KeyDeleteAsync(key);
        }
        public  void ClearCacheContaining(string pattern)
        {
            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var db = _redis.GetDatabase();

            var keys = server.Keys(pattern: $"*{pattern}*").ToArray(); // Lấy tất cả key chứa pattern
            if (keys.Length > 0)
            {
                db.KeyDelete(keys); 
            }
        }
    }
}
