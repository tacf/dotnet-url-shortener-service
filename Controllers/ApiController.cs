using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace net_url_shortner.Controllers
{

    [Route("api")]
    public class ApiController : Controller
    {
        private IDistributedCache _cache;

        public ApiController(IDistributedCache memoryCache)
        {
            _cache = memoryCache;
        }

        
        [HttpGet]
        public string Index()
        {
            return "Welcome to Url Shortener Api";
        }


        [HttpGet("{id}")]
        public IActionResult GetUrl(string id){
            var value = _cache.Get(id);
            var url = "";
            if (value != null)
            {
                url = Encoding.UTF8.GetString(value);
            } else {
                // TODO: Change to landing page Redirect
                return BadRequest();
            }
        
            return url.Contains("://")? RedirectPermanent(url): RedirectPermanent("http://"+url);
        }

        [HttpPost("create")]
        public IActionResult ShortenUrl([FromBody] string url){
            if (url == null){
                return BadRequest();
            }
            string hashCode = String.Format("{0:X}", (url+DateTime.Now.ToString()).GetHashCode());
               // Look for cache key.
            var value = _cache.Get(hashCode);
            if (value != null){
                return BadRequest("Hash Collision!!");
            }

            _cache.Set(hashCode, Encoding.UTF8.GetBytes(url));

            return Ok(hashCode);
        }

    }
}
