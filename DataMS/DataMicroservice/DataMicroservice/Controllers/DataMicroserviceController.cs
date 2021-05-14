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

        private readonly DeviceService deviceService;
        public DataMicroserviceController(DeviceService deviceService)
        {
            this.deviceService = deviceService;
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
        [HttpGet]
        [Route("getRangeSensorData/{sensorStandardMac}/{attributeName}/{lowerBound}/{upperBound}")]
        public async Task<IActionResult> GetRangeSensorData([FromRoute(Name = "sensorStandardMac")] string sensorStandardMac,
            [FromRoute(Name = "attributeName")] string attributeName,
            [FromRoute(Name = "lowerBound")] double lowerBound, [FromRoute(Name = "upperBound")] double upperBound)
        {
            IList<Entry> entries = deviceService.GetRangeEntries(sensorStandardMac,attributeName,lowerBound,upperBound);
            return new JsonResult(entries);
        }
        
    }
}
