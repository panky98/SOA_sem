using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ServiceStack.Redis;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text.Json;
using StableConditionsSensorMS.Models;
using DataMicroservice.Services;

namespace DataMicroservice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataMicroserviceController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<DataMicroserviceController> _logger;
        private readonly DeviceService deviceService;
        public DataMicroserviceController(ILogger<DataMicroserviceController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("addRow")]
        public async Task<IActionResult> addRow([FromBody] Entry deviceData)
        {
            deviceService.addRow(deviceData);
            return Ok();
        }
        [HttpGet]
        [Route("getAllSensorData/{sensorStandardMac}")]
        public async Task<IActionResult> getAllSensorData([FromRoute(Name ="sensorStandardMac")] string sensorStandardMac)
        {
            IList<Entry> entries = deviceService.GetEntries(sensorStandardMac);  
            return new JsonResult(entries);
        }
    }
}
