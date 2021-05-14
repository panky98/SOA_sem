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
        readonly RedisClient redis = new RedisClient("localhost");

        public DataMicroserviceController(ILogger<DataMicroserviceController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("addRow")]
        public async Task<IActionResult> addRow([FromBody] Entry deviceData)
        {

            string standardMac = deviceData.Sensor.Replace(':', '-');
            redis.IncrementValueInHash("devices", standardMac, 1);
            long numberOfDevices = redis.GetHashCount("devices");
            string deviceInputNumber = redis.GetValueFromHash("devices", standardMac);
            redis.SetEntryInHash(standardMac + ":" + deviceInputNumber, "Co", deviceData.Co.ToString());
            redis.SetEntryInHash(standardMac + ":" + deviceInputNumber, "Humidity", deviceData.Humidity.ToString());
            redis.SetEntryInHash(standardMac + ":" + deviceInputNumber, "Light", deviceData.Light.ToString());
            redis.SetEntryInHash(standardMac + ":" + deviceInputNumber, "Lpg", deviceData.Lpg.ToString());
            redis.SetEntryInHash(standardMac + ":" + deviceInputNumber, "Motion", deviceData.Motion.ToString());
            redis.SetEntryInHash(standardMac + ":" + deviceInputNumber, "MS", deviceData.MS.ToString());
            redis.SetEntryInHash(standardMac + ":" + deviceInputNumber, "Sensor", deviceData.Sensor.ToString());
            redis.SetEntryInHash(standardMac + ":" + deviceInputNumber, "Smoke", deviceData.Smoke.ToString());
            redis.SetEntryInHash(standardMac + ":" + deviceInputNumber, "Temp", deviceData.Temp.ToString());
            return Ok();
        }
        [HttpGet]
        [Route("getAllSensorData/{sensorStandardMac}")]
        public async Task<IActionResult> getAllSensorData([FromRoute(Name ="sensorStandardMac")] string sensorStandardMac)
        {
            long numberOfInputs = long.Parse( redis.GetValueFromHash("devices", sensorStandardMac));
            IList<Entry> entries = new List<Entry>();
            for (long i = 1; i <= numberOfInputs; i++)  
            {
                double co;
                double humidity;
                bool light;
                double lpg;
                bool motion;
                double smoke;
                double temp;
                double ms;
                string sensor;
                Double.TryParse(redis.GetValueFromHash(sensorStandardMac + ":" + i, "Co"),out co);
                Double.TryParse(redis.GetValueFromHash(sensorStandardMac + ":" + i, "Humidity"),out humidity);
                Boolean.TryParse(redis.GetValueFromHash(sensorStandardMac + ":" + i, "Light"),out light);
                Double.TryParse(redis.GetValueFromHash(sensorStandardMac + ":" + i, "Lpg"),out lpg);
                Boolean.TryParse(redis.GetValueFromHash(sensorStandardMac + ":" + i, "Motion"),out motion);
                Double.TryParse(redis.GetValueFromHash(sensorStandardMac + ":" + i, "Smoke"),out smoke);
                Double.TryParse(redis.GetValueFromHash(sensorStandardMac + ":" + i, "Temp"),out temp);
                Double.TryParse(redis.GetValueFromHash(sensorStandardMac + ":" + i, "MS"),out ms);
               sensor =redis.GetValueFromHash(sensorStandardMac + ":" + i, "Sensor");
                Entry entry = new Entry(co, humidity, light, lpg, motion, smoke, temp, ms, sensor);
                entries.Add(entry);
            }
            return new JsonResult(entries);
        }
    }
}
