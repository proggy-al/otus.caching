using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Otus.Caching.Controllers
{
    [Route("date/distributed")]
    [ApiController]
    public class DistributedCacheController : ControllerBase
    {
        private const string redisKey = "DistributedCacheController";

        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<DistributedCacheController> _logger;

        public DistributedCacheController(ILogger<DistributedCacheController> logger, 
            IDistributedCache distributedCache)
        {
            _logger = logger;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string serializedValue = await _distributedCache.GetStringAsync(redisKey);
            if (serializedValue != null)
            {
                _logger.LogInformation("From Redis");
                return Ok(JsonSerializer.Deserialize<DateTime>(serializedValue));
            }

            _logger.LogInformation("Current date is requested");
            var response = DateTime.Now;

            await _distributedCache.SetStringAsync(
                key: redisKey,
                value: JsonSerializer.Serialize(response),
                options: new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromSeconds(30)
                });

            return Ok(response);
        }
    }
}