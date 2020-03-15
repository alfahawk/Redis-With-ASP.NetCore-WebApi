using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace SampleRedisApp
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private readonly IDistributedCache _distributedCache;

        public ValuesController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        [HttpGet("getkey")]
        public async Task<string> Get([FromQuery]string cacheKey)
        {
            var existingTime = await _distributedCache.GetStringAsync(cacheKey).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(existingTime))
            {
                return "Fetched from cache: " + existingTime;
            }
            else
            {
                return "No item exist";
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] KeyValuePair<string, string> keyValue)
        {
            await _distributedCache.SetStringAsync(keyValue.Key, keyValue.Value).ConfigureAwait(false);
            return this.Ok();
        }

        [HttpPost("deletekey")]
        public async Task<IActionResult> Delete([FromBody]string cacheKey)
        {
            var existingTime = await _distributedCache.GetStringAsync(cacheKey).ConfigureAwait(false);

            if (!string.IsNullOrEmpty(existingTime))
            {
                await _distributedCache.RemoveAsync(cacheKey).ConfigureAwait(false);

                return this.Ok();
            }
            else
            {
                return this.NotFound();
            }
        }
    }
}
