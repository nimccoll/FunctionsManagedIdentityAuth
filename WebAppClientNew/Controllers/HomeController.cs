using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebAppClientNew.Models;

namespace WebAppClientNew.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private HttpClient _httpClient;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> CallFunction()
        {
            string apiUrl = _configuration.GetValue<string>("APIUrl");
            string resource = _configuration.GetValue<string>("APIScope");
            DefaultAzureCredential credential = new DefaultAzureCredential();
            TokenRequestContext tokenRequestContext = new TokenRequestContext(new string[]
            {
                resource
            });
            AccessToken accessToken = await credential.GetTokenAsync(tokenRequestContext);
            ViewBag.AccessToken = accessToken.Token;

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken.Token);
            HttpResponseMessage httpResponseMessage = await _httpClient.GetAsync(apiUrl);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                string response = await httpResponseMessage.Content.ReadAsStringAsync();
                dynamic json = JsonConvert.DeserializeObject<dynamic>(response);
                ViewBag.DateTime = json.datetime;
            }

            return View();
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
