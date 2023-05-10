using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace Otus.Caching.Controllers
{
    [Route("date/distributed")]
    [ApiController]
    public class DistributedCacheController : ControllerBase
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<DistributedCacheController> _logger;

        public DistributedCacheController(ILogger<DistributedCacheController> logger, IDistributedCache distributedCache)
        {
            _logger = logger;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            const string nowKey = "DistributedCacheController";

            string serialized = await _distributedCache.GetStringAsync(nowKey);
            if (serialized != null)
            {
                _logger.LogInformation("From redis");
                return Ok(JsonSerializer.Deserialize<DateTime>(serialized));
            }

            _logger.LogInformation("Current date is requested");
            var response = DateTime.Now;

            await _distributedCache.SetStringAsync(
                key: nowKey,
                value: JsonSerializer.Serialize(response),
                options: new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromSeconds(10)
                });

            return Ok(response);
        }
    }
}