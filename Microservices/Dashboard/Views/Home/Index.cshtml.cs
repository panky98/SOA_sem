using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace TreciCas.Pages
{
    public class IndexModel : PageModel
    {
        public List<Entry> entries { get; set; }
        public async Task OnGet()
        {
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync("http://localhost:52807/api/getAllSensorData/1c-bf-ce-15-ec-4d");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                entries = JsonConvert.DeserializeObject<List<Entry>>(result);
            }
        }
    }
}
