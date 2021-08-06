using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Otus.Caching.Controllers
{
    [Route("date")]
    [ApiController]
    public class ResponseHeaderCachedController : ControllerBase
    {
        private readonly ILogger<ResponseHeaderCachedController> _logger;

        public ResponseHeaderCachedController(ILogger<ResponseHeaderCachedController> logger)
        {
            _logger = logger;
        }

        [HttpGet("now/response-cached")]
        [ResponseCache(Location = ResponseCacheLocation.Any, Duration = 10)]
        public IActionResult ResponseCached()
        {
            return new DateTimeActionResult(_logger, DateTimeOffset.Now);
        }

        [HttpGet("now/response-vary-cached")]
        [ResponseCache(Location = ResponseCacheLocation.Any, VaryByHeader = "User-Agent", Duration = 120)]
        public IActionResult ResponseCachedUserAgent()
        {
            return new DateTimeActionResult(_logger, DateTimeOffset.Now);
        }

        [HttpGet("now/response-no-cached")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult ResponseNoCached()
        {
            return new DateTimeActionResult(_logger, DateTimeOffset.Now);
        }
    }

    internal class DateTimeActionResult : OkObjectResult
    {
        public DateTimeActionResult(ILogger<ResponseHeaderCachedController> logger, DateTimeOffset now) : base(now)
        {
            logger.LogInformation("\r\n *** Requested date: {0}", now);
        }
    }
}