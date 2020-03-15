using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SampleRedisApp.Models;

namespace SampleRedisApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration configuration;

        public HomeController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.configuration["ApiBaseAddress"]);

                var responseTask = await client.GetAsync("Values/getkey?cacheKey=" + this.configuration["RedisCacheKey"]).ConfigureAwait(false);

                string result = responseTask.Content.ReadAsStringAsync().Result;

                ViewBag.redisMessage = result;
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateKey()
        {
            string message = string.Empty;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.configuration["ApiBaseAddress"]);

                var existingTime = DateTime.UtcNow.ToString();
                
                var responseTask = await client.PostAsJsonAsync("Values/", new KeyValuePair<string, string>(this.configuration["RedisCacheKey"], existingTime)).ConfigureAwait(false);

                if (responseTask.IsSuccessStatusCode)
                {
                    message = existingTime;
                }
            }
            return this.Json(new
            {
                Message = message
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteKey()
        {
            string message = string.Empty;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(this.configuration["ApiBaseAddress"]);

                var responseTask = await client.PostAsJsonAsync("Values/deletekey/", this.configuration["RedisCacheKey"]).ConfigureAwait(false);

                if (responseTask.IsSuccessStatusCode)
                {
                    message = "Item is deleted.";
                }
                else if (responseTask.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    message = "Item couldn't find.";
                }
                else
                {
                    message = "Something is wrong.";
                }
            }
            return this.Json(new
            {
                Message = message
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
