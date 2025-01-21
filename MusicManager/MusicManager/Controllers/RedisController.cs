using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicManager.Models;
using MusicManager.Models.Base;
using MusicManager.Services;
using MusicManager.Services.Redis;
using OfficeOpenXml;
using System.Globalization;

namespace MusicManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RedisController : ControllerBase
    {
        private readonly IRedisService _redisService;
        public RedisController(IRedisService redisService)
        {
            _redisService = redisService;
        }
        [HttpGet("{key}")]
        public async Task<IActionResult> Get(string key)
        {
            var value = await _redisService.GetAsync(key);
            if (value == null)
                return NotFound("Key not found.");
            return Ok(value);
        }

        [HttpPost]
        public async Task<IActionResult> Set([FromBody] KeyValuePair<string, string> data)
        {
            await _redisService.SetAsync(data.Key, data.Value, TimeSpan.FromMinutes(30));
            return Ok("Data cached successfully.");
        }

        [HttpDelete("{key}")]
        public async Task<IActionResult> Delete(string key)
        {
            var deleted = await _redisService.DeleteAsync(key);
            if (!deleted)
                return NotFound("Key not found.");
            return Ok("Key deleted successfully.");
        }

    }
}