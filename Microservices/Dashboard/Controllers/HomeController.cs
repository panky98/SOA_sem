using Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dashboard.Controllers
{
    public class HomeController : Controller
    {

        public HttpClient httpClient;
        public IList<Entry> entries { get; set; }

        public HomeController()
        {
            this.httpClient = new HttpClient();
        }

        public IActionResult Index()
        {
            return View();
        }

        [Route("Test")]
        [HttpGet]
        public async Task<IList<Entry>> Privacy()
        {
            HttpResponseMessage response = await httpClient.GetAsync("http://localhost:52807/api/getAllSensorData/1c-bf-ce-15-ec-4d");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                entries = JsonConvert.DeserializeObject<List<Entry>>(result);
            }
            return entries;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
