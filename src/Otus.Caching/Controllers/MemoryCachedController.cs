using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Otus.Caching.Controllers
{
    [Route("date")]
    [ApiController]
    public class MemoryCachedController : ControllerBase
    {
        private const string cacheKey = "MemoryCachedNowController";

        private readonly ILogger<MemoryCachedController> _logger;
        private readonly IMemoryCache _memoryCache;

        public MemoryCachedController(ILogger<MemoryCachedController> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;
        }

        [HttpGet("now")]
        public IActionResult GetNow()
        {
            return Ok(DateTime.Now);
        }

        [HttpGet("now/memory-cached")]
        public IActionResult MemoryCached()
        {
            var cachedDate = _memoryCache.GetOrCreate<DateTime>(cacheKey, entry =>
            {
                var date = DateTime.Now;
                entry.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(5);

                _logger.LogInformation("\r\n *** Requested values (current date:{0})", date);

                return date;
            });

            return Ok(cachedDate);
        }
    }
}
