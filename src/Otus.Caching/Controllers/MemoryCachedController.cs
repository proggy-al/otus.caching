using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Otus.Caching.Controllers
{
    [Route("date")]
    [ApiController]
    public class MemoryCachedController : ControllerBase
    {
        #region
        //double-ckeck locking
        private readonly static object _sync = new();
        #endregion

        private const string cacheKey = "MemoryCachedController";

        private readonly ILogger<MemoryCachedController> _logger;
        private readonly IMemoryCache _memoryCache;

        public MemoryCachedController(ILogger<MemoryCachedController> logger,
            IMemoryCache memoryCache
            )
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
            if (_memoryCache.TryGetValue(cacheKey, out var memoryValue))
            {
                return Ok(memoryValue);
            }

            #region
            //double-check locking
            lock (_sync)
            {
            #endregion

            var cachedDate = _memoryCache.GetOrCreate<DateTime>(cacheKey, entry =>
            {
                var date = DateTime.Now;
                entry.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(50);
                //entry.SlidingExpiration = TimeSpan.FromSeconds(5);
                //entry.Priority = CacheItemPriority.Low;
                //entry.Size = 10;

                //entry.ExpirationTokens.Add(new RandomTimerChangeToken());

                //Thread.Sleep(3000);

                _logger.LogInformation("\r\n *** Requested values (current date:{0})", date);

                return date;
            });

            return Ok(cachedDate);
        }
        #region
            }
        #endregion
    }
}